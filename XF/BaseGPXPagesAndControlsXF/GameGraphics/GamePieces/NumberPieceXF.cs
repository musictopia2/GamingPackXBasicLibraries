using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.GameGraphics.GamePieces
{
    public class NumberPieceXF : BaseGraphicsXF<NumberPieceCP>
    {
        public static readonly BindableProperty CardValueProperty = BindableProperty.Create(propertyName: "CardValue", returnType: typeof(EnumCardValueList), declaringType: typeof(NumberPieceXF), defaultValue: EnumCardValueList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardValuePropertyChanged);
        public EnumCardValueList CardValue
        {
            get
            {
                return (EnumCardValueList)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (NumberPieceXF)bindable;
            thisItem.Mains.CardValue = (EnumCardValueList)newValue;
        }
        public static readonly BindableProperty NumberValueProperty = BindableProperty.Create(propertyName: "NumberValue", returnType: typeof(int), declaringType: typeof(NumberPieceXF), defaultValue: -1, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NumberValuePropertyChanged);
        public int NumberValue
        {
            get
            {
                return (int)GetValue(NumberValueProperty);
            }
            set
            {
                SetValue(NumberValueProperty, value);
            }
        }
        private static void NumberValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (NumberPieceXF)bindable;
            thisItem.Mains.NumberValue = (int)newValue;
        }
        public static readonly BindableProperty CanHighlightProperty = BindableProperty.Create(propertyName: "CanHighlight", returnType: typeof(bool), declaringType: typeof(NumberPieceXF), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanHighlightPropertyChanged);
        public bool CanHighlight
        {
            get
            {
                return (bool)GetValue(CanHighlightProperty);
            }
            set
            {
                SetValue(CanHighlightProperty, value);
            }
        }
        private static void CanHighlightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (NumberPieceXF)bindable;
            thisItem.Mains.CanHighlight = (bool)newValue;
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(propertyName: "TextColor", returnType: typeof(SKColor), declaringType: typeof(NumberPieceXF), defaultValue: SKColors.Navy, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextColorPropertyChanged);
        public SKColor TextColor
        {
            get
            {
                return (SKColor)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }
        private static void TextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (NumberPieceXF)bindable;
            thisItem.Mains.TextColor = (SKColor)newValue;
        }
    }
}