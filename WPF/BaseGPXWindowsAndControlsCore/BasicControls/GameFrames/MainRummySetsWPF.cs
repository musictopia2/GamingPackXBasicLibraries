using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameFrames
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
        public void Init(MainSetsViewModel<SU, CO, RU, SE, T> thisMod, string tagUsed)
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
        public void Update(MainSetsViewModel<SU, CO, RU, SE, T> thisMod)
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
                throw new BasicBlankException("Needs to figure out when removing an item");
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