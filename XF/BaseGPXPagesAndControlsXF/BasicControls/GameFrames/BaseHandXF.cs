using BaseGPXPagesAndControlsXF.BasicControls.Layouts;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXPagesAndControlsXF.BasicControls.GameFrames
{
    public class BaseHandXF<CA, GC, GW> : BaseFrameXF
        where CA : IDeckObject, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsXF<CA, GC>, new()
    {

        public static readonly BindableProperty HandTypeProperty = BindableProperty.Create(propertyName: "HandType", returnType: typeof(HandViewModel<CA>.EnumHandList), defaultValue: HandViewModel<CA>.EnumHandList.Horizontal, declaringType: typeof(BaseHandXF<CA, GC, GW>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: HandTypePropertyChanged);
        public HandViewModel<CA>.EnumHandList HandType
        {
            get
            {
                return (HandViewModel<CA>.EnumHandList)GetValue(HandTypeProperty);
            }
            set
            {
                SetValue(HandTypeProperty, value);
            }
        }
        private static void HandTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseHandXF<CA, GC, GW>)bindable;
            thisItem.RedoStack();
        }
        public static readonly BindableProperty DividerProperty = BindableProperty.Create(propertyName: "Divider", returnType: typeof(double), declaringType: typeof(BaseHandXF<CA, GC, GW>), defaultValue: 1.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DividerPropertyChanged);
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
        private static void DividerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseHandXF<CA, GC, GW>)bindable;
            thisItem.RedoStack();
        }
        public static readonly BindableProperty AdditionalsProperty = BindableProperty.Create(propertyName: "Additionals", returnType: typeof(double), declaringType: typeof(BaseHandXF<CA, GC, GW>), defaultValue: 0.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: AdditionalsPropertyChanged);
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
        private static void AdditionalsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseHandXF<CA, GC, GW>)bindable;
            thisItem.RedoStack();
        }
        public static readonly BindableProperty ExtraControlSpaceProperty = BindableProperty.Create(propertyName: "ExtraControlSpace", returnType: typeof(double), declaringType: typeof(BaseHandXF<CA, GC, GW>), defaultValue: 0.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ExtraControlSpacePropertyChanged);
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
        private static void ExtraControlSpacePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseHandXF<CA, GC, GW>)bindable;
            thisItem.RedoStack();// i think
        }
        public static readonly BindableProperty MaximumCardsProperty = BindableProperty.Create(propertyName: "MaximumCards", returnType: typeof(int), declaringType: typeof(BaseHandXF<CA, GC, GW>), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: MaximumCardsPropertyChanged);
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
        private static void MaximumCardsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseHandXF<CA, GC, GW>)bindable;
            if (thisItem._alreadyLoaded == false)
                return;
            if (thisItem.HasAngles == true)
            {
                thisItem.RedoStack();
                return; // because nothing is done with the built in one.
            }
            thisItem._thisStack!.MaximumCards = (int)newValue;
            thisItem.RedoStack();
        }
        public bool HasAngles;
        public float MaximumWidthHeight { get; set; } // the only exception is if there is a maximum cards
        private GameOverlapLayout<CA, GC>? _thisStack;
        private StackLayout? _otherStack;
        protected DeckObservableDict<CA>? ObjectList;
        protected ScrollView? ThisScroll;
        private bool _alreadyLoaded;
        private SKSize _sizeUsed; //maybe this has to be sent into the other part (?)
        private string _tagUsed = "";
        private HandViewModel<CA>? _thisMod;
        protected GW? FindControl(CA thisCard)
        {
            if (HasAngles == false)
            {
                foreach (var thisCon in _thisStack!.Children)
                {
                    var deck = (GW)thisCon;
                    if (deck.BindingContext.Equals(thisCard) == true)
                        return deck;
                }
                return null;
            }
            foreach (var thisCon in _otherStack!.Children)
            {
                var deck = (GW)thisCon;
                if (deck.BindingContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        public void RedoCards() //not sure what i did on old version tablet (?)
        {
            _thisStack!.Children.Clear();
            PopulateList();
        }
        private void RedoStack()
        {
            if (ObjectList == null)
                return;
            PositionMainControls();
            RecalculatePositioning();
            
            if (MaximumCards > 0)
                RecalulateFrames();
        }
        protected async Task ScrollToBottomAsync() //hopefully this works (iffy).
        {
            if (HandType == HandViewModel<CA>.EnumHandList.Vertical)
                await ThisScroll!.ScrollToAsync(0, ThisScroll.Height, true);
            else
                await ThisScroll!.ScrollToAsync(ThisScroll.Width, 0, true);
        }
        protected void RecalculatePositioning()
        {
            if (HasAngles == false)
                _thisStack!.ForceLayout(); // do this instead.
            else
                _otherStack!.ForceLayout();
        }
        private void PositionMainControls()
        {
            var whichCat = HandType;
            if (whichCat == HandViewModel<CA>.EnumHandList.Horizontal)
            {
                ThisScroll!.Orientation = ScrollOrientation.Horizontal;
                if (HasAngles == false)
                    _thisStack!.Orientation = StackOrientation.Horizontal;
                else
                    _otherStack!.Orientation = StackOrientation.Horizontal;
            }
            else
            {
                if (HasAngles == false)
                    _thisStack!.Orientation = StackOrientation.Vertical;
                else
                    _otherStack!.Orientation = StackOrientation.Vertical;
                ThisScroll!.Orientation = ScrollOrientation.Vertical;
            }
            if (HasAngles == false)
            {
                _thisStack!.Spacing = Additionals;
                _thisStack.ExtraControlSpace = ExtraControlSpace;
                _thisStack.MaximumCards = MaximumCards;
                _thisStack.Divider = Divider;
            }
            else
                _otherStack!.Spacing = 0;// i think.
        }
        private void RecalulateFrames()
        {
            var whichCat = HandType;
            SizeRequest thisR;
            if (HasAngles == false)
                thisR = _thisStack!.Measure(double.PositiveInfinity, double.PositiveInfinity);
            else
                thisR = _otherStack!.Measure(double.PositiveInfinity, double.PositiveInfinity);
            if (whichCat == HandViewModel<CA>.EnumHandList.Horizontal)
                HeightRequest = thisR.Request.Height + ThisScroll!.Margin.Bottom + ThisScroll.Margin.Top;
            else
                WidthRequest = thisR.Request.Width + ThisScroll!.Margin.Left + ThisScroll.Margin.Right;
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void CardBindings(GW thisDeck, CA thisCard)
        {
            thisDeck.SendSize(_tagUsed, thisCard); //i think this is where its needed.
            var thisBind = GetCommandBinding(nameof(HandViewModel<CA>.ObjectSingleClickCommand));
            thisDeck.SetBinding(BaseDeckGraphicsXF<CA, GC>.CommandProperty, thisBind);
            thisDeck.CommandParameter = thisCard;
            FinishBindings(thisDeck, thisCard); //so i can do differently for yahtzee hands down.
        }
        protected virtual void FinishBindings(GW thisDeck, CA thisCard) { }
        private void PopulateList()
        {
            int x = 0;
            foreach (var firstCard in ObjectList!)
            {
                GW thisGraphics = new GW(); // this does the bindings already as well
                thisGraphics.HorizontalOptions = LayoutOptions.Start;
                thisGraphics.VerticalOptions = LayoutOptions.Start;
                CardBindings(thisGraphics, firstCard);
                if (HasAngles == false)
                    _thisStack!.Children.Add(thisGraphics);
                else
                    _otherStack!.Children.Add(thisGraphics);
                x += 1;
            }
            if (HasAngles == false)
                _thisStack!.ForceLayout();
            else
                _otherStack!.ForceLayout();
        }
        public void RefreshItems()
        {
            CustomBasicList<GW> tempList = new CustomBasicList<GW>();
            foreach (var thisCard in ObjectList!)
            {
                var thisD = FindControl(thisCard);
                tempList.Add(thisD!);
            }
            if (HasAngles == false)
                _thisStack!.Children.Clear();
            else
                _otherStack!.Children.Clear();
            foreach (var thisD in tempList)
            {
                if (HasAngles == false)
                    _thisStack!.Children.Add(thisD);
                else
                    _otherStack!.Children.Add(thisD);
            }
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
                    if (HasAngles == false)
                        _thisStack!.Children.Add(thisD);
                    else
                        _otherStack!.Children.Add(thisD);
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
                        thisCon!.BindingContext = newCard;
                        thisCon!.CommandParameter = newCard;
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
                    if (HasAngles == false)
                        _thisStack!.Children.Remove(thisCon!);
                    else
                        _otherStack!.Children.Remove(thisCon);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (HasAngles == false)
                    _thisStack!.Children.Clear();
                else
                    _otherStack!.Children.Clear(); // needs to clear and do nothing else because no cards left
                PopulateList();
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldStartingIndex == e.NewStartingIndex)
                    RefreshItems();
                else
                {
                    var firstCon = _thisStack!.Children[e.OldStartingIndex];
                    if (HasAngles == false)
                    {
                        _thisStack.Children.Remove(firstCon);
                        _thisStack.Children.Insert(e.NewStartingIndex, firstCon);
                    }
                    else
                    {
                        _otherStack!.Children.Remove(firstCon);
                        _otherStack.Children.Insert(e.NewStartingIndex, firstCon);
                    }
                }
            }
            if (HasAngles == false)
                _thisStack!.ForceLayout();
            else
                _otherStack!.ForceLayout();
            AfterCollectionChange();
        }
        protected virtual async void AfterCollectionChange()
        {
            if (ObjectList.Any(Items => Items.Drew == true))
            {
                var thisObject = ObjectList.Where(Items => Items.Drew == true).Take(1).Single();
                var firstDeck = FindControl(thisObject);
                await ThisScroll!.ScrollToAsync(firstDeck, ScrollToPosition.Center, false);
            }
            else
            {
                await ThisScroll!.ScrollToAsync(0, 0, false);
            }
        }
        private SKCanvasView? _drawControl;
        private readonly Grid _thisGrid;
        public BaseHandXF()
        {
            _thisGrid = new Grid();
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
        }
        public void UpdateList(HandViewModel<CA> thisMod)
        {
            _thisMod = thisMod;
            BindingContext = _thisMod;
            ObjectList!.CollectionChanged -= ObjectList_CollectionChanged;
            ObjectList = thisMod.HandList;
            ObjectList.CollectionChanged += ObjectList_CollectionChanged;
            if (HasAngles == false)
                _thisStack!.Children.Clear(); //best just to start over in this case.
            else
                _otherStack!.Children.Clear();
            PopulateList();
        }
        public void LoadList(HandViewModel<CA> thisMod, string tagUsed)
        {
            CA firstCard;
            _tagUsed = tagUsed;
            BindingContext = thisMod;
            SetBinding(TextProperty, new Binding(nameof(HandViewModel<CA>.Text)));
            SetBinding(IsEnabledProperty, new Binding(nameof(HandViewModel<CA>.IsEnabled)));
            _thisMod = thisMod;
            if (_alreadyLoaded == true)
                throw new BasicBlankException("Needs some rethinking because i intended to load once");
            if (_thisMod.HandList.Count == 0)
                firstCard = new CA();
            else
                firstCard = _thisMod.HandList.First();

            if (firstCard.DefaultSize.Height == 0 || firstCard.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height must be greater than 0 when initializging hand wpf");
            IGamePackageResolver thisR = (IGamePackageResolver)cons;
            IProportionImage thisI = thisR.Resolve<IProportionImage>(tagUsed);
            _sizeUsed = firstCard.DefaultSize.GetSizeUsed(thisI.Proportion);
            ObjectList = _thisMod.HandList;
            ObjectList.CollectionChanged += ObjectList_CollectionChanged;
            ThisScroll = new ScrollView();
            if (HasAngles == false)
            {
                _thisStack = new GameOverlapLayout<CA, GC>(_sizeUsed);
                _thisStack.Spacing = 0;
            }
            else
            {
                _otherStack = new StackLayout();
                _otherStack.Spacing = 0;
            }
            SetBinding(MaximumCardsProperty, new Binding(nameof(HandViewModel<CA>.Maximum)));
            PositionMainControls();
            PopulateList();
            SetVisibleConverter();
            _thisGrid.Children.Add(ThisDraw);
            ThisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Default;
            ThisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Default; //try this too.
            if (HasAngles == false)
                ThisScroll.Content = _thisStack;
            else
                ThisScroll.Content = _otherStack;
            var thisRect = ThisFrame.GetControlArea();
            if (HasAngles == false)
                ThisScroll.Margin = new Thickness(thisRect.Left - 1, thisRect.Top - 1, 0, 3); // try this way.
            else
                ThisScroll.Margin = new Thickness(thisRect.Left - 1, thisRect.Top - 1, 3, 3);
            _thisGrid.Children.Add(ThisScroll);
            RecalulateFrames();
            _drawControl = new SKCanvasView();
            _drawControl.EnableTouchEvents = true;
            _drawControl.Touch += DrawControlTouch;
            _thisGrid.Children.Add(_drawControl);
            Content = _thisGrid;
            _alreadyLoaded = true;
        }
        private void DrawControlTouch(object sender, SKTouchEventArgs e)
        {
            if (_thisMod!.BoardSingleClickCommand.CanExecute(null!) == true)
                _thisMod.BoardSingleClickCommand.Execute(null!);
        }
        protected virtual void SetVisibleConverter() // can be different because of payday.
        {
            Binding ThisBind = new Binding(nameof(HandViewModel<CA>.Visible));
            SetBinding(IsVisibleProperty, ThisBind);
        }
    }
}