using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.GameFrames
{
    public class MainRummySetsXF<SU, CO, RU, GC, GW, SE, T> : BaseFrameXF
            where SU : Enum
            where CO : Enum
            where RU : IRummmyObject<SU, CO>, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<RU, GC>, new()
            where SE : SetInfo<SU, CO, RU, T> //the only catch about doing generics is even rummysetwpf needs it now.
    {
        private CustomBasicCollection<SE>? _setList;
        public double Divider { get; set; } = 1; // this is ui issue.  don't need bindings for this i think
        public double Additionals { get; set; } = 0;
        private StackLayout? _thisStack;
        private string _tagUsed = "";
        public void Init(MainSetsViewModel<SU, CO, RU, SE, T> thisMod, string tagUsed)
        {
            _tagUsed = tagUsed;
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            IndividualRummySetXF<SU, CO, RU, GC, GW, SE, T> thisTemp;
            Grid firstGrid = new Grid();
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Both;
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
                thisTemp = new IndividualRummySetXF<SU, CO, RU, GC, GW, SE, T>();
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
                var thisTemp = new IndividualRummySetXF<SU, CO, RU, GC, GW, SE, T>();
                thisTemp.Divider = Divider;
                thisTemp.Additionals = Additionals;
                thisTemp.LoadList(thisSet, _tagUsed);
                _thisStack.Children.Add(thisTemp);
            }
        }
        private void SetList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IndividualRummySetXF<SU, CO, RU, GC, GW, SE, T> thisTemp;
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
                    thisTemp = new IndividualRummySetXF<SU, CO, RU, GC, GW, SE, T>();
                    thisTemp.Divider = Divider;
                    thisTemp.Additionals = Additionals;
                    thisTemp.LoadList(thisSet, _tagUsed);
                    _thisStack!.Children.Add(thisTemp);
                }
            }
        }
    }
}