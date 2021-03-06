﻿using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.GameFrames
{
    public class MainRummySetsWPF<SU, CO, RU, GC, GW, SE, T> : BaseFrameWPF
            where SU : Enum
            where CO : Enum
            where RU : IRummmyObject<SU, CO>, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<RU, GC>, new()
            where SE : SetInfo<SU, CO, RU, T> //the only catch about doing generics is even rummysetwpf needs it now.
    {

        private CustomBasicCollection<SE>? _setList;
        public double Divider { get; set; } = 1; // this is ui issue.  don't need bindings for this i think
        public double Additionals { get; set; } = 0;
        private StackPanel? _thisStack;
        private string _tagUsed = "";
        public void Init(MainSetsObservable<SU, CO, RU, SE, T> thisMod, string tagUsed)
        {
            _tagUsed = tagUsed;
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T> thisTemp;
            Grid firstGrid = new Grid();
            ScrollViewer thisScroll = new ScrollViewer();
            thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (thisMod.HasFrame == true)
            {
                Text = thisMod.Text;
                var thisRect = ThisFrame.GetControlArea();
                thisScroll.Margin = new Thickness((double)thisRect.Left + (float)3, (double)thisRect.Top + (float)3, 3, 5); // try this way.
                firstGrid.Children.Add(ThisDraw);
                firstGrid.Children.Add(thisScroll);
            }
            else
            {
                firstGrid.Children.Add(thisScroll);// this alone.
            }
            _setList = thisMod.SetList;
            foreach (var thisSet in thisMod.SetList)
            {
                thisTemp = new IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T>();
                thisTemp.Divider = Divider;
                thisTemp.Additionals = Additionals;
                thisTemp.LoadList(thisSet, tagUsed);
                _thisStack.Children.Add(thisTemp);
            }
            _setList.CollectionChanged += SetList_CollectionChanged;
            thisScroll.Content = _thisStack;
            Content = firstGrid;
        }
        public void Update(MainSetsObservable<SU, CO, RU, SE, T> thisMod)
        {
            _thisStack!.Children.Clear();
            _setList!.CollectionChanged -= SetList_CollectionChanged;
            _setList = thisMod.SetList;
            _setList.CollectionChanged += SetList_CollectionChanged;
            foreach (var thisSet in thisMod.SetList)
            {
                var thisTemp = new IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T>();
                thisTemp.Divider = Divider;
                thisTemp.Additionals = Additionals;
                thisTemp.LoadList(thisSet, _tagUsed);
                _thisStack.Children.Add(thisTemp);
            }
        }
        private void SetList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T> thisTemp;
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count != 1)
                    throw new BasicBlankException("Cannot remove one item at a time");
                _thisStack!.Children.RemoveAt(e.OldStartingIndex);
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
                _thisStack!.Children.Clear(); //this is it.
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var thisItem in e.NewItems)
                {
                    var thisSet = (SE)thisItem!;
                    thisTemp = new IndividualRummySetWPF<SU, CO, RU, GC, GW, SE, T>();
                    thisTemp.Divider = Divider;
                    thisTemp.Additionals = Additionals;
                    thisTemp.LoadList(thisSet, _tagUsed);
                    _thisStack!.Children.Add(thisTemp);
                }
            }
        }
    }
}
