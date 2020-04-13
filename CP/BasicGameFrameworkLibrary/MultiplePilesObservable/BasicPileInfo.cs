using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFrameworkLibrary.MultiplePilesObservable
{
    public class BasicPileInfo<D> : ObservableObject where D : IDeckObject, new()
    {
        public BasicPileInfo() { }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (SetProperty(ref _isSelected, value) == true) { }
            }
        }
        private bool _isEnabled = true; // defaults to true
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (SetProperty(ref _isEnabled, value) == true) { }
            }
        }
        private string _text = ""; //trying blank here.
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (SetProperty(ref _text, value) == true) { }
            }
        }
        private DeckObservableDict<D> _objectList = new DeckObservableDict<D>();
        public DeckObservableDict<D> ObjectList
        {
            get { return _objectList; }
            set
            {
                if (SetProperty(ref _objectList, value)) { }
            }
        }
        public DeckRegularDict<D> TempList = new DeckRegularDict<D>(); // i think this is needed.  that will speed up performance as well.  otherwise, what will happen is it will notify change for every step (wrong)
        private RotateExtensions.EnumRotateCategory _angle = RotateExtensions.EnumRotateCategory.None;
        public RotateExtensions.EnumRotateCategory Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                if (SetProperty(ref _angle, value) == true) { }
            }
        }
        private D _thisObject = new D();
        public D ThisObject
        {
            get { return _thisObject; }
            set
            {
                if (SetProperty(ref _thisObject, value)) { }
            }
        }
        private bool _visible = true; // defaults to true
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (SetProperty(ref _visible, value) == true) { }

            }
        }
        private int _column; // because of how its used, i may have to just subtract one when using views with it.
        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                if (SetProperty(ref _column, value) == true) { }
            }
        }
        private int _row;
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                if (SetProperty(ref _row, value) == true) { }
            }
        }
    }
}
