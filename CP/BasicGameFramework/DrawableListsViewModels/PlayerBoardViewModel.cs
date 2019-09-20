using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace BasicGameFramework.DrawableListsViewModels
{
    public class PlayerBoardViewModel<TR> : SimpleControlViewModel
        where TR : RegularTrickCard, new()
    {
        public enum EnumGameList
        {
            None = 0,
            Skuck = 1,
            HorseShoe = 2
        }
        private EnumGameList _Game = EnumGameList.None;
        public EnumGameList Game
        {
            get
            {
                return _Game;
            }
            set
            {
                if (SetProperty(ref _Game, value) == true)
                {
                    if (HowManyRows == 0)
                    {
                        if ((int)Game == (int)EnumGameList.HorseShoe)
                            HowManyRows = 2;
                        else
                            HowManyRows = 4;
                    }
                }
            }
        }
        public bool IsSelf { get; set; }
        public int HowManyRows { get; private set; } //this works too.
        private const int _columns = 4; // this is always 4
        public DeckObservableDict<TR> CardList = new DeckObservableDict<TR>();
        public event SelectedCardEventHandler? SelectedCard; // all this needs is selectedcard.  that way if you have a hand, then the hand can unselect all cards.
        public delegate void SelectedCardEventHandler();
        public ControlCommand<TR> CardCommand { get; set; }
        public PlayerBoardViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            CardCommand = new ControlCommand<TR>(this, thisCard =>
            {
                if (IsSelf == false || thisCard.IsEnabled == false || thisCard.Visible == false)
                    return; //because you can't do anything anyways.  hopefully this simple.
                if (thisCard.IsSelected == true)
                {
                    thisCard.IsSelected = false;
                    return;
                }
                CardList.UnselectAllObjects();
                thisCard.IsSelected = true;
                SelectedCard?.Invoke();
            }, thisMod, thisMod.CommandContainer!);
        }
        public void UnselectAllCards()
        {
            CardList.UnselectAllObjects();
        }
        protected override void EnableChange()
        {
            CardCommand.ReportCanExecuteChange();
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        public void ClearBoard(IDeckDict<TR> thisList)
        {
            if (Game == EnumGameList.None)
                throw new BasicBlankException("Must choose game");
            else if (Game == EnumGameList.Skuck && thisList.Count != 16)
                throw new BasicBlankException("Skuck requires 16 cards");
            else if (Game == EnumGameList.HorseShoe && thisList.Count != 8)
                throw new BasicBlankException("Horseshoe requires 8 cards");
            CardList.ReplaceRange(thisList);
            RepositionCards();
        }
        private void RepositionCards() //i think you do have to redo though when doing autoresume.
        {
            foreach (var thisCard in CardList)
            {
                var (row, column) = GetRowColumnData(thisCard);
                thisCard.IsEnabled = false; // must be proven true.
                if (Game == EnumGameList.HorseShoe)
                {
                    if ((IsSelf == true) & (row == 2))
                    {
                        thisCard.IsUnknown = false;
                        thisCard.IsEnabled = true;
                    }
                    else if ((row == 1) & (IsSelf == false))
                    {
                        thisCard.IsUnknown = false;
                        thisCard.IsEnabled = true;
                    }
                    else
                    {
                        thisCard.IsUnknown = true;
                    }
                }
                else if (Game == EnumGameList.Skuck)
                {
                    if ((IsSelf == true) & (row == 1))
                    {
                        thisCard.IsUnknown = false;
                        thisCard.IsEnabled = true;
                    }
                    else if ((IsSelf == true) & (row == 4))
                    {
                        thisCard.IsUnknown = false;
                    }
                    else if (IsSelf == true)
                    {
                        thisCard.IsUnknown = true;
                    }
                    else if ((row == 1) | (row == 4))
                    {
                        thisCard.IsUnknown = false;
                        if (row == 4)
                            thisCard.IsEnabled = true;
                    }
                    else
                    {
                        thisCard.IsUnknown = true;
                    }
                }
            }
        }
        public (int row, int column) GetRowColumnData(TR thisCard) // convert to 0 based if doing through ui.
        {
            int x; // i think we need this.
            int y;
            int z;
            int p = 0;
            bool _isMinus;
            if (((IsSelf == true) & (Game == EnumGameList.Skuck)) | ((IsSelf == false) & (Game == EnumGameList.HorseShoe)))
            {
                p = (HowManyRows * 2);
                z = HowManyRows + 1;
                _isMinus = true;
            }
            else
            {
                _isMinus = false;
                z = 0;
            }
            for (x = 1; x <= _columns; x++)
            {
                var loopTo = HowManyRows;
                for (y = 1; y <= loopTo; y++)
                {
                    if (_isMinus == false)
                        z += 1;
                    else
                        z -= 1;
                    var tempCard = CardList[z - 1];
                    if (tempCard.Deck == thisCard.Deck)
                        return (y, x);// hopefully this works.
                }
                if (_isMinus == true)
                    z += p;
            }
            throw new BasicBlankException("Can't find row/column data for Card With Deck Of " + thisCard.Deck);
        }
        public void HideCard(TR thisCard)
        {
            if (thisCard.Visible == false)
                throw new BasicBlankException("This card was already invisible.  Should not allow to click on it");
            int oldVisible = CardList.Count(Items => Items.Visible == false);
            thisCard.Visible = false;
            thisCard.IsEnabled = false;
            thisCard.IsSelected = false;
            var nums = NextCard(thisCard);
            if (nums > -1)
            {
                var newCard = CardList[nums];
                if (newCard.Deck == thisCard.Deck)
                    throw new BasicBlankException("Can't be the same card");
                newCard.IsUnknown = false;
                newCard.IsEnabled = true;
            };
            int newVisible = CardList.Count(Items => Items.Visible == false);
            if (oldVisible + 1 != newVisible)
                throw new BasicBlankException("Only one card should have been invisible after hiding card.  Find out what happened");
        }
        public TR GetCardByRowColumn(int row, int column)
        {
            foreach (var thisCard in CardList)
            {
                var (trow, tcolumn) = GetRowColumnData(thisCard);
                if (tcolumn == column && trow == row)
                    return thisCard;
            }
            throw new BasicBlankException("No Card Found For Row " + row + " Column " + column);
        }
        private int FindSpecificCard(int row, int column)
        {
            foreach (var thisCard in CardList)
            {
                var (trow, tcolumn) = GetRowColumnData(thisCard);
                if (tcolumn == column && trow == row)
                    return CardList.IndexOf(thisCard);
            }
            return -1;
        }
        private int NextCard(TR oldCard)
        {
            var (row, column) = GetRowColumnData(oldCard);
            bool isEnd;
            if (IsSelf == true)
            {
                if (Game == EnumGameList.Skuck)
                    isEnd = true;
                else
                    isEnd = false;
            }
            else if (Game == EnumGameList.HorseShoe)
            {
                isEnd = true;
            }
            else
            {
                isEnd = false;
            }

            if (isEnd == true && row == HowManyRows)
                return -1;// no more

            if (isEnd == false && row == 1)
                return -1;
            int newRow;
            newRow = row;
            if (isEnd == true)
                newRow += 1;
            else
                newRow -= 1;

            return FindSpecificCard(newRow, column);
        }
        public bool IsFinished => CardList.All(items => items.Visible == false); //hopefully this way works too.
        public DeckRegularDict<TR> ValidCardList => CardList.Where(items => items.Visible == true && items.IsUnknown == false &&
            items.IsEnabled == true).ToRegularDeckDict();
        public DeckRegularDict<TR> KnownList => CardList.Where(items => items.IsUnknown == false).ToRegularDeckDict();
        public int CardSelected => CardList.Where(items => items.IsSelected == true).Select(items => items.Deck).SingleOrDefault();
    }
}