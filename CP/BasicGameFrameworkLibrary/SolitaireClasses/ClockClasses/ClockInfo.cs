using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using Newtonsoft.Json;
using SkiaSharp;
namespace BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses
{
    public class ClockInfo : ObservableObject
    {
        private bool _isEnabled = true; //i think this is still fine (?)
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (SetProperty(ref _isEnabled, value)) { }
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
        private int _numberGuide;
        public int NumberGuide
        {
            get { return _numberGuide; }
            set
            {
                if (SetProperty(ref _numberGuide, value)) { }
            }
        }
        private int _leftGuide; //maybe it should have been integers.
        public int LeftGuide
        {
            get { return _leftGuide; }
            set
            {
                if (SetProperty(ref _leftGuide, value)) { }
            }
        }
    }
}