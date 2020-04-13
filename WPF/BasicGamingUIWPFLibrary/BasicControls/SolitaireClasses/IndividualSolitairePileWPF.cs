using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses
{
    public class IndividualSolitairePileWPF : UserControl
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(IndividualSolitairePileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsSelectedPropertyChanged)));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (IndividualSolitairePileWPF)sender;
            if (thisItem.IsSelected == true)
                thisItem._thisStack!.Background = Brushes.Red;
            else
                thisItem._thisStack!.Background = Brushes.Transparent;
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(IndividualSolitairePileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandPropertyChanged)));
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        private static void CommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        public static readonly DependencyProperty DoubleCommandProperty = DependencyProperty.Register("DoubleCommand", typeof(ICommand), typeof(IndividualSolitairePileWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DoubleCommandPropertyChanged)));

        public ICommand DoubleCommand
        {
            get
            {
                return (ICommand)GetValue(DoubleCommandProperty);
            }
            set
            {
                SetValue(DoubleCommandProperty, value);
            }
        }
        private static void DoubleCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public PileInfoCP? ThisPile;
        public SolitairePilesCP? MainMod;
        private StackPanel? _thisStack;
        private ScrollViewer? _thisScroll;
        private float _minWidth;
        private DeckObservableDict<SolitaireCard>? _cardList;
        protected override Size MeasureOverride(Size constraint)
        {
            if (_cardList!.Count > 0)
                return base.MeasureOverride(constraint);
            return new Size(_minWidth, 200); //hopefully that works (?)
        }
        public void LoadSavedBoard()
        {
            _thisStack!.Children.Clear();
            PopulateList();
        }
        public void Init()
        {
            if (ThisPile == null)
                throw new BasicBlankException("Must send in the pile being used for this");
            if (MainMod == null)
                throw new BasicBlankException("There was no main viewmodel sent for this");
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Binding thisBind = new Binding(nameof(SolitairePilesCP.ColumnCommand));
            thisBind.Source = MainMod;
            SetBinding(CommandProperty, thisBind);
            thisBind = new Binding(nameof(SolitairePilesCP.DoubleCommand));
            thisBind.Source = MainMod;
            SetBinding(DoubleCommandProperty, thisBind);
            _cardList = ThisPile.CardList;
            DataContext = ThisPile;
            _cardList.CollectionChanged += CollectionChanged;
            MouseUp += PrivateSingleSolitairePileWPF_MouseUp;
            MouseDoubleClick += PrivateSingleSolitairePileWPF_MouseDoubleClick;
            _thisScroll = new ScrollViewer();
            _thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Vertical;
            SetBinding(IsSelectedProperty, new Binding(nameof(PileInfoCP.IsSelected)));
            PopulateList();
            _thisScroll.Content = _thisStack;
            Content = _thisScroll;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var newCard = thisItem as SolitaireCard;
                    DeckOfCardsWPF<SolitaireCard> thisD = new DeckOfCardsWPF<SolitaireCard>();
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
                        var thisCon = FindControl(oldCard!);
                        thisCon!.DataContext = null; //i think this is needed too
                        thisCon.DataContext = newCard; //the upperright is iffy.
                        x++;
                    }
                }
                else
                {
                    throw new BasicBlankException("Not sure when the numbers don't match");
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var oldCard = thisItem as SolitaireCard;
                    var thisCon = FindControl(oldCard!);
                    _thisStack!.Children.Remove(thisCon);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear();
                PopulateList();
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldStartingIndex == e.NewStartingIndex)
                {
                    RefreshItems();
                }
                else
                {
                    var firstCon = _thisStack!.Children[e.OldStartingIndex];
                    _thisStack.Children.Remove(firstCon);
                    _thisStack.Children.Insert(e.NewStartingIndex, firstCon);
                }
            }
            RecalculatePositioning();
            _thisScroll!.ScrollToBottom();
        }
        private void PrivateSingleSolitairePileWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ICommand tempCommand = Command;
            if (tempCommand.CanExecute(ThisPile))
                tempCommand.Execute(ThisPile);
        }
        private void PrivateSingleSolitairePileWPF_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ICommand tempCommand = DoubleCommand;
            if (tempCommand == null)
                return;
            if (tempCommand.CanExecute(ThisPile))
                tempCommand.Execute(ThisPile);
        }
        #region "Private Methods"
        private DeckOfCardsWPF<SolitaireCard>? FindControl(SolitaireCard thisCard)
        {
            foreach (var firstControl in _thisStack!.Children)
            {
                var thisGraphics = firstControl as DeckOfCardsWPF<SolitaireCard>;
                if (thisGraphics!.DataContext.Equals(thisCard))
                    return thisGraphics;
            }
            return null; //you can have null this time.
        }
        private void CalculateMargins(DeckOfCardsWPF<SolitaireCard> thisDeck, SolitaireCard thisCard)
        {
            float adds;
            if (thisCard.IsUnknown == false || MainMod!.IsKlondike == false)
            {
                adds = thisDeck.ObjectSize.Height / 4; //try this way
                adds = thisDeck.ObjectSize.Height - adds;
                adds *= -1;
            }
            else
            {
                adds = thisDeck.ObjectSize.Height - 7;
                adds *= -1;
            }
            thisDeck.Margin = new Thickness(6, 0, 6, adds);
        }
        private void RecalculatePositioning()
        {
            if (_cardList == null)
                return;
            _cardList.ForEach(thisCard =>
            {
                var thisDeck = FindControl(thisCard);
                if (thisDeck != null)
                {
                    if (thisCard.Equals(_cardList.Last()) == false)
                        CalculateMargins(thisDeck, thisCard);
                    else
                        thisDeck.Margin = new Thickness(6, 0, 6, 0);
                }
            });
        }
        private void PopulateList()
        {
            if (_cardList!.Count == 0)
            {
                DeckOfCardsWPF<SolitaireCard> thisGraphics = new DeckOfCardsWPF<SolitaireCard>();
                thisGraphics.SendSize(ts.TagUsed, new SolitaireCard());
                _minWidth = thisGraphics.ObjectSize.Width + 12;
                MinWidth = _minWidth;
                return;
            }
            _cardList.ForEach(thisCard =>
            {
                DeckOfCardsWPF<SolitaireCard> thisGraphics = new DeckOfCardsWPF<SolitaireCard>();
                thisGraphics.NeedsExtraSuitForSolitaire = true;
                thisGraphics.SendSize(ts.TagUsed, thisCard);
                _minWidth = thisGraphics.ObjectSize.Width + 12; //try this way.
                MinWidth = _minWidth; //do both.
                if (thisCard.Equals(_cardList.Last()) == false)
                    CalculateMargins(thisGraphics, thisCard);
                else
                    thisGraphics.Margin = new Thickness(6, 0, 6, 0);
                _thisStack!.Children.Add(thisGraphics);
            });
        }
        public void RefreshItems()
        {
            CustomBasicList<DeckOfCardsWPF<SolitaireCard>> tempList = new CustomBasicList<DeckOfCardsWPF<SolitaireCard>>();
            _cardList!.ForEach(thisCard =>
            {
                var thisD = FindControl(thisCard);
                tempList.Add(thisD!);
            });
            _thisStack!.Children.Clear();
            tempList.ForEach(thisD => _thisStack.Children.Add(thisD));
        }
        #endregion
    }
}
