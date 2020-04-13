using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIWPFLibrary.BasicControls.GameFrames
{

    public class BaseHandWPF<CA, GC, GW> : BaseFrameWPF
            where CA : IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<CA, GC>, new()
    {
        public static readonly DependencyProperty HandTypeProperty = DependencyProperty.Register("HandType", typeof(HandObservable<CA>.EnumHandList), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(HandObservable<CA>.EnumHandList.Horizontal, new PropertyChangedCallback(HandTypePropertyChanged)));
        public HandObservable<CA>.EnumHandList HandType
        {
            get
            {
                return (HandObservable<CA>.EnumHandList)GetValue(HandTypeProperty);
            }
            set
            {
                SetValue(HandTypeProperty, value);
            }
        }
        private static void HandTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseHandWPF<CA, GC, GW>)sender;
            thisItem.RedoStack();
        }
        public static readonly DependencyProperty DividerProperty = DependencyProperty.Register("Divider", typeof(double), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(DividerPropertyChanged)));
        public double Divider
        {
            get
            {
                return (double)GetValue(DividerProperty);
            }
            set
            {
                SetValue(DividerProperty, value);
            }
        }
        private static void DividerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseHandWPF<CA, GC, GW>)sender;
            thisItem.RedoStack();
        }
        public static readonly DependencyProperty AdditionalsProperty = DependencyProperty.Register("Additionals", typeof(double), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(AdditionalsPropertyChanged)));
        public double Additionals
        {
            get
            {
                return (double)GetValue(AdditionalsProperty);
            }
            set
            {
                SetValue(AdditionalsProperty, value);
            }
        }
        private static void AdditionalsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseHandWPF<CA, GC, GW>)sender;
            thisItem.RedoStack();
        }
        public static readonly DependencyProperty MaximumCardsProperty = DependencyProperty.Register("MaximumCards", typeof(int), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(new PropertyChangedCallback(MaximumCardsPropertyChanged)));
        public int MaximumCards
        {
            get
            {
                return (int)GetValue(MaximumCardsProperty);
            }
            set
            {
                SetValue(MaximumCardsProperty, value);
            }
        }
        private static void MaximumCardsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseHandWPF<CA, GC, GW>)sender;
            if (thisItem._alreadyLoaded == false)
                return;
            thisItem.RecalulateFrames();
        }
        public float MaximumWidthHeight { get; set; } // the only exception is if there is a maximum cards
        public static readonly DependencyProperty ExtraControlSpaceProperty = DependencyProperty.Register("ExtraControlSpace", typeof(double), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(new PropertyChangedCallback(ExtraControlSpacePropertyChanged)));
        public double ExtraControlSpace
        {
            get
            {
                return (double)GetValue(ExtraControlSpaceProperty);
            }
            set
            {
                SetValue(ExtraControlSpaceProperty, value);
            }
        }
        private static void ExtraControlSpacePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseHandWPF<CA, GC, GW>)sender;
            if (thisItem._alreadyLoaded == false)
                return;
            thisItem.RecalulateFrames();
        }
        private StackPanel? _thisStack;
        protected DeckObservableDict<CA>? ObjectList;
        protected ScrollViewer? ThisScroll; // had to make it protected after all because games like life card game needs it.
        private bool _alreadyLoaded; //for now, its okay to be readonly.  maybe it needs to change (?)
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(BaseHandWPF<CA, GC, GW>), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandPropertyChanged)));
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
        private GW? FindControl(CA thisCard)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.DataContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        private void RedoStack()
        {
            if (ObjectList == null == true)
                return;
            PositionMainControls();
            RecalculatePositioning();
            if (MaximumCards > 0)
                RecalulateFrames();
        }
        private void PositionMainControls()
        {
            var whichCat = HandType;
            if (whichCat == HandObservable<CA>.EnumHandList.Horizontal)
            {
                ThisScroll!.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                ThisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                _thisStack!.Orientation = Orientation.Horizontal;
            }
            else
            {
                ThisScroll!.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                ThisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                _thisStack!.Orientation = Orientation.Vertical;
            }
        }
        protected void RecalulateFrames() // try it this way.
        {
            if (MaximumCards > 0)
            {
                ComplexRecalulateFrames();
                return;
            }
            if (MaximumWidthHeight > 0)
                ComplexRecalulateFrames();
            var whatCat = HandType;
            if (whatCat == HandObservable<CA>.EnumHandList.Horizontal)
            {
                if (ExtraControlSpace > 0)
                    throw new BasicBlankException("Extra control width can only be used for vertical");
                ChangedBasedOnHeight(_sizeUsed.Height + 30); // for possible scrollbars 10 was not enough.
            }
            else
                ChangedBasedOnWidth(_sizeUsed.Width + 30 + (float)ExtraControlSpace);
        }
        private void ComplexRecalulateFrames()
        {
            double maxValue;
            var whatCat = HandType;
            double adds;
            if (whatCat == HandObservable<CA>.EnumHandList.Horizontal)
            {
                if (MaximumCards == 0)
                {
                    maxValue = MaximumWidthHeight;
                }
                else
                {
                    if (Divider == 1)
                    {
                        maxValue = (_sizeUsed.Width + Additionals) * MaximumCards;
                        maxValue += 30;
                    }
                    else if (MaximumCards == 1)
                        maxValue = _sizeUsed.Height + Additionals + 30;
                    else
                    {
                        adds = _sizeUsed.Width / Divider;
                        adds = _sizeUsed.Width - adds;
                        var firsts = adds;
                        adds *= -1;
                        adds = adds * MaximumCards - 1;
                        adds += (float)Additionals;
                        maxValue = _sizeUsed.Width * MaximumCards - 1;
                        maxValue += adds;
                        maxValue += firsts + (MaximumCards - 1 * Divider);
                    }
                }
            }
            else if (MaximumCards == 0)
                maxValue = MaximumWidthHeight;
            else if (Divider == 1)
            {
                maxValue = (_sizeUsed.Height + Additionals) * MaximumCards;
                maxValue += 30;
            }
            else if (MaximumCards == 1)
                maxValue = _sizeUsed.Height + Additionals + 30;
            else
            {
                adds = _sizeUsed.Height / Divider;
                adds = _sizeUsed.Height - adds;
                var firsts = adds;
                adds *= -1;
                adds = adds * MaximumCards - 1;
                adds += (float)Additionals;
                maxValue = _sizeUsed.Height * MaximumCards - 1;
                maxValue += adds;
                maxValue += firsts + (MaximumCards - 1 * Divider);
                maxValue += 30;
            }
            SKSize tempSize;
            if (whatCat == HandObservable<CA>.EnumHandList.Horizontal)
            {
                if (maxValue > CurrentWindow!.Width)
                {
                    maxValue = CurrentWindow.Width;
                    maxValue -= 80;
                }
                tempSize = new SKSize((float)maxValue, _sizeUsed.Height + 20);
            }
            else
            {
                if (maxValue > CurrentWindow!.Height)
                {
                    maxValue = CurrentWindow.Height;
                    maxValue -= 80;
                }
                float thisWidth;
                thisWidth = _sizeUsed.Width + 20;
                tempSize = new SKSize(thisWidth, (float)maxValue);
            }
            ChangeSizeBasedOnSizeNeeded(tempSize); // i think
        }
        private void CalculateMargins(GW thisDeck, CA thisCard)
        {
            var whatCat = HandType;
            double adds;
            //if (thisCard.Visible == false)
            //{
            //    return; //try this way.
            //}

            if (whatCat == HandObservable<CA>.EnumHandList.Horizontal)
            {
                if (Divider == 1 || ObjectList!.IndexOf(thisCard) == 0)
                    adds = (float)Additionals;
                else
                {
                    adds = thisDeck.ObjectSize.Width / Divider;
                    adds = thisDeck.ObjectSize.Width - adds;
                    adds *= -1;
                    adds += (float)Additionals;
                }
                _totalLeft += adds + thisDeck.ObjectSize.Width;
                thisDeck.Margin = new Thickness(adds, 0, 0, 0);
            }
            else
            {
                if (Divider == 1 || ObjectList!.IndexOf(thisCard) == 0)
                    adds = (float)Additionals;
                else
                {
                    adds = thisDeck.ObjectSize.Height / Divider;
                    adds = thisDeck.ObjectSize.Height - adds;
                    adds *= -1;
                    adds += (float)Additionals;
                }
                _totalTop += adds + thisDeck.ObjectSize.Height;
                thisDeck.Margin = new Thickness(0, adds, 0, 0);
            }
            SetLocations(thisDeck);
        }
        private void SetLocations(GW thisDeck)
        {
            SetTopProperty(thisDeck, _totalTop);
            SetLeftProperty(thisDeck, _totalLeft);
        }
        private double _totalLeft;
        private double _totalTop;
        protected void RecalculatePositioning() //needs to be protected so games like life card game can access it.
        {
            if (ObjectList == null == true)
                return;
            _totalLeft = 0;
            _totalTop = 0;
            int x = 0;

            DeckRegularDict<CA> vList = ObjectList.Where(x => x.Visible).ToRegularDeckDict();


            foreach (var thisCard in vList!)
            {
                x += 1;
                var deck = FindControl(thisCard);
                if (deck != null)
                {
                    if (x > 1)
                        CalculateMargins(deck, thisCard);
                    else
                    {
                        deck.Margin = new Thickness(0, 0, 0, 0);
                        SetLocations(deck);
                    }
                }
            }
        }
        private void CardBindings(GW thisDeck, CA thisCard)
        {
            thisDeck.SendSize(_tagUsed, thisCard); //i think this is where its needed.
            var thisBind = GetCommandBinding(nameof(HandObservable<CA>.ObjectSingleClickCommand));
            thisDeck.SetBinding(BaseDeckGraphicsWPF<CA, GC>.CommandProperty, thisBind);
            thisDeck.CommandParameter = thisCard;
        }
        public void RedoCards()
        {
            _thisStack!.Children.Clear();
            PopulateList();
        }
        private void PopulateList()
        {
            int x = 0;
            _totalLeft = 0;
            _totalTop = 0;
            foreach (var firstCard in ObjectList!)
            {
                GW thisGraphics = new GW(); // this does the bindings already as well
                CardBindings(thisGraphics, firstCard);
                CalculateMargins(thisGraphics, firstCard);
                _thisStack!.Children.Add(thisGraphics);
                x += 1;
            }
        }
        public void RefreshItems()
        {
            CustomBasicList<GW> tempList = new CustomBasicList<GW>();
            foreach (var thisCard in ObjectList!)
            {
                var thisD = FindControl(thisCard);
                tempList.Add(thisD!);
            }
            _thisStack!.Children.Clear();
            foreach (var ThisD in tempList)
                _thisStack.Children.Add(ThisD);
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var newCard = (CA)thisItem!;
                    GW thisD = new GW();
                    CardBindings(thisD, newCard);
                    _thisStack!.Children.Add(thisD);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count == e.NewItems.Count)
                {
                    int x;
                    var loopTo = e.OldItems.Count;
                    for (x = 1; x <= loopTo; x++)
                    {
                        var oldCard = (CA)e.OldItems[x - 1]!;
                        var newCard = (CA)e.NewItems[x - 1]!;
                        var thisCon = FindControl(oldCard);
                        thisCon!.DataContext = newCard;
                    }
                }
                else
                    throw new BasicBlankException("Not sure when the numbers don't match");
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems) //only old cards
                {
                    var oldCard = (CA)thisItem!;
                    var thisCon = FindControl(oldCard);
                    _thisStack!.Children.Remove(thisCon); // because not there anymore.
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // needs to clear and do nothing else because no cards left
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
            RecalculatePositioning();
            AfterCollectionChange();
        }
        protected virtual void AfterCollectionChange()
        {
            if (ObjectList.Any(Items => Items.Drew == true))
            {
                var thisObject = ObjectList.Where(Items => Items.Drew == true).Take(1).Single();
                var firstDeck = FindControl(thisObject);
                if (HandType == HandObservable<CA>.EnumHandList.Horizontal)
                    ThisScroll!.ScrollToHorizontalOffset(GetLeftProperty(firstDeck!));
                else
                    ThisScroll!.ScrollToVerticalOffset(GetTopProperty(firstDeck!));
            }
            else
            {
                ThisScroll!.ScrollToTop();
                ThisScroll!.ScrollToLeftEnd();
            }
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private SKSize _sizeUsed;
        private string _tagUsed = "";
        private HandObservable<CA>? _thisMod;
        public void UpdateList(HandObservable<CA> thisMod)
        {
            _thisMod = thisMod;
            DataContext = _thisMod;
            ObjectList!.CollectionChanged -= ObjectList_CollectionChanged;
            ObjectList = thisMod.HandList;
            ObjectList.CollectionChanged += ObjectList_CollectionChanged;
            _thisStack!.Children.Clear(); //best just to start over in this case.
            PopulateList();
        }
        public void LoadList(HandObservable<CA> thisMod, string tagUsed) // don't need the cardlist because that is in the viewmodel
        {
            CA firstCard;
            _tagUsed = tagUsed;
            DataContext = thisMod;
            SetBinding(TextProperty, nameof(HandObservable<CA>.Text));
            SetBinding(IsEnabledProperty, nameof(HandObservable<CA>.IsEnabled));
            _thisMod = thisMod;
            if (_alreadyLoaded == true)
            {
                throw new BasicBlankException("Needs some rethinking because i intended to load once");
            }
            if (_thisMod.HandList.Count == 0)
                firstCard = new CA();
            else
                firstCard = _thisMod.HandList.First();

            if (firstCard.DefaultSize.Height == 0 || firstCard.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height must be greater than 0 when initializging hand wpf");
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisI = thisR.Resolve<IProportionImage>(tagUsed);
            _sizeUsed = firstCard.DefaultSize.GetSizeUsed(thisI.Proportion);
            ObjectList = _thisMod.HandList;
            ObjectList.CollectionChanged += ObjectList_CollectionChanged;
            ThisScroll = new ScrollViewer();
            _thisStack = new StackPanel();
            PositionMainControls();
            PopulateList();
            SetBinding(CommandProperty, nameof(HandObservable<CA>.BoardSingleClickCommand));
            SetBinding(MaximumCardsProperty, nameof(HandObservable<CA>.Maximum));
            SetVisibleConverter();
            _thisGrid = new Grid();
            _thisGrid.Children.Add(ThisDraw);
            ThisScroll.Content = _thisStack;
            _thisGrid.Children.Add(ThisScroll);
            var thisRect = ThisFrame.GetControlArea();
            ThisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3); // try this way.
            RecalulateFrames();
            Content = _thisGrid;
            _alreadyLoaded = true;
        }
        protected virtual void SetVisibleConverter()
        {
            Binding binding = new Binding(nameof(HandObservable<CA>.Visible));
            binding.Converter = new BooleanToVisibilityConverter();
            SetBinding(VisibilityProperty, binding);
        }
        private Grid? _thisGrid;
        public BaseHandWPF()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            MouseUp += BaseHandWPF_MouseUp;
        }
        private void BaseHandWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tempCommand = Command;
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(null) == true)
                    tempCommand.Execute(null);
            }
        }
    }
}
