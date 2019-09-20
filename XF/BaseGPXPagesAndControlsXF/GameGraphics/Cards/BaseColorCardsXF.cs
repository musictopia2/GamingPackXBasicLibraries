using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.Cards;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.GameGraphics.Cards
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
                return (string) GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseColorCardsXF<CA, G>)bindable;
            thisItem.MainObject!.Value = (string) newValue;
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColorTypes), declaringType: typeof(BaseColorCardsXF<CA, G>), defaultValue: EnumColorTypes.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColorTypes Color
        {
            get
            {
                return (EnumColorTypes) GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseColorCardsXF<CA, G>)bindable;
            thisItem.MainObject!.Color = (EnumColorTypes) newValue;
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