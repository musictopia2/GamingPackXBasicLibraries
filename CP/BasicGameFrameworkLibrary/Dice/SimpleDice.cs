using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFrameworkLibrary.Dice
{
    public class SimpleDice : ObservableObject, IStandardDice, IGenerateDice<int>, ISimpleValueObject<int>
    {
        public int HeightWidth { get; } = 60; //for now does not matter.
        public string DotColor { get; set; } = cs.Black; //you have to make it public.  otherwise, you can't save the color which is needed for games like kismet.
        public string FillColor { get; set; } = cs.White;
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
        public EnumStyle Style { get; } = EnumStyle.Regular;
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
        CustomBasicList<int> IGenerateDice<int>.GetPossibleList => GetIntegerList(1, 6); //this simple.
        int ISimpleValueObject<int>.ReadMainValue => Value;
        public virtual void Populate(int Chosen)
        {
            Value = Chosen;
        }
    }
}