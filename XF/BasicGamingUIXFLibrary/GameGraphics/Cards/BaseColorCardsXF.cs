using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.Cards;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.GameGraphics.Cards
{
    public abstract class BaseColorCardsXF<CA, G> : BaseDeckGraphicsXF<CA, G>
        where CA : IColorCard, new()
        where G : BaseColorCardsCP, new()
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(string), declaringType: typeof(BaseColorCardsXF<CA, G>), defaultValue: "", defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseColorCardsXF<CA, G>)bindable;
            thisItem.MainObject!.Value = (string)newValue;
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColorTypes), declaringType: typeof(BaseColorCardsXF<CA, G>), defaultValue: EnumColorTypes.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColorTypes Color
        {
            get
            {
                return (EnumColorTypes)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseColorCardsXF<CA, G>)bindable;
            thisItem.MainObject!.Color = (EnumColorTypes)newValue;
        }
        protected override void DoCardBindings() // needs this too
        {
            SetBinding(ValueProperty, new Binding(nameof(IColorCard.Display)));
            SetBinding(ColorProperty, new Binding(nameof(IColorCard.Color)));
        }
        protected override void PopulateInitObject()
        {
            if (MainObject == null)
                throw new BasicBlankException("Needs to create main object before you can init.  Rethink");
            MainObject.Init();
        }
    }
}
