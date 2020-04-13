using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace BasicGamingUIWPFLibrary.GameGraphics.Base
{

    public abstract class BaseGraphicsWPF<G> : GraphicsCommand,
       ISelectableObject, IRepaintControl where G : BaseGraphicsCP, new()
    {
        protected G Mains;
        protected SKElement ThisDraw; //figure out the invoking of events.  they chose to use events.
        public void SendPiece(G thisPiece) // this is needed so it can support the color picker.  the color picker already uses the cp control
        {
            Mains = thisPiece;
            thisPiece.PaintUI = this; //try here too.
            Init(); // i think needs to initialize here too.
        }

        public static readonly DependencyProperty MainColorProperty = DependencyProperty.Register("MainColor", typeof(string), typeof(BaseGraphicsWPF<G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(MainColorPropertyChanged)));
        public string MainColor
        {
            get
            {
                return (string)GetValue(MainColorProperty);
            }
            set
            {
                SetValue(MainColorProperty, value);
            }
        }
        private static void MainColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseGraphicsWPF<G>)sender;
            thisItem.Mains.MainColor = e.NewValue.ToString()!; // hopefully will be that simple  this will trigger the paint event.  hoepfully this way will still work out.
        }


        public static readonly DependencyProperty DoubleCommandProperty = DependencyProperty.Register("DoubleCommand", typeof(ICommand), typeof(BaseGraphicsWPF<G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(DoubleCommandPropertyChanged)));
        public ICommand DoubleCommand
        {
            get
            {
                return (ICommand)GetValue(DoubleCommandProperty);
            }
            set
            {
                SetValue(DoubleCommandProperty, value);
            }
        }// if there is double clicking, would be here.
        private static void DoubleCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(BaseGraphicsWPF<G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsSelectedPropertyChanged)));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseGraphicsWPF<G>)sender;
            thisItem.Mains.IsSelected = (bool)e.NewValue;
            thisItem.ThisDraw.InvalidateVisual(); //just in case.  trying this.
        }
        public static readonly DependencyProperty NeedsHighLightingProperty = DependencyProperty.Register("NeedsHighLighting", typeof(bool), typeof(BaseGraphicsWPF<G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(NeedsHighLightingPropertyChanged)));
        public bool NeedsHighLighting
        {
            get
            {
                return (bool)GetValue(NeedsHighLightingProperty);
            }
            set
            {
                SetValue(NeedsHighLightingProperty, value);
            }
        }
        private static void NeedsHighLightingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseGraphicsWPF<G>)sender;
            thisItem.Mains.NeedsHighlighting = (bool)e.NewValue;
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(IsEnabled))
            {
                if (NeedsHighLighting == true)
                    Mains.IsEnabled = IsEnabled;// i think
            }
            base.OnPropertyChanged(e);
        }
        public virtual void Init() // the actual is not working at this moment until the property change happens
        {
            Mains.ActualWidth = Width - Margin.Left - Margin.Right;
            Mains.ActualHeight = Height - Margin.Bottom - Margin.Top;
            ThisDraw.InvalidateVisual();
        }
        protected virtual void ChangeActualHeightAtBeginning()
        {
            Mains.ActualHeight = ActualHeight;
            Mains.ActualWidth = ActualWidth;
        }
        public BaseGraphicsWPF()
        {
            ThisDraw = new SKElement();
            ThisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Mains = new G();
            Mains.PaintUI = this;
            ChangeActualHeightAtBeginning();
            MouseDoubleClick += BaseGraphics_MouseDoubleClick;
            MinHeight = Mains.MinimumHeight;
            MinWidth = Mains.MinimumWidth;
            SetContent();
        }

        protected virtual void SetContent() // most of the time, the draw will be the entire content.  however, for dice, i think its going to be different.
        {
            Content = ThisDraw;
        }


        protected virtual void BeforePainting() { }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Execute.OnUIThread(() =>
            {
                BeforePainting();
                StepThroughManualRepaint();
                Mains.ActualHeight = e.Info.Height;
                Mains.ActualWidth = e.Info.Width;
                Mains.DrawImage(e.Surface.Canvas);
            });
        }
        protected virtual void StepThroughManualRepaint() { }
        public void RepaintManually()
        {
            ThisDraw.InvalidateVisual(); // try this.  no guarantees though.
        }
        private void BaseGraphics_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tempCommand = DoubleCommand;
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(CommandParameter) == true)
                    tempCommand.Execute(CommandParameter);
            }
        }
        public void DoInvalidate()
        {
            ThisDraw.InvalidateVisual();
        }
    }
}
