using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using Xamarin.Forms;
namespace BasicGamingUIXFLibrary.BasicControls.Misc
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
        public void Init(TempSetsObservable<SU, CO, RU> thisMod, string tagUsed)
        {
            StackLayout thisStack = new StackLayout();
            thisStack.Orientation = StackOrientation.Horizontal;
            BaseHandXF<RU, GC, GW> thisHand;
            _handList = new CustomBasicList<BaseHandXF<RU, GC, GW>>();
            foreach (var thisSet in thisMod.SetList)
            {
                thisHand = new BaseHandXF<RU, GC, GW>();
                thisHand.ExtraControlSpace = 40;
                thisHand.HandType = HandObservable<RU>.EnumHandList.Vertical;
                thisHand.Divider = Divider;
                thisHand.Additionals = Additionals;
                thisHand.LoadList(thisSet, tagUsed);
                _handList.Add(thisHand);
                thisHand.VerticalOptions = LayoutOptions.Fill; //try this way.
                thisStack.Children.Add(thisHand);
            }
            Content = thisStack;
        }
        public void Dispose()
        {
            _handList!.ForEach(x =>
            {
                x.Dispose();
            });
        }

    }
}