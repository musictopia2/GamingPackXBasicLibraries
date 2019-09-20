using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.GraphicsViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using System.Linq;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using Xamarin.Forms;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
namespace SolitaireGraphicsXF
{
    public class IndividualSolitairePileXF : ContentView
    {
        private readonly SolitaireOverlapLayoutXF _thisStack;
        private ScrollView _thisScroll;
        public PileInfoCP? ThisPile { get; set; }
        public SolitairePilesCP? MainMod { get; set; }
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(propertyName: "IsSelected", returnType: typeof(bool), declaringType: typeof(IndividualSolitairePileXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsSelectedPropertyChanged);
        public bool IsSelected
        {
            get
            {
                return (bool) GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (IndividualSolitairePileXF)bindable;
            if (thisItem.IsSelected == true)
                thisItem._thisStack.BackgroundColor = Color.Red;
            else
                thisItem._thisStack.BackgroundColor = Color.Transparent;
        }
        private DeckObservableDict<SolitaireCard>? _cardList;
        public IndividualSolitairePileXF()
        {
            _thisStack = new SolitaireOverlapLayoutXF();
            _thisScroll = new ScrollView();
            _thisScroll.Orientation = ScrollOrientation.Vertical;
            _thisScroll.InputTransparent = true;
        }
        private void BindCard(DeckOfCardsXF<SolitaireCard> thisGraphics, SolitaireCard thisCard)
        {
            thisGraphics.NeedsExtraSuitForSolitaire = true; //i think
            thisGraphics.SendSize(ts.TagUsed, thisCard);
            thisGraphics.DisableInput = true;
        }
        private void PopulateList()
        {
            _cardList!.ForEach(thisCard =>
            {
                DeckOfCardsXF<SolitaireCard> thisGraphics = new DeckOfCardsXF<SolitaireCard>();
                BindCard(thisGraphics, thisCard);
                _thisStack!.Children.Add(thisGraphics);
            });
        }
        public void RefreshItems()
        {
            CustomBasicList<DeckOfCardsXF<SolitaireCard>> tempList = new CustomBasicList<DeckOfCardsXF<SolitaireCard>>();
            _cardList!.ForEach(thisCard =>
            {
                var thisD = _thisStack.FindControl(thisCard);
                tempList.Add(thisD!);
            });
            _thisStack!.Children.Clear();
            tempList.ForEach(thisD => _thisStack.Children.Add(thisD));
        }
        public void Init()
        {
            if (ThisPile == null)
                throw new BasicBlankException("Must send in the pile being used for this");
            if (MainMod == null)
                throw new BasicBlankException("There was no main viewmodel sent for this");
            _cardList = ThisPile.CardList;
            BindingContext = ThisPile;
            VerticalOptions = LayoutOptions.StartAndExpand; // had to actually expand.
            _thisStack.IsKlondike = MainMod.IsKlondike; // needed this as well.
            _cardList.CollectionChanged += CardList_CollectionChanged;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_thisStack);
            BlankClickXF thisBlank = new BlankClickXF();
            thisBlank.Init(MainMod.ColumnCommand, ThisPile);
            thisGrid.Children.Add(thisBlank);
            PopulateList();
            _thisScroll = new ScrollView();
            _thisScroll.Orientation = ScrollOrientation.Vertical;
            _thisScroll.InputTransparent = true;
            _thisScroll.Content = thisGrid;
            Content = _thisScroll;
            SetBinding(IsSelectedProperty, new Binding(nameof(PileInfoCP.IsSelected)));
        }
        public void LoadSavedBoard() //hopefully this simple.
        {
            _thisStack!.Children.Clear();
            PopulateList();
        }
        private async void CardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action ==NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var newCard = thisItem as SolitaireCard;
                    DeckOfCardsXF<SolitaireCard> thisD = new DeckOfCardsXF<SolitaireCard>();
                    thisD.NeedsExtraSuitForSolitaire = true;
                    thisD.SendSize(ts.TagUsed, newCard!);
                    _thisStack!.Children.Add(thisD);//hopefully this simple (?)
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count == e.NewItems.Count)
                {
                    int x = 0;
                    foreach (var thisItem in e.OldItems)
                    {
                        var oldCard = thisItem as SolitaireCard;
                        var newCard = e.NewItems[x] as SolitaireCard;
                        var thisCon = _thisStack.FindControl(oldCard!);
                        thisCon!.BindingContext = null; //i think this is needed too
                        thisCon.BindingContext = newCard; //the upperright is iffy.
                        x++;
                    }
                }
                else
                    throw new BasicBlankException("Not sure when the numbers don't match");
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var oldCard = thisItem as SolitaireCard;
                    var thisCon = _thisStack.FindControl(oldCard!);
                    _thisStack!.Children.Remove(thisCon!);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack.Children.Clear(); // needs to clear and do nothing else because no cards left
                PopulateList();
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldStartingIndex == e.NewStartingIndex)
                    RefreshItems();
                else
                {
                    var firstCon = _thisStack!.Children[e.OldStartingIndex];
                    _thisStack.Children.Remove(firstCon);
                    _thisStack.Children.Insert(e.NewStartingIndex, firstCon);
                }
            }
            if (_cardList!.Count == 0)
                return;
            var ThisCard = _cardList.Last();
            var FinCo = _thisStack.FindControl(ThisCard);
            await _thisScroll.ScrollToAsync(FinCo, ScrollToPosition.End, false);
        }
    }
}