using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.Dice;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace BaseGPXWindowsAndControlsCore.BasicControls.Misc
{
    public class DiceListControlWPF<D> : UserControl where D : IStandardDice, new()
    {
        private DiceCup<D>? _thisCup;
        private DiceList<D>? _diceList;
        private StackPanel? _thisStack;
        public static readonly DependencyProperty CanShowDiceProperty = DependencyProperty.Register("CanShowDice", typeof(bool), typeof(DiceListControlWPF<D>), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CanShowDicePropertyChanged)));
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
        private static void CanShowDicePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DiceListControlWPF<D>)sender;
            thisItem.VisibleProcesses();
        }
        public static readonly DependencyProperty ViewModelVisibleProperty = DependencyProperty.Register("ViewModelVisible", typeof(bool), typeof(DiceListControlWPF<D>), new FrameworkPropertyMetadata(new PropertyChangedCallback(ViewModelVisiblePropertyChanged)));
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
        private static void ViewModelVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DiceListControlWPF<D>)sender;
            thisItem.VisibleProcesses();
        }
        private void VisibleProcesses()
        {
            if (ViewModelVisible == false)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            if (CanShowDice == false)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            Visibility = Visibility.Visible;
        }
        private StandardDiceWPF? FindControl(D thisDice)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var Deck = (StandardDiceWPF)thisCon!;
                if (Deck.DataContext.Equals(thisDice) == true)
                    return Deck;
            }
            return null;
        }
        private void DiceBindings(StandardDiceWPF thisGraphics, D thisDice) // needs the dice for the data context
        {
            thisGraphics.DataContext = thisDice;
            thisGraphics.CommandParameter = thisDice;
            var thisBind = GetCommandBinding(nameof(DiceCup<D>.DiceCommand));
            thisGraphics.SetBinding(StandardDiceWPF.CommandProperty, thisBind);
        }
        private void RefreshItems()
        {
            CustomBasicList<StandardDiceWPF> tempList = new CustomBasicList<StandardDiceWPF>();
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
                StandardDiceWPF thisGraphics = new StandardDiceWPF(); // this does the bindings already as well
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
            DataContext = null;
            DataContext = cup;
            _diceList!.CollectionChanged -= DiceList_CollectionChanged;
            _diceList = _thisCup.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged;
            _thisStack!.Children.Clear();
            PopulateList();
        }
        public void LoadDiceViewModel(DiceCup<D> cup)
        {
            _thisCup = cup;
            DataContext = _thisCup;
            _diceList = _thisCup.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged; //i think
            SetBinding(IsEnabledProperty, new Binding(nameof(DiceCup<D>.IsEnabled)));
            SetBinding(CanShowDiceProperty, new Binding(nameof(DiceCup<D>.CanShowDice)));
            SetBinding(ViewModelVisibleProperty, new Binding(nameof(DiceCup<D>.Visible)));
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal; // will always be horizontal for this one.
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
                    StandardDiceWPF thisD = new StandardDiceWPF();
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
                        thisCon!.DataContext = newDice;
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