using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.GameGraphicsCP.Dominos;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BasicGamingUIXFLibrary.GameGraphics.Dominos
{
    public class DominosXF<D> : BaseDeckGraphicsXF<D, DominosCP>
        where D : IDominoInfo, new()
    {
        public static readonly BindableProperty CurrentFirstProperty = BindableProperty.Create(propertyName: "CurrentFirst", returnType: typeof(int), declaringType: typeof(DominosXF<D>), defaultValue: -1, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CurrentFirstPropertyChanged);
        public int CurrentFirst
        {
            get
            {
                return (int)GetValue(CurrentFirstProperty);
            }
            set
            {
                SetValue(CurrentFirstProperty, value);
            }
        }
        private static void CurrentFirstPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DominosXF<D>)bindable;
            thisItem.MainObject!.CurrentFirst = (int)newValue;
        }
        public static readonly BindableProperty CurrentSecondProperty = BindableProperty.Create(propertyName: "CurrentSecond", returnType: typeof(int), declaringType: typeof(DominosXF<D>), defaultValue: -1, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CurrentSecondPropertyChanged);
        public int CurrentSecond
        {
            get
            {
                return (int)GetValue(CurrentSecondProperty);
            }
            set
            {
                SetValue(CurrentSecondProperty, value);
            }
        }
        private static void CurrentSecondPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DominosXF<D>)bindable;
            thisItem.MainObject!.CurrentSecond = (int)newValue;
        }
        public static readonly BindableProperty HighestDominoProperty = BindableProperty.Create(propertyName: "HighestDomino", returnType: typeof(int), declaringType: typeof(DominosXF<D>), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HighestDominoPropertyChanged);
        public int HighestDomino
        {
            get
            {
                return (int)GetValue(HighestDominoProperty);
            }
            set
            {
                SetValue(HighestDominoProperty, value);
            }
        }
        private static void HighestDominoPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DominosXF<D>)bindable;
            thisItem.MainObject!.HighestDomino = (int)newValue;
        }
        protected override void PopulateInitObject()
        {
            if (MainObject == null)
                throw new BasicBlankException("Needs to create main object before you can init.  Rethink");
            MainObject.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CurrentFirstProperty, new Binding(nameof(IDominoInfo.CurrentFirst)));
            SetBinding(CurrentSecondProperty, new Binding(nameof(IDominoInfo.CurrentSecond)));
            SetBinding(HighestDominoProperty, new Binding(nameof(IDominoInfo.HighestDomino)));
        }
    }
}
