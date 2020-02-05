using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using ps = BasicGameFramework.BasicDrawables.MiscClasses;
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class CardGameClass<D, P, S> : BasicGameClass<P, S>, ICardGameMainProcesses<D>, IReshuffledCardsNM, IDrawCardNM,
        IPickUpNM, IDiscardNM
        where D : class, IDeckObject, new()
        where P : class, IPlayerSingleHand<D>, new()
        where S : BasicSavedCardClass<P, D>, new()
    {
        public CardGameClass(IGamePackageResolver container) : base(container) { _thisMod = container.Resolve<IBasicCardGameVM<D>>(); }
        private readonly IBasicCardGameVM<D> _thisMod; //change after i have code done.
        public async Task SendDrawMessageAsync()
        {
            await ThisNet!.SendAllAsync("drawcard");
        }
        public IListShuffler<D>? DeckList; //i think this could be okay.
        public bool DoDraw; //somehow this was used.
        public int PlayerDraws { get; set; }
        public int LeftToDraw { get; set; }
        private IGameInfo? _thisGame;
        public ICardInfo<D>? CardInfo;
        private bool _didReshuffle;
        public PileViewModel<D>? OtherPile;  //i think belongs here.
        public int PreviousCard
        {
            get
            {
                return SaveRoot!.PreviousCard;
            }
            set
            {
                SaveRoot!.PreviousCard = value;
            }
        }
        public bool AlreadyDrew
        {
            get
            {
                return SaveRoot!.AlreadyDrew;
            }
            set
            {
                SaveRoot!.AlreadyDrew = value;
            }
        }
        public override Task FinishGetSavedAsync() //the overrided will do the regular and extras.
        {
            PlayerList!.ForEach(Items =>
            {
                Items.HookUpHand();
            });
            DeckList!.ClearObjects(); //just in case.
            DeckList.OrderedObjects(); //maybe this is needed in this case.
            var newList = SaveRoot!.PublicDeckList.GetNewObjectListFromDeckList(DeckList); //hopefully this will work.
            if (newList.Count > 0)
                _thisMod.Deck1!.OriginalList(newList);
            _thisMod.Pile1!.SavedDiscardPiles(SaveRoot.PublicDiscardData!);
            if (_thisGame!.SinglePlayerChoice != EnumPlayerChoices.HumanOnly)
                SingleInfo = PlayerList.GetSelf();
            if (_thisMod.PlayerHand1!.Visible == true)
            {
                if (CardInfo!.PlayerGetsCards == true)
                {
                    SetHand(); //i think needs this instead.  so other things can happen instead.
                    PrepSort(); //i think  if i am wrong, rethink.
                    SortCards(); //looks like its better to be safe than sorry.
                }
            }
            if (OtherPile != null)
            {
                if (SaveRoot.CurrentCard > 0)
                {
                    var ThisCard = DeckList.GetSpecificItem(SaveRoot.CurrentCard);
                    if (OtherPile.PileEmpty() == true)
                        OtherPile.AddCard(ThisCard);
                }
                else
                {
                    if (OtherPile.PileEmpty() == false)
                        OtherPile.ClearCards();
                }
            }
            return Task.CompletedTask;
        }
        public override Task StartNewTurnAsync()
        {
            PrepStartTurn();
            AlreadyDrew = false;
            PreviousCard = 0;
            PlayerDraws = 0; // i think
            this.ShowTurn(); //must specify this though since its an extension.
            return Task.CompletedTask;
        }
        public override void Init()
        {
            base.Init(); //must have this too now on.
            _thisGame = MainContainer.Resolve<IGameInfo>();
            CardInfo = MainContainer.Resolve<ICardInfo<D>>();
            DeckList = MainContainer.Resolve<IListShuffler<D>>();
        }
        /// <summary>
        /// this is used to get extra information needed.
        /// for example card games needs extras for deck and discard piles.
        /// had to do this way so when the host sends the game state, they will have the extra data to deserialize properly.
        /// it is suggested to do the original plus your extras.
        /// has to be public so it can be used from another class
        /// </summary>
        /// <returns></returns>
        public override Task PopulateSaveRootAsync()
        {
            if (_thisMod.PlayerHand1!.Maximum > 0 && _thisMod.PlayerHand1.IgnoreMaxRules == false)
            {
                if (SingleInfo!.MainHandList.Count > _thisMod.PlayerHand1.Maximum) //needs to apply to both players.
                    throw new BasicBlankException("You have too many cards.  Rethink");
            }
            SaveRoot!.PublicDiscardData = _thisMod.Pile1!.GetSavedPile();
            SaveRoot.PublicDeckList = _thisMod.Deck1!.GetCardIntegers();
            if (OtherPile != null)
            {
                if (OtherPile.PileEmpty())
                    SaveRoot.CurrentCard = 0;
                else
                    SaveRoot.CurrentCard = OtherPile.GetCardInfo().Deck;
            }
            return Task.CompletedTask;
        }
        public int PlayerWentOut()
        {
            P thisPlayer = PlayerList.Where(Items => Items.MainHandList.Count == 0).SingleOrDefault();
            if (thisPlayer == null)
                return -1;
            return thisPlayer.Id;
        }
        public DeckRegularDict<D> GetPlayerCards()
        {
            var firstList = PlayerList.Select(Items => Items.MainHandList).ToCustomBasicList();
            DeckRegularDict<D> output = new DeckRegularDict<D>();
            firstList.ForEach(items => output.AddRange(items));
            return output;
        }
        public virtual async Task EndRoundAsync() //not sure if the interface needs this or not (?)
        {
            await SaveRoundAsync();
        }
        protected virtual Task SaveRoundAsync()
        {
            SaveRoot!.NewRound = true;
            ThisData!.NetworkStatus = EnumNetworkStatus.GameOrRoundOver;
            return Task.CompletedTask;
        }
        protected bool PlayerCanWin()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                return true;
            if (ThisTest!.ComputerEndsTurn == true || ThisTest.ComputerNoCards == true)
                return false; //the computer can't win under those conditions.
            return true;
        }
        public async Task SendDiscardMessageAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == false)
                return;
            await ThisNet!.SendAllAsync("discard", deck);
        }
        public async Task DiscardAsync(int deck) //done
        {
            var thisCard = DeckList!.GetSpecificItem(deck);
            thisCard.Drew = false;
            thisCard.IsSelected = false; //i think
            await DiscardAsync(thisCard);
        }
        protected async Task AnimatePlayAsync(D thisCard)
        {
            if (OtherPile != null && OtherPile.CurrentOnly == true)
                OtherPile.ClearCards();
            if (ThisTest!.NoAnimations == false)
            {
                await ThisE.AnimatePlayAsync(thisCard, finalAction: () =>
                {
                    if (_thisMod.Pile1!.Visible == true)
                        _thisMod.Pile1.AddCard(thisCard);
                }); //this is default.
            }
            else if (_thisMod.Pile1!.Visible == true)
            {
                _thisMod.Pile1.AddCard(thisCard);
            }
        }
        public virtual async Task DiscardAsync(D thisCard) //done
        {
            await AnimatePlayAsync(thisCard);
            await AfterDiscardingAsync(); //this is default.  but can override it.
        }
        protected virtual async Task AfterDiscardingAsync() //done.
        {
            await EndTurnAsync(); //most of the time, end turn but not always
        }
        public virtual async Task DrawAsync()
        {
            DoDraw = true;
            if (PlayerDraws == 0)
            {
                PlayerDraws = WhoTurn;
                if (LeftToDraw == 0)
                    LeftToDraw = 1;
                SingleInfo = PlayerList!.GetWhoPlayer();
                AlreadyDrew = true;
            }
            else if (CardInfo!.PlayerGetsCards == false)
            {
                LeftToDraw = 1;
            }
            else if (LeftToDraw == 0)
            {
                LeftToDraw = 1;
            }

            do
            {
                if (_thisMod.Deck1!.IsEndOfDeck() == true && _thisMod.Deck1.NeverAutoDisable == false)
                {
                    await AfterDrawingAsync(); //because its at the end of the deck and does not reshuffle.
                    return;
                }
                if (_thisMod.Deck1.IsEndOfDeck() == true)
                {
                    if (_didReshuffle == true)
                        throw new BasicBlankException("Already reshuffled.  Therefore; must be a problem.  Find out what happened");
                    _didReshuffle = true;
                    bool canSendMessage;
                    canSendMessage = SingleInfo!.CanSendMessage(ThisData!); //try to use this function here too.
                    if (canSendMessage == true || ThisData!.MultiPlayer == false)
                    {
                        if (CardInfo!.ShowMessageWhenReshuffling == true)
                            await _thisMod.ShowGameMessageAsync("Its the end of the deck; therefore; the cards are being reshuffled");
                        await ReshuffleCardsAsync(canSendMessage);
                    }
                    else
                    {
                        ThisCheck!.IsEnabled = true;
                    }

                    return;
                }
                _didReshuffle = false;
                LeftToDraw--;
                var thisCard = _thisMod.Deck1.DrawCard();
                if (CardInfo!.HasDrawAnimation == true && ThisTest!.NoAnimations == false)
                    await ThisE.AnimateDrawAsync(thisCard);
                if (CardInfo.PlayerGetsCards == false)
                {
                    await PlayerReceivesNoCardsAfterDrawingAsync(thisCard);
                    return;
                }
                if (OtherPile != null && CardToCurrentPile() == true)
                {
                    OtherPile.AddCard(thisCard);
                    await AfterDrawingAsync();
                    return;
                }
                var tempPlayer = PlayerList![PlayerDraws]; //not 0 based anymore.
                if (ShowNewCardDrawn(tempPlayer) == true)
                    thisCard.Drew = true;
                await AddCardAsync(thisCard, tempPlayer);
                if (LeftToDraw == 0)
                {
                    if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
                        SortAfterDrawing();
                    await AfterDrawingAsync();
                    return;
                }

            } while (true);
        }
        protected virtual void SortAfterDrawing() //games like blades of steele could do something else.
        {
            SortCards();//this should be fine.
        }
        protected virtual Task AddCardAsync(D thisCard, P tempPlayer) //games like spades 2 player, they may or may not add to list.
        {
            thisCard.IsUnknown = false;
            tempPlayer.MainHandList.Add(thisCard);
            return Task.CompletedTask;
        }
        protected virtual bool CardToCurrentPile()
        {
            return true; //defaults to true.
        }
        protected virtual bool ShowNewCardDrawn(P tempPlayer)
        {
            if (tempPlayer.PlayerCategory != EnumPlayerCategory.OtherHuman)
                return true;
            return false;
        }
        protected virtual async Task PlayerReceivesNoCardsAfterDrawingAsync(D thisCard)
        {
            await ThisE.AnimatePlayAsync(thisCard, () =>
            {
                if (_thisMod.Pile1!.Visible == true)
                    _thisMod.Pile1.AddCard(thisCard); //this needs a delegate as well.  otherwise, does not add to list.
            }); //usually just this.
            await AfterDrawingAsync();
        }
        protected async Task ReshuffleCardsAsync(bool canSend) //needs to be protected so games like hit the deck can call into it when cutting deck.
        {
            var thisCol = GetReshuffleList();
            thisCol.ShuffleList();
            _thisMod.Deck1!.OriginalList(thisCol); //i think here makes most sense.
            await MiddleReshuffleCardsAsync(thisCol, canSend);
        }
        protected virtual DeckObservableDict<D> GetReshuffleList() //games like milk run requires something else to get the list of cards to reshuffle.
        {
            var thisCol = _thisMod.Pile1!.DiscardList();
            if (CardInfo!.ReshuffleAllCardsFromDiscard == true)
            {
                thisCol.Add(_thisMod.Pile1.GetCardInfo());
                //try to not have it here.  otherwise, only works for host but not for clients.
                _thisMod.Pile1.ClearCards(); //did call it cards.  probably best to leave it.
            }
            foreach (var thisCard in thisCol)
                thisCard.Reset();// has to be here.
            return thisCol;
        }
        protected virtual async Task MiddleReshuffleCardsAsync(IDeckDict<D> thisList, bool canSend)
        {
            if (canSend == true)
            {
                CustomBasicList<int> newList = thisList.ExtractIntegers(Items => Items.Deck);
                await ThisNet!.SendAllAsync("reshuffledcards", newList);
            }
            await AfterReshuffleAsync();
        }
        protected virtual async Task AfterReshuffleAsync() //at this time, the cards should have been reshuffled.
        {
            //previously did cardsreshuffled all the time.  taking a risk by making the changes.  could be better, worse, the same
            //can affect many games.
            if (CardInfo!.ReshuffleAllCardsFromDiscard)
                _thisMod!.Pile1!.ClearCards();
            else
                _thisMod.Pile1!.CardsReshuffled(); //this was forgotten.  was a serious problem.
            if (DoDraw == true)
            {
                await DrawAsync();
                return;
            }
            bool hadComputer = SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer;
            if (hadComputer == true)
                SingleInfo = PlayerList!.GetSelf();
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                await ContinueTurnAsync();
            if (hadComputer == true)
                SingleInfo = PlayerList!.GetWhoPlayer();
        }
        protected virtual async Task AfterDrawingAsync()
        {
            await ContinueTurnAsync();
        }
        protected virtual Task SetSaveRootObjectsAsync() { return Task.CompletedTask; }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            await StartShufflingCardsAsync(isBeginning);
        }
        private async Task StartShufflingCardsAsync(bool isBeginning)
        {
            DeckList!.ClearObjects(); //because it needs to shuffle all.
            DeckList.ShuffleObjects();
            await StartSetUpAsync(isBeginning);
        }
        protected virtual async Task StartSetUpAsync(bool isBeginning)
        {
            if (OtherPile != null)
            {
                OtherPile.CurrentOnly = true;
                OtherPile.ClearCards();
            }
            _thisMod.Pile1!.ClearCards(); //in the old version, this was in clear game.  there is no clear game.  i think its best to be safe than sorry.
            _thisMod.Deck1!.ClearCards(); //i think this could be needed too.   best to be safe than sorry.
            DeckRegularDict<D>? tempList = default;
            SaveRoot!.PreviousCard = 0; //i think when its starting new part, this needs to be 0.
            AlreadyDrew = false; //i think it should be here.
            if (CardInfo!.NoPass == false)
            {
                if (CardInfo.CardsToPassOut == 0)
                    throw new BasicBlankException("Cannot have 0 cards to pass out.  If there are truly 0 cards to pass out; then NoPass will be True");
                if (CardInfo.NeedsDummyHand == true)
                    PlayerList!.AddDummy();
                DeckRegularDict<D> firstList = DeckList!.ToRegularDeckDict();
                CardInfo.ExcludeList.ForEach(items =>
                {
                    if (firstList.ObjectExist(items))
                        firstList.RemoveObjectByDeck(items);
                });
                int cardsToPassOut;
                if (ThisTest!.CardsToPass > 0)
                    cardsToPassOut = ThisTest.CardsToPass;
                else
                    cardsToPassOut = CardInfo.CardsToPassOut;
                if (CardInfo.PassOutAll == false)
                {
                    bool rets;
                    int testCount = 0;
                    rets = MainContainer.ObjectExist<ITestCardSetUp<D, P>>();
                    if (rets == true)
                    {
                        if (ThisData!.GamePackageMode == EnumGamePackageMode.Production)
                            throw new BasicBlankException("Cannot have test hands because its production.");
                        ITestCardSetUp<D, P> testPass = MainContainer.Resolve<ITestCardSetUp<D, P>>();
                        await testPass.SetUpTestHandsAsync(PlayerList!, DeckList!);
                        PlayerList!.ForEach(items =>
                        {
                            testCount += items.StartUpList.Count;
                            items.StartUpList.ForEach(Card =>
                            {
                                firstList.RemoveObjectByDeck(Card.Deck); //because its already in hand.
                            });
                        });
                    }
                    ps.CardProcedures.PassOutCards(PlayerList!, firstList, cardsToPassOut, testCount, ThisTest.ComputerNoCards, ref tempList!);
                }
                else
                {
                    ps.CardProcedures.PassOutCards(PlayerList!, firstList, ThisTest.ComputerNoCards);
                }

                if (CardInfo.NeedsDummyHand == true)
                {
                    CardInfo.DummyHand = PlayerList!["dummy"].MainHandList;
                    PlayerList.RemoveDummy();
                }
            }
            else if (CardInfo.NeedsDummyHand == true)
            {
                throw new BasicBlankException("Cannot require dummy hand because no cards are even being passed out");
            }

            if (CardInfo.NoPass == false && CardInfo.PassOutAll == false)
                _thisMod.Deck1!.OriginalList(tempList!);
            else if (CardInfo.PassOutAll == false)
                _thisMod.Deck1!.OriginalList(DeckList!); //i think
            if (_thisGame!.SinglePlayerChoice != EnumPlayerChoices.HumanOnly)
                SingleInfo = PlayerList!.GetSelf(); //hopefully this is fine.
            if (CardInfo.NoPass == false)
            {
                if (_thisMod.PlayerHand1!.Visible == true)
                {
                    SetHand();
                    SetUpSelfHand(); //usually nothing but games like fluxx can do other things.
                    PrepSort();
                    if (CardInfo.CanSortCardsToBeginWith == true)
                    {
                        SortCards();
                        if (CardInfo.NeedsDummyHand == true)
                        {
                            if (_thisSort == null)
                                CardInfo.DummyHand.Sort();
                            else
                                CardInfo.DummyHand.Sort(_thisSort);
                        }
                    }
                }
                else
                {
                    LinkHand();
                }
            }
            else
            {
                LinkHand();
            }

            if (CardInfo.AddToDiscardAtBeginning == true && _thisMod.Pile1!.Visible == true && CardInfo.NoPass == false)
            {
                if (ThisTest!.AutoNearEndOfDeckBeginning == false)
                {
                    _thisMod.Pile1.AddCard(_thisMod.Deck1!.DrawCard()); //drawing a card for deck
                }
                else
                {
                    int Suggested = _thisMod.Deck1!.CardsLeft() - 2;
                    var FList = _thisMod.Deck1.DrawSeveralCards(Suggested);
                    _thisMod.Pile1.AddSeveralCards(FList);
                }
            }
            await LastPartOfSetUpBeforeBindingsAsync();
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected virtual void LinkHand() { }
        protected virtual void SetUpSelfHand() { }
        protected virtual void SetHand()
        {
            _thisMod.PlayerHand1!.HandList = SingleInfo!.MainHandList;
        }
        protected virtual Task LastPartOfSetUpBeforeBindingsAsync() { return Task.CompletedTask; }
        private ISortObjects<D>? _thisSort;
        protected void PrepSort() //games like spades may need to call this so it can link up and still sort.
        {
            bool rets;
            rets = MainContainer.ObjectExist<ISortObjects<D>>();
            if (rets == true)
            {
                _thisSort = MainContainer.Resolve<ISortObjects<D>>();
            }
        }
        public void SortCards()
        {
            if (_thisSort != null)
                _thisMod.PlayerHand1!.HandList.Sort(_thisSort);
            else
                _thisMod.PlayerHand1!.HandList.Sort();
        }
        protected void SortCards(IDeckDict<D> thisList) //this is needed for games like cribbage.
        {
            if (_thisSort != null)
                thisList.Sort(_thisSort);
            else
                thisList.Sort();
        }
        protected virtual Task PlayerChosenForPickingUpFromDiscardAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            return Task.CompletedTask;
        }
        protected virtual async Task AfterPickupFromDiscardAsync()
        {
            await ContinueTurnAsync();
        }
        public virtual async Task PickupFromDiscardAsync() //i think done.
        {
            await PlayerChosenForPickingUpFromDiscardAsync();
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("pickup"); //this simple this time.
            var thisCard = _thisMod.Pile1!.GetCardInfo();
            AlreadyDrew = true; //forgot this part.
            SaveRoot!.PreviousCard = thisCard.Deck;
            _thisMod.Pile1.RemoveFromPile();
            if (OtherPile != null && ThisTest!.NoAnimations == false)
                await ThisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartUpToCard, "otherpile");
            else if (ThisTest!.NoAnimations == false)
                await ThisE.AnimatePickUpDiscardAsync(thisCard);
            if (OtherPile != null)
            {
                OtherPile.AddCard(thisCard);
                await ContinueTurnAsync();
                return;
            }
            thisCard.Drew = true;
            SingleInfo!.MainHandList.Add(thisCard);
            SortCards();
            await AfterPickupFromDiscardAsync(); //forgot this line of code.
        }
        async Task IReshuffledCardsNM.ReshuffledCardsReceived(string data)
        {
            if (CardInfo!.ShowMessageWhenReshuffling == true)
                await _thisMod.ShowGameMessageAsync("Its the end of the deck; therefore; the cards are being reshuffled");
            CustomBasicList<int> firstList = await js.DeserializeObjectAsync<CustomBasicList<int>>(data);
            DeckObservableDict<D> newList = new DeckObservableDict<D>();
            firstList.ForEach(index =>
            {
                D card = new D();
                card.Populate(index);
                newList.Add(card);
                DeckList!.RelinkObject(index, card); //maybe this is the only case where its needed.
            });
            _thisMod.Deck1!.OriginalList(newList);
            await AfterReshuffleAsync();
        }
        async Task IDrawCardNM.DrawCardReceivedAsync(string data)
        {
            await SentDrawCardAsync(data);
        }
        protected virtual async Task SentDrawCardAsync(string data)
        {
            LeftToDraw = 0;
            PlayerDraws = 0;
            SingleInfo = PlayerList!.GetWhoPlayer();
            await DrawAsync();
        }
        async Task IPickUpNM.PickUpReceivedAsync(string data)
        {
            await PickupFromDiscardAsync();
        }
        async Task IDiscardNM.DiscardReceivedAsync(string data)
        {
            await DiscardAsync(int.Parse(data));
        }
    }
}