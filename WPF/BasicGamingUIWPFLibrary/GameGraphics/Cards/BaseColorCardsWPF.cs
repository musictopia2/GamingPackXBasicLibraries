using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.Cards;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
namespace BasicGamingUIWPFLibrary.GameGraphics.Cards
{
    public abstract class BaseColorCardsWPF<CA, G> : BaseDeckGraphicsWPF<CA, G>
        where CA : IColorCard, new()
        where G : BaseColorCardsCP, new()
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(BaseColorCardsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(ValuePropertyChanged)));
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
        private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseColorCardsWPF<CA, G>)sender;
            thisItem.MainObject!.Value = (string)e.NewValue;
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(EnumColorTypes), typeof(BaseColorCardsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));
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
        protected override void PopulateInitObject()
        {
            if (MainObject == null)
                throw new BasicBlankException("Needs to create main object before you can init.  Rethink");
            MainObject.Init();
        }
        private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseColorCardsWPF<CA, G>)sender;
            thisItem.MainObject!.Color = (EnumColorTypes)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ValueProperty, nameof(IColorCard.Display)); //decided to use string value.
            SetBinding(ColorProperty, nameof(IColorCard.Color));
        }
    }
}
