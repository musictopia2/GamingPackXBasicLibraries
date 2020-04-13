using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows.Input;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGamingUIXFLibrary.GameGraphics.Dice
{
    public sealed class StandardDiceXF : ContentView, ISelectableObject, IRepaintControl
    {
        private StandardDiceGraphicsCP? _mains; //just keep mains
        private readonly SKCanvasView _thisDraw;
        public static readonly BindableProperty DiceValueProperty = BindableProperty.Create(propertyName: "DiceValue", returnType: typeof(int), declaringType: typeof(StandardDiceXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DiceValuePropertyChanged);
        public int DiceValue
        {
            get
            {
                return (int)GetValue(DiceValueProperty);
            }
            set
            {
                SetValue(DiceValueProperty, value);
            }
        }
        private static void DiceValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            thisItem._mains!.Value = (int)newValue;
        }
        public static readonly BindableProperty DotColorProperty = BindableProperty.Create(propertyName: "DotColor", returnType: typeof(string), declaringType: typeof(StandardDiceXF), defaultValue: cs.Black, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DotColorPropertyChanged);
        public string DotColor
        {
            get
            {
                return (string)GetValue(DotColorProperty);
            }
            set
            {
                SetValue(DotColorProperty, value);
            }
        }
        private static void DotColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            if (newValue == null)
                thisItem._mains!.DotColor = cs.Black;
            else
                thisItem._mains!.DotColor = (string)newValue;
        }
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(propertyName: "FillColor", returnType: typeof(string), declaringType: typeof(StandardDiceXF), defaultValue: cs.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FillColorPropertyChanged);
        public string FillColor
        {
            get
            {
                return (string)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }
        private static void FillColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            if (newValue == null)
                thisItem._mains!.FillColor = cs.White;
            else
                thisItem._mains!.FillColor = (string)newValue;
        }
        public static readonly BindableProperty DiceStyleProperty = BindableProperty.Create(propertyName: "DiceStyle", returnType: typeof(EnumStyle), declaringType: typeof(StandardDiceXF), defaultValue: EnumStyle.Regular, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DiceStylePropertyChanged);
        public EnumStyle DiceStyle
        {
            get
            {
                return (EnumStyle)GetValue(DiceStyleProperty);
            }
            set
            {
                SetValue(DiceStyleProperty, value);
            }
        }
        private static void DiceStylePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            thisItem._mains!.Style = (EnumStyle)newValue;
        }
        public static readonly BindableProperty HoldProperty = BindableProperty.Create(propertyName: "Hold", returnType: typeof(bool), declaringType: typeof(StandardDiceXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HoldPropertyChanged);
        public bool Hold
        {
            get
            {
                return (bool)GetValue(HoldProperty);
            }
            set
            {
                SetValue(HoldProperty, value);
            }
        }
        private static void HoldPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            thisItem._mains!.Hold = (bool)newValue;
        }
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: "Command", returnType: typeof(ICommand), declaringType: typeof(StandardDiceXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandPropertyChanged);
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }
        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: "CommandParameter", returnType: typeof(object), declaringType: typeof(StandardDiceXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandParameterPropertyChanged);
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }
        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(propertyName: "IsSelected", defaultValue: false, returnType: typeof(bool), declaringType: typeof(StandardDiceXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsSelectedPropertyChanged);
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (StandardDiceXF)bindable;
            thisItem._mains!.IsSelected = (bool)newValue;
        }
        public void Init() // the actual is not working at this moment until the property change happens
        {
            double thisD = WidthRequest - Margin.Left - Margin.Right; //i think its widthrequest (?)
            _mains!.ActualWidthHeight = (float)thisD;
            _thisDraw.InvalidateSurface();
        }
        public static string GetDiceTag => "StandardDice";
        public void SendDiceInfo(IStandardDice thisDice) //it did send dice
        {
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _mains = new StandardDiceGraphicsCP(this);
            _mains.OriginalHeightWidth = thisDice.HeightWidth;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            SetBinding(DiceStyleProperty, new Binding(nameof(IStandardDice.Style)));
            SetBinding(DotColorProperty, new Binding(nameof(IStandardDice.DotColor)));
            SetBinding(DiceValueProperty, new Binding(nameof(IStandardDice.Value)));
            SetBinding(FillColorProperty, new Binding(nameof(IStandardDice.FillColor)));
            SetBinding(HoldProperty, new Binding(nameof(IStandardDice.Hold)));
            SetBinding(IsSelectedProperty, new Binding(nameof(IStandardDice.IsSelected))); //maybe i forgot this too.
            BindingContext = thisDice;
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = tempSize.GetSizeUsed(thisP.Proportion);
            HeightRequest = DiceSize.Height;
            WidthRequest = DiceSize.Width;
            _thisDraw.HeightRequest = DiceSize.Height;
            _thisDraw.WidthRequest = DiceSize.Width;
            Init();
        }
        public static readonly BindableProperty DiceSizeProperty = BindableProperty.Create(propertyName: "DiceSize", defaultValue: new SKSize(), returnType: typeof(SKSize), declaringType: typeof(StandardDiceXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: DiceSizePropertyChanged);
        public SKSize DiceSize
        {
            get
            {
                return (SKSize)GetValue(DiceSizeProperty);
            }
            set
            {
                SetValue(DiceSizeProperty, value);
            }
        }
        private static void DiceSizePropertyChanged(BindableObject bindable, object oldValue, object newValue) { } //hopefully okay for this like in wpf (?)
        public void DoInvalidate()
        {
            _thisDraw.InvalidateSurface();
        }
        public StandardDiceXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += DrawPaintSurface;
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += DrawTouch;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            Content = _thisDraw;
        }
        private void DrawTouch(object sender, SKTouchEventArgs e)
        {
            var tempCommand = Command;
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(CommandParameter) == true)
                    tempCommand.Execute(CommandParameter);
            }
        }
        private void BeforePainting()
        {
            var thisDice = (IStandardDice)BindingContext;
            _mains!.OriginalHeightWidth = thisDice.HeightWidth;
        }
        private void DrawPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            BeforePainting(); //i think.  if i am wrong, rethink.
            _mains!.ActualWidthHeight = e.Info.Height;
            _mains!.DrawDice(e.Surface.Canvas);
        }
    }
}
