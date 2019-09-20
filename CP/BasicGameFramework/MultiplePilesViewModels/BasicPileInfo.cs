using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFramework.MultiplePilesViewModels
{
    public class BasicPileInfo<D> : ObservableObject where D : IDeckObject, new()
    {
        public BasicPileInfo() { }

        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (SetProperty(ref _IsSelected, value) == true) { }
            }
        }
        private bool _IsEnabled = true; // defaults to true
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (SetProperty(ref _IsEnabled, value) == true) { }
            }
        }
        private string _Text = ""; //trying blank here.
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (SetProperty(ref _Text, value) == true) { }
            }
        }
        private DeckObservableDict<D> _ObjectList = new DeckObservableDict<D>();
        public DeckObservableDict<D> ObjectList
        {
            get { return _ObjectList; }
            set
            {
                if (SetProperty(ref _ObjectList, value)) { }
            }
        }
        public DeckRegularDict<D> TempList = new DeckRegularDict<D>(); // i think this is needed.  that will speed up performance as well.  otherwise, what will happen is it will notify change for every step (wrong)
        private RotateExtensions.EnumRotateCategory _Angle = RotateExtensions.EnumRotateCategory.None;
        public RotateExtensions.EnumRotateCategory Angle
        {
            get
            {
                return _Angle;
            }
            set
            {
                if (SetProperty(ref _Angle, value) == true) { }
            }
        }
        private D _ThisObject = new D();
        public D ThisObject
        {
            get { return _ThisObject; }
            set
            {
                if (SetProperty(ref _ThisObject, value)) { }
            }
        }
        private bool _Visible = true; // defaults to true
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (SetProperty(ref _Visible, value) == true) { }

            }
        }
        private int _Column; // because of how its used, i may have to just subtract one when using views with it.
        public int Column
        {
            get
            {
                return _Column;
            }
            set
            {
                if (SetProperty(ref _Column, value) == true) { }
            }
        }
        private int _Row;
        public int Row
        {
            get
            {
                return _Row;
            }
            set
            {
                if (SetProperty(ref _Row, value) == true) { }
            }
        }
    }
}