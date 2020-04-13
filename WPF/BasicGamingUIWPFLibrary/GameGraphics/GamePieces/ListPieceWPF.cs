using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using SkiaSharp;
using System.Windows;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.GameGraphics.GamePieces
{
    public class ListPieceWPF : BaseGraphicsWPF<ListViewPieceCP>
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ListPieceWPF), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        private static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (ListPieceWPF)sender;
            thisItem.Mains.DisplayText = (string)e.NewValue;
        }
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(ListPieceWPF), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(IndexPropertyChanged)));
        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (ListPieceWPF)sender;
            thisItem.Mains.Index = (int)e.NewValue;
        }
        public static readonly DependencyProperty CanHighlightProperty = DependencyProperty.Register("CanHighlight", typeof(bool), typeof(ListPieceWPF), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CanHighlightPropertyChanged)));
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
            var thisItem = (ListPieceWPF)sender;
            thisItem.Mains.CanHighlight = (bool)e.NewValue;
        }
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(SKColor), typeof(ListPieceWPF), new FrameworkPropertyMetadata(SKColors.Navy, new PropertyChangedCallback(TextColorPropertyChanged)));
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
            var thisItem = (ListPieceWPF)sender;
            thisItem.Mains.TextColor = (SKColor)e.NewValue;
        }
        public void UseSmallerFont()
        {
            Mains.SmallerFontSize = true;
        }
    }
}
