using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIWPFLibrary.GameGraphics.Dice
{
    public sealed class StandardDiceWPF : GraphicsCommand, ISelectableObject, IRepaintControl
    {
        private StandardDiceGraphicsCP? _mains; //just keep mains
        private readonly SKElement _thisDraw;
        public static readonly DependencyProperty DiceValueProperty = DependencyProperty.Register("DiceValue", typeof(int), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiceValuePropertyChanged)));
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
        private static void DiceValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.Value = (int)e.NewValue;
        }
        public static readonly DependencyProperty DotColorProperty = DependencyProperty.Register("DotColor", typeof(string), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DotColorPropertyChanged)));
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
        private static void DotColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.DotColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty DiceStyleProperty = DependencyProperty.Register("DiceStyle", typeof(EnumStyle), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiceStylePropertyChanged)));
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
        private static void DiceStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.Style = (EnumStyle)e.NewValue;
        }
        public bool Hold
        {
            get { return (bool)GetValue(HoldProperty); }
            set { SetValue(HoldProperty, value); }
        }
        public static readonly DependencyProperty HoldProperty =
            DependencyProperty.Register("Hold", typeof(bool), typeof(StandardDiceWPF),
                 new FrameworkPropertyMetadata(new PropertyChangedCallback(HoldPropertyChanged)));
        private static void HoldPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.Hold = (bool)e.NewValue; // hopefully will be that simple  this will trigger the paint event.  hoepfully this way will still work out.
        }
        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("MainColor", typeof(string), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(FillColorPropertyChanged)));
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
        private static void FillColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.FillColor = e.NewValue.ToString()!; // hopefully will be that simple  this will trigger the paint event.  hoepfully this way will still work out.
        }

        public static readonly DependencyProperty DoubleCommandProperty = DependencyProperty.Register("DoubleCommand", typeof(ICommand), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DoubleCommandPropertyChanged)));
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
        private static void CommandParameterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsSelectedPropertyChanged)));
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
            var thisItem = (StandardDiceWPF)sender;
            thisItem._mains!.IsSelected = (bool)e.NewValue;
        }
        public void Init() // the actual is not working at this moment until the property change happens
        {
            double thisD = Width - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = (float)thisD;
            _thisDraw.InvalidateVisual();
        }
        public static string GetDiceTag => "StandardDice";
        public void SendDiceInfo(IStandardDice thisDice) //it did send dice
        {
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _mains = new StandardDiceGraphicsCP(this);
            _mains.OriginalHeightWidth = thisDice.HeightWidth;
            SetBinding(DiceStyleProperty, new Binding(nameof(IStandardDice.Style)));
            SetBinding(DotColorProperty, new Binding(nameof(IStandardDice.DotColor)));
            SetBinding(DiceValueProperty, new Binding(nameof(IStandardDice.Value)));
            SetBinding(FillColorProperty, new Binding(nameof(IStandardDice.FillColor)));
            SetBinding(HoldProperty, new Binding(nameof(IStandardDice.Hold)));
            SetBinding(IsSelectedProperty, new Binding(nameof(IStandardDice.IsSelected))); //maybe i forgot this too.
            DataContext = thisDice;
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = tempSize.GetSizeUsed(thisP.Proportion);
            Height = DiceSize.Height;
            Width = DiceSize.Width;
            Init();
        }
        public static readonly DependencyProperty DiceSizeProperty = DependencyProperty.Register("DiceSize", typeof(SKSize), typeof(StandardDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiceSizePropertyChanged)));
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
        private static void DiceSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateVisual();
        }
        public StandardDiceWPF()
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            //MouseUp += BaseGraphics_MouseUp;
            MouseDoubleClick += BaseGraphics_MouseDoubleClick;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            BeforePainting();
            _mains!.DrawDice(e.Surface.Canvas);
        }
        private void BeforePainting()
        {
            var thisDice = (IStandardDice)DataContext;
            _mains!.OriginalHeightWidth = thisDice.HeightWidth;
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
        //private void BaseGraphics_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    var tempCommand = Command;
        //    if (tempCommand != null)
        //    {
        //        if (tempCommand.CanExecute(CommandParameter) == true)
        //            tempCommand.Execute(CommandParameter);
        //    }
        //}
    }
}
