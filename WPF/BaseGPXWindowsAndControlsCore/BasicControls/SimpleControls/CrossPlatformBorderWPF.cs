using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Controls;
namespace BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls
{
    //was used for yahtzee and kismet  could be used for others too.
    public class CrossPlatformBorderWPF : UserControl, IRepaintControl
    {
        private readonly SKElement _thisDraw;
        private readonly BorderGraphics _thisBorder;
        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(string), typeof(CrossPlatformBorderWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(BorderColorPropertyChanged)));
        public string BorderColor
        {
            get
            {
                return (string)GetValue(BorderColorProperty);
            }
            set
            {
                SetValue(BorderColorProperty, value);
            }
        }
        private static void BorderColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CrossPlatformBorderWPF)sender;
            thisItem._thisBorder.BorderColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(float), typeof(CrossPlatformBorderWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(BorderWidthPropertyChanged)));
        public float BorderWidth
        {
            get
            {
                return (float)GetValue(BorderWidthProperty);
            }
            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }
        private static void BorderWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CrossPlatformBorderWPF)sender;
            thisItem._thisBorder.BorderWidth = (float)e.NewValue;
        }
        public CrossPlatformBorderWPF()
        {
            _thisDraw = new SKElement();
            _thisBorder = new BorderGraphics(this); //maybe this was correct.
            Content = _thisDraw; // try this.
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _thisBorder.ActualHeight = e.Info.Height;
            _thisBorder.ActualWidth = e.Info.Width;
            _thisBorder.DrawBorder(e.Surface.Canvas);
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateVisual();
        }
    }
}