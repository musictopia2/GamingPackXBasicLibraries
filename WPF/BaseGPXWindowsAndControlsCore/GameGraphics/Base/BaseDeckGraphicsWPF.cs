using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Interfaces; //try this instead now.
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Windows;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.Base
{
    public abstract class BaseDeckGraphicsWPF<CA, G> : BaseGraphicsWPF<BaseDeckGraphicsCP>
        where CA : IDeckObject where G : class, IDeckGraphicsCP, new()
    {

        protected G? MainObject;
        protected virtual void PopulateInitObject() { }
        public void SendSize(string tag, CA thisData)
        {
            if (cons == null)
                throw new BasicBlankException("The resolver was never set");
            IGamePackageResolver thisR = (IGamePackageResolver)cons;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(tag);
            if (thisData.DefaultSize.Height <= 0 || thisData.DefaultSize.Width <= 0)
                throw new BasicBlankException("The height and width must be greater than 0"); //if this happens, has to really rethink
            Mains = new BaseDeckGraphicsCP();
            Mains.OriginalSize = thisData.DefaultSize;
            MainObject = new G();
            MainObject.MainGraphics = Mains;
            Mains.ThisGraphics = MainObject; //i think
            Mains.PaintUI = this; //i do need it after all here.
            DataContext = thisData;
            PopulateInitObject();
            Init(); //maybe i forgot this part.
            ObjectSize = thisData.DefaultSize.GetSizeUsed(thisP.Proportion);
        }

        public event AngleChangedEventHandler? AngleChanged;

        public delegate void AngleChangedEventHandler(object sender);

        public static readonly DependencyProperty DrewProperty = DependencyProperty.Register("Drew", typeof(bool), typeof(BaseDeckGraphicsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(DrewCardPropertyChanged)));
        public bool Drew
        {
            get
            {
                return (bool)GetValue(DrewProperty);
            }
            set
            {
                SetValue(DrewProperty, value);
            }
        }
        private static void DrewCardPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseDeckGraphicsWPF<CA, G>)sender;
            thisItem.MainObject!.Drew = (bool)e.NewValue;
        }
        public static readonly DependencyProperty BackGroundColorProperty = DependencyProperty.Register("BackGroundColor", typeof(string), typeof(BaseDeckGraphicsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(BackGroundColorPropertyChanged)));
        public string BackGroundColor
        {
            get
            {
                return (string)GetValue(BackGroundColorProperty);
            }
            set
            {
                SetValue(BackGroundColorProperty, value);
            }
        }
        private static void BackGroundColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseDeckGraphicsWPF<CA, G>)sender;
            thisItem.Mains.BackgroundColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty ObjectSizeProperty = DependencyProperty.Register("ObjectSize", typeof(SKSize), typeof(BaseDeckGraphicsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(ObjectSizePropertyChanged)));
        public SKSize ObjectSize
        {
            get
            {
                return (SKSize)GetValue(ObjectSizeProperty);
            }
            set
            {
                SetValue(ObjectSizeProperty, value);
            }
        }
        private static void ObjectSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseDeckGraphicsWPF<CA, G>)sender;
            var newValue = (SKSize)e.NewValue;
            thisItem.Mains.ActualHeight = newValue.Height;
            thisItem.Mains.ActualWidth = newValue.Width;
            thisItem.OnObjectSizeChanged();
        }
        protected virtual void OnObjectSizeChanged() // still needed because could have other controls now to worry  about.
        {
        }
        public static readonly DependencyProperty IsUnknownProperty = DependencyProperty.Register("IsUnknown", typeof(bool), typeof(BaseDeckGraphicsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsUnknownPropertyChanged)));
        public bool IsUnknown
        {
            get
            {
                return (bool)GetValue(IsUnknownProperty);
            }
            set
            {
                SetValue(IsUnknownProperty, value);
            }
        }
        private static void IsUnknownPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseDeckGraphicsWPF<CA, G>)sender;
            thisItem.Mains.IsUnknown = (bool)e.NewValue;
            thisItem.OnUnknownChange();
        }
        protected virtual void OnUnknownChange() { }
        protected override void ChangeActualHeightAtBeginning() // do nothing.  because the sizes has to be different
        {
        }
        protected bool HasAngles = true;
        protected override Size MeasureOverride(Size constraint) // if it does not fix it, then will need extra logic here.
        {
            if (HasAngles == false)
                return base.MeasureOverride(constraint);
            var tempAngle = Angle;
            if (tempAngle == RotateExtensions.EnumRotateCategory.FlipAndRotate270 || tempAngle == RotateExtensions.EnumRotateCategory.RotateOnly90)
            {
                return new Size(ObjectSize.Height, ObjectSize.Width);
            }
            else
                return new Size(ObjectSize.Width, ObjectSize.Height);
        }
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(RotateExtensions.EnumRotateCategory), typeof(BaseDeckGraphicsWPF<CA, G>), new FrameworkPropertyMetadata(new PropertyChangedCallback(AnglePropertyChanged)));
        public RotateExtensions.EnumRotateCategory Angle
        {
            get
            {
                return (RotateExtensions.EnumRotateCategory)GetValue(AngleProperty);
            }
            set
            {
                SetValue(AngleProperty, value);
            }
        }
        private static void AnglePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (BaseDeckGraphicsWPF<CA, G>)sender;
            thisItem.Mains.Angle = (RotateExtensions.EnumRotateCategory)e.NewValue;
            thisItem.DoRaiseEvent();
            thisItem.InvalidateMeasure();
        }
        private void DoRaiseEvent()
        {
            AngleChanged?.Invoke(this); // so the frames can update its sizes accordingly.
        }

        protected override void BeforePainting()
        {
            var ThisCard = (CA)DataContext;
            Mains.OriginalSize = ThisCard.DefaultSize; //i think
        }
        protected abstract void DoCardBindings();
        private void FirstBindings()
        {
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(IDeckObject.Visible), false));
            SetBinding(DrewProperty, new Binding(nameof(IDeckObject.Drew)));
            SetBinding(IsUnknownProperty, new Binding(nameof(IDeckObject.IsUnknown)));
            SetBinding(IsSelectedProperty, new Binding(nameof(IDeckObject.IsSelected)));
            SetBinding(AngleProperty, new Binding(nameof(IDeckObject.Angle)));
        }
        protected virtual void FirstSetUp() { }
        public BaseDeckGraphicsWPF()
        {
            FirstSetUp();
            FirstBindings();
            DoCardBindings();
        }
    }
}