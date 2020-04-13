using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using Newtonsoft.Json;

namespace BasicGameFrameworkLibrary.Dice
{
    public abstract class BaseSpecialStyleDice : ObservableObject, IStandardDice, IGenerateDice<int>, ISelectableObject
    {
        public int HeightWidth { get; } = 60; //for now does not matter.
        public EnumStyle Style { get; } = EnumStyle.DrawWhiteNumber;
        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public int Index { get; set; }
        private bool _Hold;
        public bool Hold
        {
            get { return _Hold; }
            set
            {
                if (SetProperty(ref _Hold, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (SetProperty(ref _IsEnabled, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        [JsonIgnore]
        public abstract CustomBasicList<int> GetPossibleList { get; }
        public abstract string DotColor { get; set; } //has to be public all the way.  otherwise, autoresume does not work.
        public abstract string FillColor { get; set; }
        public virtual void Populate(int chosen)
        {
            Value = chosen;
        }
    }
}