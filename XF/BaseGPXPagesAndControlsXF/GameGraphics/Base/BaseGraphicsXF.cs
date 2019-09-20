using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BaseGPXPagesAndControlsXF.GameGraphics.Base
{
    public abstract class BaseGraphicsXF<G> : ContentView,
        ISelectableObject, IRepaintControl where G : BaseGraphicsCP, new()
    {
        protected G Mains;
        protected SKCanvasView ThisDraw;
        protected virtual bool IsCircle => false;
        public void SendPiece(G thisPiece) // this is needed so it can support the color picker.  the color picker already uses the cp control
        {
            Mains = thisPiece;
            thisPiece.PaintUI = this; //try here too.
            Init(); // i think needs to initialize here too.
        }
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(propertyName: "IsSelected", defaultValue: false, returnType: typeof(bool), declaringType: typeof(BaseGraphicsXF<G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsSelectedPropertyChanged);
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
        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseGraphicsXF<G>)bindable;
            thisItem.Mains.IsSelected = (bool)newValue;
        }
        public static readonly BindableProperty MainColorProperty = BindableProperty.Create(propertyName: "MainColor", defaultValue: cs.Transparent, returnType: typeof(string), declaringType: typeof(BaseGraphicsXF<G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: MainColorPropertyChanged);
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
        private static void MainColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseGraphicsXF<G>)bindable;
            if (newValue == null)
                thisItem.Mains.MainColor = cs.Transparent;
            else
                thisItem.Mains.MainColor = newValue.ToString();// trying this way.
        }
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: "Command", returnType: typeof(ICommand), declaringType: typeof(BaseGraphicsXF<G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandPropertyChanged);
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: "CommandParameter", returnType: typeof(object), declaringType: typeof(BaseGraphicsXF<G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandParameterPropertyChanged);
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty DoubleCommandProperty = BindableProperty.Create(propertyName: "DoubleCommand", returnType: typeof(ICommand), declaringType: typeof(BaseGraphicsXF<G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: DoubleCommandPropertyChanged);
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
        }
        private static void DoubleCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty NeedsHighLightingProperty = BindableProperty.Create(propertyName: "NeedsHighLighting", returnType: typeof(bool), declaringType: typeof(BaseGraphicsXF<G>), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NeedsHighLightingPropertyChanged);
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
        public bool DisableInput
        {
            get
            {
                return ThisDraw.InputTransparent;
            }
            set
            {
                ThisDraw.InputTransparent = value;
            }
        }

        private static void NeedsHighLightingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseGraphicsXF<G>)bindable;
            thisItem.Mains.NeedsHighlighting = (bool)newValue;
        }
        public virtual void Init() // the actual is not working at this moment until the property change happens
        {
            Mains.ActualWidth = WidthRequest - Margin.Left - Margin.Right;
            Mains.ActualHeight = HeightRequest - Margin.Bottom - Margin.Top;
            ThisDraw.InvalidateSurface();
        }
        protected virtual void ChangeSizeAtStart() //i think
        {
            Mains.ActualHeight = Height;
            Mains.ActualWidth = Width;
            MinimumHeightRequest = Mains.MinimumHeight;
            MinimumWidthRequest = Mains.MinimumWidth;
        }
        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsEnabled))
            {
                if (NeedsHighLighting == true)
                    Mains.IsEnabled = IsEnabled;// i think
            }
        }
        protected virtual void SetUpContent()
        {
            Content = ThisDraw;
        }
        public BaseGraphicsXF()
        {
            ThisDraw = new SKCanvasView();
            ThisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start; //try this too (?)
            Mains = new G();
            Mains.PaintUI = this;
            ThisDraw.EnableTouchEvents = true;
            ThisDraw.Touch += ThisDraw_Touch;
            SetUpContent();
        }
        private void ThisDraw_Touch(object sender, SKTouchEventArgs e)
        {
            if (Command == null)
                return;
            if (Command.CanExecute(CommandParameter) == true)
                TouchProcess(e.Location);
        }
        private async void TouchProcess(SKPoint thisPoint)
        {
            await Task.Delay(20);
            BeforeProcessClick(Command, Mains, CommandParameter, thisPoint);
            Command.Execute(CommandParameter);
        }
        protected virtual void BeforeProcessClick(ICommand thisCommand, G thisObj, object thisParameter, SKPoint clickPoint) { }
        protected virtual void StepThroughManualRepaint() { }
        private void ThisDraw_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            BeforePaint(e.Info.Height, e.Info.Width);
            StepThroughManualRepaint();
            Mains.ActualHeight = e.Info.Height;
            Mains.ActualWidth = e.Info.Width;
            if (IsCircle)
            {
                if (Mains.ActualHeight > Mains.ActualWidth)
                    Mains.ActualHeight = Mains.ActualWidth;
                if (Mains.ActualWidth > Mains.ActualHeight)
                    Mains.ActualWidth = Mains.ActualHeight;
            }
            Mains.DrawImage(e.Surface.Canvas);
        }
        protected virtual void BeforePaint(int paintHeight, int paintWidth) // this is most of the time.  but can be overrided to what is needed.  card games does different.
        {
            Mains.ActualHeight = paintHeight;
            Mains.ActualWidth = paintWidth;
        }
        public void DoInvalidate() => ThisDraw.InvalidateSurface(); //if somebody wants to do manually, do it via doinvalidate.
    }
}