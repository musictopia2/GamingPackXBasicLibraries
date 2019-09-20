using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.Dice;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Specialized;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.Misc
{
    public class DiceListControlXF<D> : ContentView
        where D : IStandardDice, new()
    {
        private DiceCup<D>? _thisCup;
        private DiceList<D>? _diceList;
        private StackLayout? _thisStack;
        public static readonly BindableProperty CanShowDiceProperty = BindableProperty.Create(propertyName: "CanShowDice", returnType: typeof(bool), declaringType: typeof(DiceListControlXF<D>), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanShowDicePropertyChanged);
        public bool CanShowDice
        {
            get
            {
                return (bool)GetValue(CanShowDiceProperty);
            }
            set
            {
                SetValue(CanShowDiceProperty, value);
            }
        }
        private static void CanShowDicePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DiceListControlXF<D>)bindable;
            thisItem.VisibleProcesses();
        }
        public static readonly BindableProperty ViewModelVisibleProperty = BindableProperty.Create(propertyName: "ViewModelVisible", returnType: typeof(bool), declaringType: typeof(DiceListControlXF<D>), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ViewModelVisiblePropertyChanged);
        public bool ViewModelVisible
        {
            get
            {
                return (bool)GetValue(ViewModelVisibleProperty);
            }
            set
            {
                SetValue(ViewModelVisibleProperty, value);
            }
        }
        private static void ViewModelVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DiceListControlXF<D>)bindable;
            thisItem.VisibleProcesses();
        }
        private void VisibleProcesses()
        {
            if (ViewModelVisible == false)
            {
                IsVisible = false; // for now
                return;
            }
            if (CanShowDice == false)
            {
                IsVisible = false;
                return;
            }
            IsVisible = true;
        }
        private StandardDiceXF? FindControl(D thisDice)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var Deck = (StandardDiceXF)thisCon!;
                if (Deck.BindingContext.Equals(thisDice) == true)
                    return Deck;
            }
            return null;
        }
        private void DiceBindings(StandardDiceXF thisGraphics, D thisDice) // needs the dice for the data context
        {
            thisGraphics.BindingContext = thisDice;
            thisGraphics.CommandParameter = thisDice;
            var thisBind = GetCommandBinding(nameof(DiceCup<D>.DiceCommand));
            thisGraphics.SetBinding(StandardDiceXF.CommandProperty, thisBind); //hopefully i don't need extra on this one for sizes (?)
        }
        private void RefreshItems()
        {
            CustomBasicList<StandardDiceXF> tempList = new CustomBasicList<StandardDiceXF>();
            foreach (var thisDice in _diceList!)
            {
                var thisD = FindControl(thisDice);
                tempList.Add(thisD!);
            }
            _thisStack!.Children.Clear();
            foreach (var ThisD in tempList)
                _thisStack.Children.Add(ThisD);
        }
        private void PopulateList()
        {
            foreach (var firstDice in _diceList!)
            {
                StandardDiceXF thisGraphics = new StandardDiceXF(); // this does the bindings already as well
                thisGraphics.SendDiceInfo(firstDice); //this has to be done too.
                DiceBindings(thisGraphics, firstDice);
                _thisStack!.Children.Add(thisGraphics);
            }
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisCup;
            return thisBind;
        }
        public void UpdateDice(DiceCup<D> cup)
        {
            _thisCup = cup;
            BindingContext = null;
            BindingContext = cup;
            _diceList!.CollectionChanged -= DiceList_CollectionChanged;
            _diceList = _thisCup.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged;
            _thisStack!.Children.Clear();
            PopulateList();
        }
        public void LoadDiceViewModel(DiceCup<D> cup)
        {
            _thisCup = cup;
            BindingContext = _thisCup;
            _diceList = _thisCup.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged; //i think
            SetBinding(IsEnabledProperty, new Binding(nameof(DiceCup<D>.IsEnabled)));
            SetBinding(CanShowDiceProperty, new Binding(nameof(DiceCup<D>.CanShowDice)));
            SetBinding(ViewModelVisibleProperty, new Binding(nameof(DiceCup<D>.Visible)));
            _thisStack = new StackLayout();
            _thisStack.Spacing = 0;
            _thisStack.Orientation = StackOrientation.Horizontal; // will always be horizontal for this one.
            PopulateList();
            Content = _thisStack;
        }
        private void DiceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var newDice = (D)thisItem!;
                    StandardDiceXF thisD = new StandardDiceXF();
                    DiceBindings(thisD, newDice); // well see what we need
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
                        var oldDice = (D)e.OldItems[x - 1]!;
                        var newDice = (D)e.NewItems[x - 1]!;
                        var thisCon = FindControl(oldDice);
                        thisCon!.BindingContext = newDice;
                    }
                }
                else
                    throw new Exception("Not sure when the numbers don't match");
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var oldDice = (D)thisItem!;
                    var thisCon = FindControl(oldDice);
                    _thisStack!.Children.Remove(thisCon); // because not there anymore.
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // needs to clear and do nothing else because no dice left
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
        }
    }
}