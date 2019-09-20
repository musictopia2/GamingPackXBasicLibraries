using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
using System.Windows;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces
{
    public class NumberPieceWPF : BaseGraphicsWPF<NumberPieceCP>
    {
        public static readonly DependencyProperty CardValueProperty = DependencyProperty.Register("CardValue", typeof(EnumCardValueList), typeof(NumberPieceWPF), new FrameworkPropertyMetadata(EnumCardValueList.None, new PropertyChangedCallback(CardValuePropertyChanged)));
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
        private static void CardValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (NumberPieceWPF)sender;
            thisItem.Mains.CardValue = (EnumCardValueList)e.NewValue;
        }
        public static readonly DependencyProperty NumberValueProperty = DependencyProperty.Register("NumberValue", typeof(int), typeof(NumberPieceWPF), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(NumberValuePropertyChanged)));
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
        private static void NumberValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (NumberPieceWPF)sender;
            thisItem.Mains.NumberValue = (int)e.NewValue;
        }
        public static readonly DependencyProperty CanHighlightProperty = DependencyProperty.Register("CanHighlight", typeof(bool), typeof(NumberPieceWPF), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CanHighlightPropertyChanged)));
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
        private static void CanHighlightPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (NumberPieceWPF)sender;
            thisItem.Mains.CanHighlight = (bool)e.NewValue;
        }
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(SKColor), typeof(NumberPieceWPF), new FrameworkPropertyMetadata(SKColors.Navy, new PropertyChangedCallback(TextColorPropertyChanged)));
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

        private static void TextColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (NumberPieceWPF)sender;
            thisItem.Mains.TextColor = (SKColor)e.NewValue;
        }
    }
}