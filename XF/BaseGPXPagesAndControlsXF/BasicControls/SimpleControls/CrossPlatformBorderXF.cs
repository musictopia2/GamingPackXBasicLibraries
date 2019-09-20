using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.Interfaces;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BaseGPXPagesAndControlsXF.BasicControls.SimpleControls
{
    public class CrossPlatformBorderXF : ContentView, IRepaintControl //was used for yahtzee and kismet  could be used for others too.
    {
        private readonly SKCanvasView _thisDraw;
        private readonly BorderGraphics _thisBorder;
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(propertyName: "BorderColor", returnType: typeof(string), declaringType: typeof(CrossPlatformBorderXF), defaultValue: cs.Black, defaultBindingMode: BindingMode.TwoWay, propertyChanged: BorderColorPropertyChanged);
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
        private static void BorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CrossPlatformBorderXF)bindable;
            thisItem._thisBorder.BorderColor = (string)newValue;
        }
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(propertyName: "BorderWidth", returnType: typeof(double), declaringType: typeof(CrossPlatformBorderXF), defaultValue: 2.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: BorderWidthPropertyChanged);
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }
            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }
        private static void BorderWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CrossPlatformBorderXF)bindable;
            thisItem._thisBorder.BorderWidth = (float)newValue; //hopefully this works too.  if this gets casting error, rethink.
        }
        void IRepaintControl.DoInvalidate()
        {
            _thisDraw.InvalidateSurface();
        }
        public CrossPlatformBorderXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += DrawPaint;
            _thisBorder = new BorderGraphics(this); //forgot
            Content = _thisDraw;
        }
        private void DrawPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            _thisBorder.ActualHeight = e.Info.Height;
            _thisBorder.ActualWidth = e.Info.Width;
            _thisBorder.DrawBorder(e.Surface.Canvas);
        }
    }
}