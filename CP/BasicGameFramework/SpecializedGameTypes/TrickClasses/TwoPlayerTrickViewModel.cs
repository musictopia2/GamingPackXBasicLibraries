using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public class TwoPlayerTrickViewModel<SU, T, P, SA> : BasicTrickAreaViewModel<SU, T>,
        ITrickPlay, IAdvancedTrickProcesses
        where SU : struct, Enum
        where T : class, ITrickCard<SU>, new()
        where P : class, IPlayerTrick<SU, T>, new()
        where SA : BasicSavedTrickGamesClass<SU, T, P>
    {
        protected ITrickGameMainProcesses<SU, T, P, SA> MainGame; //will be this one this time.
        public void FirstLoad()
        {
            if (MainGame.PlayerList.Count() != 2)
                throw new BasicBlankException("Must have 2 players in order to load");
            int x;
            for (x = 1; x <= 2; x++)
            {
                T newCard = new T();
                newCard.Populate(x);
                newCard.Deck = x + 1000; //try to do it this way so at least it will work.
                newCard.IsUnknown = true;
                CardList.Add(newCard); //i think
            }
        }
        protected int GetCardIndex()
        {
            if (MainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return 0;
            return 1;
        }
        public bool IsLead => OrderList.Count == 0;
        public void ClearBoard()
        {
            DeckRegularDict<T> tempList = new DeckRegularDict<T>();
            int x = 0;
            foreach (var thisCard in CardList)
            {
                T tempCard = new T();
                x++;
                tempCard.Populate(x);
                tempCard.Deck += 1000; //try this way.  hopefully won't cause any other issues.
                tempCard.IsUnknown = true;
                tempCard.Visible = true;
                tempList.Add(tempCard);
            }
            OrderList.Clear();
            CardList.ReplaceRange(tempList); // hopefully its that simple.
            Visible = true; // now it is visible.
        }
        public virtual void LoadGame()
        {
            var tempList = OrderList.ToRegularDeckDict();
            ClearBoard();
            if (tempList.Count == 0)
                return;
            int index;
            int tempTurn;
            T lastCard;
            tempTurn = MainGame.WhoTurn;
            DeckRegularDict<T> otherList = new DeckRegularDict<T>();
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                MainGame.WhoTurn = thisCard.Player;
                MainGame.SingleInfo = MainGame.PlayerList!.GetWhoPlayer();
                index = GetCardIndex();
                lastCard = MainGame.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this tiem.
            MainGame.WhoTurn = tempTurn;
        }
        protected T GetWinningCard(int wins)
        {
            return OrderList.Single(items => items.Player == wins);
        }
        public async Task AnimateWinAsync(int wins)
        {
            var thisCard = GetWinningCard(wins);
            WinCard = thisCard;
            await AnimateWinAsync();
        }
        public async Task PlayCardAsync(int deck)
        {
            T thisCard = MainGame.GetSpecificCardFromDeck(deck);
            thisCard.Player = MainGame.WhoTurn;
            int index = GetCardIndex();
            T newCard = MainGame.GetBrandNewCard(deck);
            newCard.Player = MainGame.WhoTurn;
            newCard.Visible = true;
            OrderList.Add(newCard);
            TradeCard(index, newCard);
            await AfterPlayCardAsync(thisCard);
        }
        protected virtual async Task AfterPlayCardAsync(T thisCard) //overrided versions may need this.
        {
            if (OrderList.Count == MainGame.PlayerList.Count())
                await MainGame.EndTrickAsync();
            else
                await MainGame.ContinueTrickAsync();
        }
        protected DeckObservableDict<T> OrderList => MainGame.SaveRoot!.TrickList;
        public TwoPlayerTrickViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            MainGame = thisMod.MainContainer!.Resolve<ITrickGameMainProcesses<SU, T, P, SA>>();
        }
        protected override async Task ProcessCardClickAsync(T thisCard)
        {
            if (CardList.IndexOf(thisCard) == 0)
                await MainGame.CardClickedAsync(); //because only human can do it for 2 player trick games.
        }
    }
}