using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Windows.Controls;
namespace BasicGamingUIWPFLibrary.BasicControls.Misc
{
    public class TempRummySetsWPF<SU, CO, RU, GC, GW> : UserControl
            where SU : Enum
            where CO : Enum
            where RU : IRummmyObject<SU, CO>, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<RU, GC>, new()
    {
        public double Divider { get; set; } = 1; // this is ui issue.  don't need bindings for this i think
        public double Additionals { get; set; } = 0;
        CustomBasicList<BaseHandWPF<RU, GC, GW>>? _handList;
        public void Init(TempSetsObservable<SU, CO, RU> thisMod, string tagUsed)
        {
            StackPanel thisStack = new StackPanel();
            thisStack.Orientation = Orientation.Horizontal;
            BaseHandWPF<RU, GC, GW> thisHand;
            _handList = new CustomBasicList<BaseHandWPF<RU, GC, GW>>();
            foreach (var ThisSet in thisMod.SetList)
            {
                thisHand = new BaseHandWPF<RU, GC, GW>();
                thisHand.ExtraControlSpace = 20;
                thisHand.HandType = HandObservable<RU>.EnumHandList.Vertical;
                thisHand.MaximumWidthHeight = (float)Height - 20;
                thisHand.Divider = Divider;
                thisHand.Additionals = Additionals;
                thisHand.LoadList(ThisSet, tagUsed);
                _handList.Add(thisHand);
                thisStack.Children.Add(thisHand);
            }
            Content = thisStack;
        }
        public void Update(TempSetsObservable<SU, CO, RU> thisMod)
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
