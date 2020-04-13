using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGamingUIXFLibrary.BasicControls.SimpleControls
{
    public abstract class BaseFrameXF : ContentView, IRepaintControl
    {
        protected SKCanvasView ThisDraw; //for now, don't enable touch. if we need it later, can put back in.
        protected FrameGraphics ThisFrame; //looks like i did not do notifications of properties for isenabled.  if needed, can still do.
        protected virtual void RepaintText() { }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(string), declaringType: typeof(BaseFrameXF), defaultValue: "", defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextPropertyChanged);
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
        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            if (thisItem.CanDrawText == false)
            {
                thisItem.RepaintText();
                return;
            }
            thisItem.ThisFrame.Text = (string)newValue;
        }
        public static readonly BindableProperty CanDrawTextProperty = BindableProperty.Create(propertyName: "CanDrawText", returnType: typeof(bool), declaringType: typeof(BaseFrameXF), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanDrawTextPropertyChanged);
        public bool CanDrawText
        {
            get
            {
                return (bool)GetValue(CanDrawTextProperty);
            }
            set
            {
                SetValue(CanDrawTextProperty, value);
            }
        }
        private static void CanDrawTextPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty IsBoldProperty = BindableProperty.Create(propertyName: "IsBold", returnType: typeof(bool), declaringType: typeof(BaseFrameXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsBoldPropertyChanged);
        public bool IsBold
        {
            get
            {
                return (bool)GetValue(IsBoldProperty);
            }
            set
            {
                SetValue(IsBoldProperty, value);
            }
        }
        private static void IsBoldPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.IsBold = (bool)newValue;
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(propertyName: "TextColor", returnType: typeof(string), declaringType: typeof(BaseFrameXF), defaultValue: cs.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextColorPropertyChanged);
        public string TextColor
        {
            get
            {
                return (string)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }
        private static void TextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.TextColor = (string)newValue;
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(propertyName: "BorderColor", returnType: typeof(string), declaringType: typeof(BaseFrameXF), defaultValue: cs.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: BorderColorPropertyChanged);
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
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.BorderColor = (string)newValue;
        }
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(propertyName: "BorderWidth", defaultValue: 2.0, returnType: typeof(double), declaringType: typeof(BaseFrameXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: BorderWidthPropertyChanged);
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
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.BorderWidth = (float)newValue; //taking a risk here.
        }
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(propertyName: "FontSize", returnType: typeof(double), defaultValue: 11.0, declaringType: typeof(BaseFrameXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: FontSizePropertyChanged);
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }
        private static void FontSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.FontSize = (float)newValue; //risk here too.
        }
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(propertyName: "FontFamily", defaultValue: MiscHelpers.DefaultFont, returnType: typeof(string), declaringType: typeof(BaseFrameXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: FontFamilyPropertyChanged);
        public string FontFamily
        {
            get
            {
                return (string)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }
        private static void FontFamilyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.FontFamily = (string)newValue;
        }
        public static new readonly BindableProperty IsEnabledProperty = BindableProperty.Create(propertyName: "IsEnabled", defaultValue: true, returnType: typeof(bool), declaringType: typeof(BaseFrameXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsEnabledPropertyChanged);
        public new bool IsEnabled
        {
            get
            {
                return (bool)GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }
        private static void IsEnabledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseFrameXF)bindable;
            thisItem.ThisFrame.IsEnabled = (bool)newValue;
        }
        public BaseFrameXF()
        {
            ThisDraw = new SKCanvasView();
            ThisDraw.PaintSurface += ThisDraw_PaintSurface;
            SizeChanged += BaseFrameXF_SizeChanged;
            ThisFrame = new FrameGraphics(this);
            HorizontalOptions = LayoutOptions.Start;
            FirstSetUp();
        }
        protected virtual void FirstSetUp() { }
        private void ThisDraw_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SetPaintDimensions(e.Info.Width, e.Info.Height);
            ThisFrame.DrawFrame(e.Surface.Canvas);
        }
        private void BaseFrameXF_SizeChanged(object sender, EventArgs e) { ThisDraw.InvalidateSurface(); }
        void IRepaintControl.DoInvalidate()
        {
            ThisDraw.InvalidateSurface();
        }
        protected virtual void SetPaintDimensions(float width, float height)
        {
            ThisFrame.ActualHeight = height;
            ThisFrame.ActualWidth = width;
        }
        protected virtual bool UseLess => false;
        protected void SetUpMarginsOnParentControl(View thisControl)
        {
            var thisRect = ThisFrame.GetControlArea();
            if (UseLess == false)
                thisControl.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 5, 3, 3); // 3 was too much.
            else
                thisControl.Margin = new Thickness(thisRect.Left, thisRect.Top - 3, 2, 2);// try this
        }
        protected Thickness GetControlArea()
        {
            var thisRect = ThisFrame.GetControlArea();
            return new Thickness(thisRect.Left - 2, thisRect.Top - 6, 2, 2);
        }
        protected SKPoint GetStartLocation()
        {
            return new SKPoint(0, ThisFrame.ActualHeight * -1); // i think
        }
        protected SKPoint GetBottomLocation()
        {
            return new SKPoint(0, ThisFrame.ActualHeight); // i think
        }
    }
}