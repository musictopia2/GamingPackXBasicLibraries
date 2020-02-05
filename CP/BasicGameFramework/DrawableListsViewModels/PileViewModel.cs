using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.DrawableListsViewModels
{
    public class SavedDiscardPile<D> where D : IDeckObject, new() // hopefully this will work.  not sure though.
    {
        public D CurrentCard { get; set; } = new D();
        public DeckRegularDict<D> PileList { get; set; } = new DeckRegularDict<D>();
    }
    public class PileViewModel<D> : SimpleControlViewModel where D : IDeckObject, new()
    {
        private readonly DeckObservableDict<D> _objectList;
        private readonly EventAggregator _thisE;
        public event PileClickedEventHandler? PileClickedAsync;
        public delegate Task PileClickedEventHandler();
        public ControlCommand PileObjectClickedCommand { get; set; }
        //public ControlCommand DeckCardMoveCommand { get; set; }
        //public ControlCommand DeckCardDoubleClickedCommand { get; set; } // something else will handle what we do with it.
        public PileViewModel(EventAggregator thisE, IBasicGameVM thisMod) : base(thisMod) //realy don't want to have to resolve with this library.
        {
            _objectList = new DeckObservableDict<D>();
            _thisE = thisE;
            PileObjectClickedCommand = new ControlCommand(this, async items =>
            {
                if (PileClickedAsync == null)
                    return;
                await PileClickedAsync.Invoke();
            }, thisMod, thisMod.CommandContainer!);
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
        }
        private int _previousNum; // used to doublecheck
        private D _currentCard = new D();
        private bool _isFirst;
        public D CurrentCard
        {
            get
            {
                return _currentCard;
            }

            set
            {
                if (SetProperty(ref _currentCard, value) == true)
                    OnPropertyChanged(nameof(CurrentDisplayCard));// so the binding knows it changed.
            }
        }
        // found cases where the text is not discard.  however, we want this to be cross platform.   therefore, must be here
        private string _Text = "Discard"; // this is default for this but can change to whatever you need though.
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (SetProperty(ref _Text, value) == true) { }
            }
        }
        private void NotifyCommands()
        {
            PileObjectClickedCommand.ReportCanExecuteChange();
        }
        public SavedDiscardPile<D> GetSavedPile()
        {
            SavedDiscardPile<D> output = new SavedDiscardPile<D>();
            output.CurrentCard = CurrentCard;
            output.PileList = _objectList.ToRegularDeckDict();
            return output;
        }
        public void SavedDiscardPiles(SavedDiscardPile<D> save)
        {
            _objectList.Clear();
            if (save.PileList.Count == 0 && save.CurrentCard.Deck == 0)
            {
                CurrentCard = new D();
                return;
            }
            CurrentCard = save.CurrentCard;
            _objectList.ReplaceRange(save.PileList);
            _previousNum = _objectList.Count;
        }
        public void MakeKnown()
        {
            CurrentCard.IsUnknown = false;
        }
        public bool CanCutDiscard()
        {
            if (_objectList.Count < 4)
                return false;
            return true;
        }
        public void NewList(IDeckDict<D> whatList)
        {
            if (whatList.Count == 0)
                return;
            CurrentCard = whatList.First();
            CurrentCard.IsUnknown = false;
            CurrentCard.IsSelected = false;
            CurrentCard.Drew = false;
            whatList.RemoveFirstItem(); //try this way.  could fix at least one of the problems.
            whatList.ForEach(Items =>
            {
                Items.IsSelected = false;
                Items.Drew = false;
            });
            _objectList.ReplaceRange(whatList); // because we are replacing the entire range.
            _previousNum = _objectList.Count;
        }
        public bool CardExist(int deck)
        {
            if (deck == CurrentCard.Deck)
                return true;
            return _objectList.ObjectExist(deck);
        }
        public void AddSeveralCards(IDeckDict<D> whatList)
        {
            if (whatList.Count == 0)
                return;
            DeckRegularDict<D> newList = new DeckRegularDict<D>();
            if (CurrentCard.Deck > 0)
                newList.Add(CurrentCard);
            CurrentCard = whatList.Last();
            CurrentCard.IsUnknown = false;
            CurrentCard.IsSelected = false;
            CurrentCard.Drew = false;
            int x;
            for (x = whatList.Count - 1; x >= 1; x += -1)
            {
                whatList[x - 1].IsSelected = false;
                whatList[x - 1].Drew = false; // i think
                newList.Add(whatList[x - 1]);
            }
            _objectList.AddRange(newList);
            _previousNum = _objectList.Count; // i think
        }
        public int CardsLeft()
        {
            if (CurrentCard.Deck == 0)
                return 0;
            return _objectList.Count + 1;
        }
        public DeckObservableDict<D> FlipCardList() // will need the actual CardList.CardList
        {
            DeckObservableDict<D> tempList = _objectList.ToObservableDeckDict();
            tempList.Reverse();
            tempList.Add(CurrentCard); //i think
            ClearCards();
            return tempList;
        }
        public int HowManyInDiscard()
        {
            return _objectList.Count;
        }
        public DeckObservableDict<D> DiscardList()
        {
            return DiscardList(_objectList.Count);
        }
        public DeckObservableDict<D> DiscardList(int howManyToKeep)
        {
            if (_objectList.Count != _previousNum)
                throw new BasicBlankException("The list is wrong"); // 
            return _objectList.Take(howManyToKeep).ToObservableDeckDict(); //try this way
        }
        public D CurrentDisplayCard
        {
            get
            {
                return CurrentCard; // maybe this simple (well see)
            }
        }
        public D GetCardInfo()
        {
            return GetCardInfo(CurrentCard.Deck);
        }
        public D GetCardInfo(int deck)
        {
            if (deck == 0)
                throw new BasicBlankException("The deck cannot be 0 when trying to getcardinfo.  Use PileEmpty function to determine whether the card even exists or not");
            if (CurrentCard.Deck == deck)
                return CurrentCard;
            return _objectList.GetSpecificItem(deck);
        }
        public int CardSelected()
        {
            if (CurrentCard.Deck == 0)
                return 0;
            if (CurrentCard.IsSelected == false)
                return 0;
            return CurrentCard.Deck;
        }
        public void HighlightCard()
        {
            CurrentCard.Drew = true;
        }
        public void ClearCards()
        {
            _objectList.Clear();
            _previousNum = 0;
            if (CurrentCard.Deck == 0)
                return;
            CurrentCard = new D();
            _thisE.NewCardMessage(EnumNewCardCategories.discard);
        }
        public void UnhighlightCard()
        {
            CurrentCard.Drew = false;
        }
        public void UnselectCard()
        {
            CurrentCard.IsSelected = false;
        }
        public void IsSelected(bool selects)
        {
            CurrentCard.IsSelected = selects;
        }
        public void CardsReshuffled(int howManyToKeep)
        {
            DeckRegularDict<D> thisCol = _objectList.Skip(_objectList.Count - howManyToKeep + 1).ToRegularDeckDict();
            _objectList.ReplaceRange(thisCol); //try this way.
            CurrentCard.IsSelected = false;
            CurrentCard.Drew = false;
            _previousNum = _objectList.Count;
        }
        public void CardsReshuffled()
        {
            _objectList.Clear();
            CurrentCard.IsSelected = false;
            CurrentCard.Drew = false;
            _previousNum = 0;
        }
        public void CutDeck()
        {
            if (_objectList.Count < 4)
                throw new BasicBlankException("Cannot cut deck because there are less then 4 cards left");
            int Index;
            Index = _objectList.Count / 2;
            if (_objectList.ObjectExist(CurrentCard.Deck))
                _objectList.RemoveObjectByDeck(CurrentCard.Deck); //try this way.  not sure if it will later cause problems but its a risk to try to take.
            _objectList.InsertMiddle(Index, CurrentCard);
            RemoveFromPile();
        }
        public void AddRestOfDeck(IDeckDict<D> currentList)
        {
            if (currentList.ObjectExist(_objectList.First().Deck))
                _objectList.RemoveFirstItem(); //try to remove the first item.  hopefully does not cause another issue.
            _objectList.AddRange(currentList);
            _previousNum = _objectList.Count; // so far should be fine
        }
        public void AddCard(D thisCard)
        {
            if (thisCard.Deck == CurrentCard.Deck)
                throw new Exception("Cannot put a card that is already here");
            if (thisCard.Deck == -1)
                throw new Exception("Cannot send a blank card.  Find out what happened");
            thisCard.IsUnknown = false;
            thisCard.IsSelected = false;
            thisCard.Drew = false; // i think
            if (CurrentCard.Deck == 0)
            {
                _previousNum = 0;
                _objectList.Clear();
            }
            else
            {
                if (CurrentOnly == false)
                {
                    if (_objectList.ObjectExist(thisCard.Deck) == true)
                        _objectList.RemoveObjectByDeck(thisCard.Deck);

                    if (_objectList.ObjectExist(CurrentCard.Deck))
                        _objectList.RemoveObjectByDeck(CurrentCard.Deck); //try this way.  not sure what problem this will cause.
                    _objectList.Add(CurrentCard);
                    _previousNum = _objectList.Count;
                }
                else
                {
                    CurrentCard = thisCard; // try this 
                    _thisE.NewCardMessage(EnumNewCardCategories.discard);
                    return; // try this way.
                }
            }
            FirstLoad(thisCard, thisCard.Deck);
        }
        public void FirstLoad(D thisCard)
        {
            FirstLoad(thisCard, 0);
        }
        private void FirstLoad(D thisCard, int deck)
        {
            if (_isFirst == true)
                return;
            if (deck > 0)
                CurrentCard = thisCard;
            _thisE.NewCardMessage(EnumNewCardCategories.discard);
            _isFirst = false;
        }
        public void RemoveCardFromPile(D thisCard)
        {
            if (thisCard.Deck == CurrentCard.Deck)
            {
                RemoveFromPile();
                return;
            }
            _objectList.RemoveSpecificItem(thisCard);
            _previousNum = _objectList.Count; // this way if a card has to get played again like fluxx; won't cause the problem that happened before
        }
        public void RemoveFromPile()
        {
            if (CurrentCard.Deck == 0)
                throw new BasicBlankException("Cannot remove from discard because there are no cards to remove");
            if (_objectList.Count == 0)
            {
                _previousNum = 0;
                CurrentCard = new D();
                _thisE.NewCardMessage(EnumNewCardCategories.discard);
                return;
            }
            CurrentCard = _objectList.Last();
            _objectList.RemoveLastItem();
            _previousNum = _objectList.Count;
        }
        public bool PileEmpty()
        {
            if (CurrentCard.Deck == 0)
                return true;
            return false;
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _thisE.NewCardMessage(EnumNewCardCategories.discard);
        }
        protected override void EnableChange()
        {
            NotifyCommands();
        }
        protected override void VisibleChange()
        {
            NotifyCommands();
        }
        protected override void PrivateEnableAlways() { }
        private bool _CurrentOnly;
        public bool CurrentOnly
        {
            get
            {
                return _CurrentOnly;
            }
            set
            {
                if (SetProperty(ref _CurrentOnly, value) == true) { }
            }
        }
    }
}