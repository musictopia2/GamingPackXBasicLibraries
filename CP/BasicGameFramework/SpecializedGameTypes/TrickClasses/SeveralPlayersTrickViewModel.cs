﻿using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public class SeveralPlayersTrickViewModel<SU, T, P, SA> : BasicTrickAreaViewModel<SU, T>, IMultiplayerTrick<SU, T, P>
        , ITrickPlay, IAdvancedTrickProcesses
        where SU : struct, Enum
        where T : class, ITrickCard<SU>, new()
        where P : class, IPlayerTrick<SU, T>, new()
        where SA : BasicSavedTrickGamesClass<SU, T, P>
    {
        protected ITrickGameMainProcesses<SU, T, P, SA> MainGame; //will be this one this time.
        public CustomBasicList<TrickCoordinate>? ViewList { get; set; }
        protected DeckObservableDict<T> OrderList => MainGame.SaveRoot!.TrickList;
        public SeveralPlayersTrickViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            MainGame = thisMod.MainContainer!.Resolve<ITrickGameMainProcesses<SU, T, P, SA>>();
        }
        private CustomBasicList<TrickCoordinate> GetCoordinateList()
        {
            CustomBasicList<TrickCoordinate> output = new CustomBasicList<TrickCoordinate>();
            int howManyPlayers = MainGame.PlayerList.Count();
            TrickCoordinate thisPlayer;
            if (howManyPlayers == 2)
            {
                thisPlayer = new TrickCoordinate();
                thisPlayer.IsSelf = true;
                thisPlayer.Row = 1;
                thisPlayer.Column = 1; // 1 based
                thisPlayer.Player = MainGame.SelfPlayer;
                output.Add(thisPlayer);
                thisPlayer = new TrickCoordinate();
                thisPlayer.Column = 2;
                thisPlayer.Row = 1;
                if (MainGame.SelfPlayer == 1)
                    thisPlayer.Player = 2;
                else
                    thisPlayer.Player = 1;
                output.Add(thisPlayer);
                return output;
            }
            int howManyBottom;
            int howManyTop;
            if (howManyPlayers <= 4)
                howManyBottom = 2;
            else if (howManyPlayers <= 6)
                howManyBottom = 3;
            else
                howManyBottom = 4;
            howManyTop = howManyPlayers - howManyBottom;
            if (howManyTop > howManyBottom)
                throw new Exception("Top can never have more than bottom");
            int x;
            int y;
            y = MainGame.SelfPlayer;
            var loopTo = howManyBottom;
            for (x = 1; x <= loopTo; x++)
            {
                thisPlayer = new TrickCoordinate();
                if (x == 1)
                    thisPlayer.IsSelf = true;
                thisPlayer.Row = 2;
                thisPlayer.Column = x;
                thisPlayer.Player = y;
                y += 1;
                if (y > howManyPlayers)
                    y = 1;
                output.Add(thisPlayer);
            }
            var loopTo1 = howManyTop;
            for (x = 1; x <= loopTo1; x++)
            {
                thisPlayer = new TrickCoordinate();
                thisPlayer.Column = x;
                thisPlayer.Row = 1;
                thisPlayer.Player = y;
                y += 1;
                if (y > howManyPlayers)
                    y = 1;
                output.Add(thisPlayer);
            }
            return output;
        }
        public void FirstLoad()
        {
            if (MainGame.PlayerList.Count() == 0)
                throw new BasicBlankException("Playerlist Has Not Been Initialized Yet");
            ViewList = GetCoordinateList();
            if (ViewList.First().IsSelf == false)
                throw new BasicBlankException("First must be self");
            int x;
            MaxPlayers = MainGame.PlayerList.Count();
            for (x = 1; x <= MaxPlayers; x++)
            {
                T newCard = new T();
                newCard.Populate(x);
                newCard.IsUnknown = true;
                newCard.Deck = x + 1000; //to try to make it work.
                CardList.Add(newCard); //i think
            }
        }
        public virtual void ClearBoard() // so the lead color for rage will happen after this.
        {
            DeckRegularDict<T> tempList = new DeckRegularDict<T>();
            int x = 0;
            foreach (var thisCard in CardList)
            {
                T tempCard = new T();
                x++;
                tempCard.Populate(x);
                tempCard.Deck += 1000; //try this way.
                tempCard.IsUnknown = true;
                tempCard.Visible = true;
                tempList.Add(tempCard);
            }
            OrderList.Clear();
            CardList.ReplaceRange(tempList); // hopefully its that simple.
            Visible = true; // now it is visible.
        }
        private int GetCardIndex()
        {
            var thisC = (from Items in ViewList
                         where Items.Player == MainGame.WhoTurn
                         select Items).Single();
            return ViewList!.IndexOf(thisC);
        }
        protected virtual void PopulateNewCard(T oldCard, ref T newCard) { }
        public virtual void LoadGame()
        {
            var tempList = OrderList.ToRegularDeckDict();
            ClearBoard();
            ViewList = GetCoordinateList(); // needs this as well
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
        protected override async Task ProcessCardClickAsync(T thisCard)
        {
            if (CardList.IndexOf(thisCard) == 0)
                await MainGame.CardClickedAsync(); //because only human can do it for 2 player trick games.
        }
        public P GetSpecificPlayer(int id)
        {
            return MainGame.PlayerList![id];
        }
    }
}