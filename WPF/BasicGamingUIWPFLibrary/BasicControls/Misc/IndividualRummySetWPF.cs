﻿using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace BasicGamingUIWPFLibrary.BasicControls.Misc
{
    public class IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T> : UserControl
            where SU : Enum
            where CO : Enum
            where RU : IRummmyObject<SU, CO>, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<RU, GC>, new()
            where SE : SetInfo<SU, CO, RU, T> //the only catch about doing generics is even rummysetwpf needs it now.
    {
        public double Divider { get; set; }
        public double Additionals { get; set; } // i don't think bindings are needed here
        private DeckObservableDict<RU>? _objectList; //not necessarily cards.
        private SE? _thisMod;
        private StackPanel? _thisStack;
        private string _tagUsed = "";
        private GW? FindControl(RU thisCard)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.DataContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        private void CalculateMargins(GW thisDeck) //hopefully still okay (?)
        {
            double adds;
            adds = thisDeck.ObjectSize.Height / Divider;
            adds = thisDeck.ObjectSize.Height - adds;
            adds += Additionals; // try this
            adds *= (float)-1;
            thisDeck.Margin = new Thickness(6, 0, 6, adds); // i think
        }
        private void RecalculatePositioning()
        {
            if (_objectList == null == true)
                return;
            foreach (var thisObject in _objectList!)
            {
                var thisDeck = FindControl(thisObject);
                if (thisDeck != null)
                    if (thisObject.Equals(_objectList.Last()) == false)
                        CalculateMargins(thisDeck);
                    else
                        thisDeck.Margin = new Thickness(6, 0, 6, 0);
            }
        }
        private void PopulateList()
        {
            foreach (var firstObject in _objectList!)
            {
                GW thisGraphics = new GW(); // this does the bindings already as well
                ObjectBindings(thisGraphics, firstObject);
                MinWidth = thisGraphics.ObjectSize.Width + 12; //try this way.
                if (firstObject.Equals(_objectList.Last()) == false)
                    CalculateMargins(thisGraphics);
                else
                    thisGraphics.Margin = new Thickness(6, 0, 6, 0);
                _thisStack!.Children.Add(thisGraphics);
            }
        }
        private void ObjectBindings(GW thisDeck, RU thisCard)
        {
            thisDeck.SendSize(_tagUsed, thisCard); //i think this is where its needed.
            var ThisBind = GetCommandBinding(nameof(HandObservable<RU>.ObjectSingleClickCommand));
            thisDeck.SetBinding(GraphicsCommand.CommandProperty, ThisBind);
            thisDeck.CommandParameter = thisCard;
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        public void RefreshItems()
        {
            CustomBasicList<GW> tempList = new CustomBasicList<GW>();
            foreach (var thisCard in _objectList!)
            {
                var thisD = FindControl(thisCard);
                tempList.Add(thisD!);
            }
            _thisStack!.Children.Clear();
            foreach (var thisD in tempList)
                _thisStack.Children.Add(thisD);
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T>), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandPropertyChanged)));
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
        private void BaseHand_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tempCommand = Command;
            if (tempCommand != null)
                if (tempCommand.CanExecute(null) == true)
                    tempCommand.Execute(null);
        }
        public void LoadList(SE thisSet, string tagUsed)
        {
            _tagUsed = tagUsed;
            _thisMod = thisSet;
            _thisStack = new StackPanel();
            Content = _thisStack;
            Command = thisSet.BoardSingleClickCommand;
            _objectList = thisSet.HandList;
            MouseUp += BaseHand_MouseUp;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            PopulateList();
        }
        public void UpdateList(SE thisSet)
        {
            _thisMod = thisSet;
            _objectList!.CollectionChanged -= ObjectList_CollectionChanged;
            _objectList = thisSet.HandList;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            _thisStack!.Children.Clear();
            PopulateList();
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                RU newCard;
                GW thisD;
                if (e.NewItems.Count == 1 && e.NewStartingIndex == 0)
                {
                    newCard = (RU)e.NewItems[0]!;
                    thisD = new GW();
                    ObjectBindings(thisD, newCard);
                    _thisStack!.Children.Insert(0, thisD);
                }
                else
                    foreach (var thisItem in e.NewItems)
                    {
                        newCard = (RU)thisItem!;
                        thisD = new GW();
                        ObjectBindings(thisD, newCard);
                        _thisStack!.Children.Add(thisD);
                    }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
                if (e.OldItems.Count == e.NewItems.Count)
                {
                    int x;
                    var loopTo = e.OldItems.Count;
                    for (x = 1; x <= loopTo; x++)
                    {
                        var oldCard = (RU)e.OldItems[x - 1]!;
                        var newCard = (RU)e.NewItems[x - 1]!;
                        var thisCon = FindControl(oldCard);
                        ObjectBindings(thisCon!, newCard);
                    }
                }
                else
                    throw new BasicBlankException("Not sure when the numbers don't match");
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (var thisItem in e.OldItems)
                {
                    var oldCard = (RU)thisItem!;
                    var thisCon = FindControl(oldCard);
                    _thisStack!.Children.Remove(thisCon); // because not there anymore.
                }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear();
                PopulateList();
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
                if (e.OldStartingIndex == e.NewStartingIndex)
                    RefreshItems();
                else
                {
                    var FirstCon = _thisStack!.Children[e.OldStartingIndex];
                    _ = _thisStack.Children[e.NewStartingIndex];
                    _thisStack.Children.Remove(FirstCon);
                    _thisStack.Children.Insert(e.NewStartingIndex, FirstCon);
                }
            RecalculatePositioning();
        }
    }
}
