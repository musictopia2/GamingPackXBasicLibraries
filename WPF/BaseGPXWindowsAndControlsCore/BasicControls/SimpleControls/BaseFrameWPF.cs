using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Controls;
namespace BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls
{
    public abstract class BaseFrameWPF : UserControl, IRepaintControl
    {
        protected SKElement ThisDraw;
        protected FrameGraphics ThisFrame;
        public static readonly DependencyProperty CanDrawTextProperty = DependencyProperty.Register("CanDrawText", typeof(bool), typeof(BaseFrameWPF), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CanDrawTextPropertyChanged)));
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
        private static void CanDrawTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        protected virtual void RepaintText() { }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BaseFrameWPF), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
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
            var thisItem = (BaseFrameWPF)sender;
            if (e.NewValue == null == true || (string)e.NewValue! == ",")
            {
                thisItem.ThisFrame.Text = "";
                return;
            }
            if (thisItem.CanDrawText == false)
            {
                thisItem.RepaintText();
                return; // do this instead.
            }
            thisItem.ThisFrame.Text = (string)e.NewValue!;
        }
        public static readonly DependencyProperty IsBoldProperty = DependencyProperty.Register("IsBold", typeof(bool), typeof(BaseFrameWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsBoldPropertyChanged)));
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
        private static void IsBoldPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseFrameWPF)sender;
            thisItem.ThisFrame.IsBold = (bool)e.NewValue;
        }
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(string), typeof(BaseFrameWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextColorPropertyChanged)));
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
        private static void TextColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseFrameWPF)sender;
            thisItem.ThisFrame.TextColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(string), typeof(BaseFrameWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(BorderColorPropertyChanged)));
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
            var thisItem = (BaseFrameWPF)sender;
            thisItem.ThisFrame.BorderColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(float), typeof(BaseFrameWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(BorderWidthPropertyChanged)));
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
            var thisItem = (BaseFrameWPF)sender;
            thisItem.ThisFrame.BorderWidth = (float)e.NewValue;
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(BaseFrameWPF), new PropertyMetadata());
        public static void SetTopProperty(DependencyObject Item, double Value)
        {
            Item.SetValue(TopProperty, Value);
        }
        public static double GetTopProperty(DependencyObject Item)
        {
            return (double)Item.GetValue(TopProperty);
        }
        public static readonly DependencyProperty LeftPropertyProperty = DependencyProperty.Register("LeftProperty", typeof(double), typeof(BaseFrameWPF), new PropertyMetadata());
        public static void SetLeftProperty(DependencyObject Item, double Value)
        {
            Item.SetValue(LeftPropertyProperty, Value);
        }
        public static double GetLeftProperty(DependencyObject Item)
        {
            return (double)Item.GetValue(LeftPropertyProperty);
        }
        protected SKPoint GetStartLocation()
        {
            return new SKPoint(0, ThisFrame.ActualHeight * -1); // i think
        }
        protected SKPoint GetBottomLocation()
        {
            return new SKPoint(0, ThisFrame.ActualHeight); // i think
        }
        public BaseFrameWPF()
        {
            ThisDraw = new SKElement();
            ThisDraw.PaintSurface += ThisDraw_PaintSurface;
            SizeChanged += BaseFrameWPF_SizeChanged;
            ThisFrame = new FrameGraphics(this);
            Text = ","; // default to that which will translate to blank.  since there seemed to be a weird problem and not sure why.
            HorizontalAlignment = HorizontalAlignment.Left;
            FirstSetUp();
        }
        protected virtual void FirstSetUp() { }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            SetPaintDimensions(e.Info.Width, e.Info.Height);
            ThisFrame.DrawFrame(e.Surface.Canvas);
        }
        protected virtual void SetPaintDimensions(float width, float height) { }
        protected void SetUpMarginsOnParentControl(FrameworkElement thisControl, SKRect thisRect)
        {
            thisControl.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 3); // try this way.
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(BaseFrameWPF.IsEnabled))
                ThisFrame.IsEnabled = (bool)e.NewValue;// i think
            if (e.Property.Name == nameof(BaseFrameWPF.FontSize))
                ThisFrame.FontSize = (float)e.NewValue;
            if (e.Property.Name == nameof(BaseFrameWPF.FontFamily))
                ThisFrame.FontFamily = (string)e.NewValue;
            base.OnPropertyChanged(e);
        }
        private void BaseFrameWPF_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ThisFrame.ActualHeight = (float)e.NewSize.Height;
            ThisFrame.ActualWidth = (float)e.NewSize.Width;
        }
        protected void ChangeSizeBasedOnSizeNeeded(SKSize thisSize)
        {
            var newSize = ThisFrame.FrameSizeNeededForExpectedImage(thisSize);
            Height = newSize.Height;
            Width = newSize.Width;
        }
        protected void ChangedBasedOnWidth(float widthRequest)
        {
            var thiss = ThisFrame.WidthMargins();
            Width = thiss + widthRequest;
        }
        protected void ChangedBasedOnHeight(float heightRequest)
        {
            var thiss = ThisFrame.FrameForHeights(heightRequest);
            Height = thiss;
        }
        public void DoInvalidate()
        {
            ThisDraw.InvalidateVisual();
        }
    }
}