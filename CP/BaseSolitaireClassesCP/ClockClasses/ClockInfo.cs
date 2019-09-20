using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.MVVMHelpers;
using Newtonsoft.Json;
using SkiaSharp;
namespace BaseSolitaireClassesCP.ClockClasses
{
    public class ClockInfo : ObservableObject
    {
        private bool _IsEnabled = true; //i think this is still fine (?)
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (SetProperty(ref _IsEnabled, value)) { }
            }
        }
        public DeckObservableDict<SolitaireCard> CardList { get; set; } = new DeckObservableDict<SolitaireCard>(); //maybe this one is okay.
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value)) { }
            }
        }
        [JsonIgnore] //i think needs to ignore.  hopefully that will work.
        public SKPoint Location { get; set; } //once set, will not change.
        private int _NumberGuide;
        public int NumberGuide
        {
            get { return _NumberGuide; }
            set
            {
                if (SetProperty(ref _NumberGuide, value)) { }
            }
        }
        private int _LeftGuide; //maybe it should have been integers.
        public int LeftGuide
        {
            get { return _LeftGuide; }
            set
            {
                if (SetProperty(ref _LeftGuide, value)) { }
            }
        }
    }
}