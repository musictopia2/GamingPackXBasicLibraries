using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.Misc
{
    public class TempRummySetsXF<SU, CO, RU, GC, GW> : ContentView
            where SU : Enum
            where CO : Enum
            where RU : IRummmyObject<SU, CO>, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<RU, GC>, new()
    {
        public double Divider { get; set; } = 1; // this is ui issue.  don't need bindings for this i think
        public double Additionals { get; set; } = 0;
        CustomBasicList<BaseHandXF<RU, GC, GW>>? _handList;
        public void Init(TempSetsViewModel<SU, CO, RU> thisMod, string tagUsed)
        {
            StackLayout thisStack = new StackLayout();
            thisStack.Orientation = StackOrientation.Horizontal;
            BaseHandXF<RU, GC, GW> thisHand;
            _handList = new CustomBasicList<BaseHandXF<RU, GC, GW>>();
            foreach (var thisSet in thisMod.SetList)
            {
                thisHand = new BaseHandXF<RU, GC, GW>();
                thisHand.ExtraControlSpace = 40;
                thisHand.HandType = HandViewModel<RU>.EnumHandList.Vertical;
                thisHand.Divider = Divider;
                thisHand.Additionals = Additionals;
                thisHand.LoadList(thisSet, tagUsed);
                _handList.Add(thisHand);
                thisHand.VerticalOptions = LayoutOptions.Fill; //try this way.
                thisStack.Children.Add(thisHand);
            }
            Content = thisStack;
        }
        public void Update(TempSetsViewModel<SU, CO, RU> thisMod)
        {
            if (thisMod.SetList.Count != _handList!.Count!)
                throw new BasicBlankException("Does not reconcile when updating temp sets");
            int x = 0;
            foreach (var ThisSet in thisMod.SetList)
            {
                var ThisHand = _handList[x];
                ThisHand.UpdateList(ThisSet);
                x++;
            }
        }
    }
}