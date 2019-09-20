using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ViewModelInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class TrickGameClass<SU, T, P, SA> : CardGameClass<T, P, SA>,
        ITrickGameMainProcesses<SU, T, P, SA>, ITrickNM
        where SU : struct, Enum
        where T : class, ITrickCard<SU>, new()
        where P : class, IPlayerTrick<SU, T>, new()
        where SA : BasicSavedTrickGamesClass<SU, T, P>, new()
    {
        private readonly IBasicCardGameVM<T> _thisMod;
        public TrickGameClass(IGamePackageResolver container) : base(container)
        {
            _trickMod = MainContainer.Resolve<ITrickGameVM<SU>>();
            _thisMod = MainContainer.Resolve<IBasicCardGameVM<T>>();
        }
        private readonly ITrickGameVM<SU> _trickMod;
        public int SelfPlayer => PlayerList.Single(Items => Items.PlayerCategory == EnumPlayerCategory.Self).Id;
        public BasicTrickAreaViewModel<SU, T>? TrickArea1;
        protected ITrickData? TrickData;
        protected ITrickPlay? ThisPlay; //needs to be protected so games like pinochle can access it.
        protected virtual bool CanEnableTrickAreas => true; //has to be overridable because some trick games, you can't enable in some situations like when bidding.
        protected void LoadTrickAreas()
        {
            TrickArea1 = MainContainer.Resolve<BasicTrickAreaViewModel<SU, T>>();
            var temps = MainContainer.Resolve<IBasicEnableProcess>();
            TrickArea1.SendEnableProcesses(temps, () => CanEnableTrickAreas); //i think
            TrickData = MainContainer.Resolve<ITrickData>();
            ThisPlay = MainContainer.Resolve<ITrickPlay>();
            var thisA = MainContainer.Resolve<IAdvancedTrickProcesses>();
            thisA.FirstLoad();
        }
        protected virtual void LoadVM()
        {
            SaveRoot!.LoadTrickVM(_trickMod);
        }
        private bool HeartsValidMove(int deck)
        {
            var heartSave = (ITrickStatusSavedClass)SaveRoot!;
            var thisList = SaveRoot!.TrickList;
            var thisCard = DeckList!.GetSpecificItem(deck);
            EnumSuitList cardSuit = thisCard.GetSuit.GetRegularSuit();
            if (thisList.Count == 0)
            {
                if (heartSave.TrickStatus == EnumTrickStatus.FirstTrick)
                {
                    var tempCard = SingleInfo!.MainHandList.OrderBy(Items => Items.GetSuit).ThenBy(Items => Items.ReadMainValue).Take(1).Single();
                    if (tempCard.Deck == deck)
                        return true;
                    return false;
                }
                if (heartSave.TrickStatus == EnumTrickStatus.SuitBroken)
                    return true;
                if (cardSuit == EnumSuitList.Hearts)
                {
                    return !SingleInfo!.MainHandList.Any(Items => Items.GetSuit.GetRegularSuit() != EnumSuitList.Hearts);
                }
                return true;
            }
            var leadCard = thisList.First();
            if (leadCard.GetSuit.Equals(thisCard.GetSuit))
                return true;
            DeckObservableDict<T> tempList;
            if (TrickData!.HasDummy == true)
            {
                var temps = MainContainer.Resolve<ITrickDummyHand<SU, T>>(); //if you don't have it, then will raise an error.
                tempList = temps.GetCurrentHandList();
            }
            else
                tempList = SingleInfo!.MainHandList;
            if (tempList.Any(Items => Items.GetSuit.Equals(leadCard.GetSuit)))
                return false;
            if (heartSave.TrickStatus == EnumTrickStatus.FirstTrick)
            {
                if (cardSuit == EnumSuitList.Hearts && tempList.Any(Items => Items.GetSuit.GetRegularSuit() != EnumSuitList.Hearts))
                    return false;
                if (cardSuit == EnumSuitList.Spades && thisCard.ReadMainValue == 12)
                    return false;
            }
            return true;
        }
        private bool SpadesValidMove(int deck)
        {
            var spadeSave = (ITrickStatusSavedClass)SaveRoot!;
            var thisList = SaveRoot!.TrickList;
            var thisCard = DeckList!.GetSpecificItem(deck);
            EnumSuitList cardSuit = thisCard.GetSuit.GetRegularSuit();
            if (thisList.Count == 0)
            {
                if (spadeSave.TrickStatus == EnumTrickStatus.SuitBroken)
                    return true;
                if (cardSuit == EnumSuitList.Spades)
                    return !SingleInfo!.MainHandList.Any(Items => Items.GetSuit.GetRegularSuit() != EnumSuitList.Spades);
                return true;
            }
            var leadCard = thisList.First();
            if (leadCard.GetSuit.Equals(thisCard.GetSuit))
                return true;
            if (SingleInfo!.MainHandList.Any(Items => Items.GetSuit.Equals(leadCard.GetSuit)))
                return false;
            if (spadeSave.TrickStatus == EnumTrickStatus.FirstTrick)
            {
                if (cardSuit == EnumSuitList.Spades && SingleInfo.MainHandList.Any(Items => Items.GetSuit.GetRegularSuit() != EnumSuitList.Spades))
                    return false;
            }
            return true;
        }
        public virtual bool IsValidMove(int deck) //needs to be public so computer ai class can call into it.
        {
            if (TrickData!.TrickStyle == EnumTrickStyle.Hearts)
            {
                return HeartsValidMove(deck);
            }
            if (TrickData.TrickStyle == EnumTrickStyle.Spades)
                return SpadesValidMove(deck);
            var thisList = SaveRoot!.TrickList;
            if (thisList.Count == 0 && TrickData.FirstPlayerAnySuit == true)
                return true;
            var leadCard = thisList.First();
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (TrickData.FollowSuit == true && thisCard.GetSuit.Equals(leadCard.GetSuit))
                return true;
            DeckObservableDict<T> currentHand;
            if (TrickData.HasDummy)
            {
                ITrickDummyHand<SU, T> Temps = Resolve<ITrickDummyHand<SU, T>>();
                currentHand = Temps.GetCurrentHandList();
            }
            else
                currentHand = SingleInfo!.MainHandList;
            if (TrickData.MustFollow == true)
            {
                if (currentHand.Any(Items => Items.GetSuit.Equals(leadCard.GetSuit)))
                    return false; //because you have to follow suit
            }
            if (TrickData.HasTrump == true)
            {
                if (thisCard.GetSuit.Equals(SaveRoot.TrumpSuit))
                    return true;
            }
            if (TrickData.MustPlayTrump == true)
            {
                if (currentHand.Any(Items => Items.GetSuit.Equals(SaveRoot.TrumpSuit)))
                    return false; //because the setting says you have to play trump
            }
            return true;
        }
        protected virtual async Task PlayCardAsync(int deck) //most of the time this simple.  could rethink if necessary.
        {
            if (TrickData!.HasDummy == true)
            {
                ITrickDummyHand<SU, T> Temps = Resolve<ITrickDummyHand<SU, T>>();
                Temps.RemoveCard(deck);
            }
            else
                SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            await ThisPlay!.PlayCardAsync(deck);
        }
        protected string PlayErrorMessage { get; set; } = "Illegal Move";
        protected virtual int PossibleOtherSelected(int firstChosen, out string message)
        {
            message = "";
            return firstChosen;
        }
        public async Task CardClickedAsync() //this is a card being clicked on.
        {
            int decks;
            if (_thisMod.PlayerHand1!.AutoSelect == DrawableListsViewModels.HandViewModel<T>.EnumAutoType.SelectAsMany)
            {
                if (_thisMod.PlayerHand1.HowManySelectedObjects > 1)
                {
                    await _thisMod.ShowGameMessageAsync("Can you only choose one card at a time to play");
                    return;
                }
            }
            if (TrickData!.HasDummy == true)
            {
                ITrickDummyHand<SU, T> temps = Resolve<ITrickDummyHand<SU, T>>();
                decks = temps.CardSelected();
            }
            else
                decks = _thisMod.PlayerHand1.ObjectSelected();
            decks = PossibleOtherSelected(decks, out string message);
            if (message != "")
            {
                await _thisMod.ShowGameMessageAsync(message);
                return;
            }
            if (decks == 0) //if you did not choose a card, its already handled if no message is sent.
            {
                await _thisMod.ShowGameMessageAsync("Must choose a card to play");
                return;
            }
            if (IsValidMove(decks) == false)
            {
                await _thisMod.ShowGameMessageAsync(PlayErrorMessage);
                PlayErrorMessage = "Illegal Move"; //to set back.  will accomodate games like sixty six and maybe pinacle.
                return;
            }
            if (ThisData!.MultiPlayer == true)
            {
                await ThisNet!.SendAllAsync("trickplay", decks); //i think
            }
            await PlayCardAsync(decks);
        }
        protected virtual bool CanEndTurnToContinueTrick => true;
        public virtual async Task ContinueTrickAsync()
        {
            if (CanEndTurnToContinueTrick == true)
                await EndTurnAsync(); //usually will end turn but can have exceptions.
            else
                await ContinueTurnAsync();
        }
        public T GetBrandNewCard(int deck)
        {
            T thisCard = DeckList!.GetSpecificItem(deck);
            return (T)thisCard.CloneCard(); //hopefully this works.
        }
        public T GetSpecificCardFromDeck(int deck)
        {
            return DeckList!.GetSpecificItem(deck);
        }
        public abstract Task EndTrickAsync();
        async Task ITrickNM.TrickPlayReceivedAsync(int deck)
        {
            await PlayCardAsync(deck);
        }
    }
}