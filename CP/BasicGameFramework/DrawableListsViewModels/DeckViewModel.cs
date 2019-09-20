using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Collections.Specialized;
using System.Linq; //sometimes i do use linq.
using System.Threading.Tasks;
namespace BasicGameFramework.DrawableListsViewModels
{
    public interface IDeckClick : IBasicGameVM
    {
        Task DeckClicked(); //decided to be more specific.
    }
    public class DeckViewModel<D> : SimpleControlViewModel where D : IDeckObject, new()//decided to keep generics for this.
    {

        private readonly DeckObservableDict<D> _objectList;
        private readonly EventAggregator _thisE;
        private void CommandChanges()
        {
            DeckObjectClickedCommand.ReportCanExecuteChange();
        }
        public ControlCommand DeckObjectClickedCommand { get; set; }
        public DeckViewModel(EventAggregator thisE, IDeckClick thisClick) : base(thisClick) //realy don't want to have to resolve with this library.
        {
            _objectList = new DeckObservableDict<D>();
            this._thisE = thisE;
            DeckObjectClickedCommand = new ControlCommand(this, async Items => await thisClick.DeckClicked(), thisClick, thisClick.CommandContainer!);
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
        }
        public enum EnumStyleType
        {
            Unknown = 1,
            AlwaysKnown = 2
        }
        private EnumStyleType _DeckStyle = EnumStyleType.Unknown; // has to be property now because games like eagle wings can change mid game.
        public EnumStyleType DeckStyle
        {
            get
            {
                return _DeckStyle;
            }
            set
            {
                if (SetProperty(ref _DeckStyle, value) == true)
                {

                    _objectList.ForEach(Items =>
                    {
                        if (value == EnumStyleType.AlwaysKnown)
                            Items.IsUnknown = false;
                        else
                            Items.IsUnknown = true;
                    });
                }
            }
        }
        public string TextToAppear
        {
            get
            {
                if (DrawInCenter == false)
                    return NewText + " (" + _objectList.Count + ")";
                return _objectList.Count.ToString(); // needs to be this way.  all the ui needs to know is the text its drawing.
            }
        }
        public D CurrentDisplayCard
        {
            get
            {
                if (_objectList.Count == 0)
                    return new D();
                return _objectList.First();
            }
        }
        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (SetProperty(ref _IsSelected, value) == true)
                {
                    CurrentDisplayCard.IsSelected = value; //try this too.
                }
            }
        }
        public bool NeverAutoDisable { get; set; }
        public bool CanCutDeck() //used for hit the deck
        {
            if (_objectList.Count < 5)
                return false;
            return true;
        }
        public bool CanFlipDeck()
        {
            if (_objectList.Count <= 1)
                return false;
            return true;
        }
        public bool IsEndOfDeck()
        {
            if (_objectList.Count == 0)
                return true;
            return false;
        }
        public int CardsLeft()
        {
            return _objectList.Count;
        }
        public void ClearCards()
        {
            _objectList.Clear();
        }
        public void ShuffleInExtraCards(IDeckDict<D> extraCards) //this is needed for games like life card game where some cards can't be in hand but must be in deck.
        {
            foreach (var thisCard in extraCards)
            {
                if (DeckStyle == EnumStyleType.AlwaysKnown)
                    thisCard.IsUnknown = false;
                else
                    thisCard.IsUnknown = true;
            }
            _objectList.AddRange(extraCards);
            _objectList.ShuffleList();//i think we need this too.
        }
        public DeckObservableDict<D> DeckList() // try this way
        {
            _objectList.ForEach(thisO => thisO.Visible = true);
            return _objectList.ToObservableDeckDict();
        }
        public DeckRegularDict<D> SavedList()
        {
            return _objectList.ToRegularDeckDict();
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TextToAppear));
            if (DidOriginal == true)
            {
                _thisE.NewCardMessage(EnumNewCardCategories.deck);
            }
            OnPropertyChanged(nameof(CurrentDisplayCard));
        }
        public DeckRegularDict<D> DrawSeveralCards(int howMany, bool showErrors = true)
        {
            if (_objectList.Count < howMany && showErrors)
                throw new BasicBlankException("Not enough cards to draw.  Needs at least " + howMany + ".  There are " + _objectList.Count + " cards left");
            else if (_objectList.Count < howMany)
                howMany = _objectList.Count;
            DeckRegularDict<D> TempList = _objectList.Take(howMany).ToRegularDeckDict();
            _objectList.RemoveGivenList(TempList, NotifyCollectionChangedAction.Remove); //try this way.  so the event happens once, and not several times.
            TempList.MakeAllObjectsKnown();
            return TempList;
        }
        public D DrawCard()
        {
            if (_objectList.Count == 0)
                throw new BasicBlankException("Cannot draw a card because there are no more cards left");
            D thisCard;
            thisCard = _objectList.First();
            _objectList.RemoveFirstItem();
            thisCard.IsUnknown = false;
            return thisCard;
        }
        public void ManuallyRemoveSpecificCard(D thisCard)
        {
            thisCard.IsUnknown = false; // is not unknown because its being drawn manually (like in games like cousin)
            _objectList.RemoveSpecificItem(thisCard);
        }
        public void AddCard(D thisCard)
        {
            _objectList.Add(thisCard);
        }
        public void PutInMiddle(D thisCard)
        {
            int index;
            index = _objectList.Count / 2;
            _objectList.InsertMiddle(index, thisCard);
        }
        public D CutTheDeck()
        {
            D thisCard;
            int index;
            index = _objectList.Count / 2;
            thisCard = _objectList[index];
            _objectList.RemoveSpecificItem(thisCard);
            return thisCard;
        }
        public CustomBasicList<int> GetCardIntegers()
        {
            return (from Items in _objectList
                    select Items.Deck).ToCustomBasicList();
        }
        public DeckObservableDict<D> FlipCardList()
        {
            _objectList.Reverse();
            DeckObservableDict<D> TempList = _objectList.ToObservableDeckDict();
            ClearCards();
            return TempList;
        }
        public D RevealCard()
        {
            if (_objectList.Count == 0)
                throw new BasicBlankException("Cannot reveal a card because there are no more cards left");
            return _objectList.First();
        }
        public void HideText()
        {
            NewText = ""; //this is used in cases where not enough room.  in that case, can just hide it.
        }
        private string NewText = "Deck";
        public void InsertText(string whatText)
        {
            NewText = whatText;
            OnPropertyChanged(nameof(TextToAppear)); // the text to display will change because of inserting text
        }
        public bool CardExists(int deck)
        {
            return _objectList.ObjectExist(deck); // i think
        }
        private bool _DrawInCenter;
        public bool DrawInCenter
        {
            get
            {
                return _DrawInCenter;
            }
            set
            {
                if (SetProperty(ref _DrawInCenter, value) == true) { }
            }
        }
        private bool DidOriginal;
        public void OriginalList(IEnumerableDeck<D> thisList)
        {
            DidOriginal = true;
            _objectList.ReplaceRange(thisList); // i think
            foreach (var thisCard in thisList)
            {
                if (DeckStyle == EnumStyleType.AlwaysKnown)
                    thisCard.IsUnknown = false;
                else
                    thisCard.IsUnknown = true;
            }
        }
        protected override bool CanEnableFirst()
        {
            if (_objectList.Count == 0 && NeverAutoDisable == false)
                return false;
            return base.CanEnableFirst();
        }
        protected override void EnableChange()
        {
            CommandChanges();
        }
        protected override void VisibleChange()
        {
            CommandChanges();
        }
        protected override void PrivateEnableAlways() { }//we want anybody inheriting to know it exists.
    }
}