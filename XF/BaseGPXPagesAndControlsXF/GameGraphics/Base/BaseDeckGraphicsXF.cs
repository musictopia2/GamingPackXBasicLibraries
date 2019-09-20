using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Interfaces; //try this instead now.
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BaseGPXPagesAndControlsXF.GameGraphics.Base
{
    public abstract class BaseDeckGraphicsXF<CA, G> : BaseGraphicsXF<BaseDeckGraphicsCP>
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
            BindingContext = thisData;
            PopulateInitObject();
            Init(); //maybe i forgot this part.
            ObjectSize = thisData.DefaultSize.GetSizeUsed(thisP.Proportion);
        }
        public event AngleChangedEventHandler? AngleChanged;
        public delegate void AngleChangedEventHandler(object sender);
        public static readonly BindableProperty DrewProperty = BindableProperty.Create(propertyName: "Drew", defaultValue: false, returnType: typeof(bool), declaringType: typeof(BaseDeckGraphicsXF<CA, G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: DrewPropertyChanged);
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
        private static void DrewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseDeckGraphicsXF<CA, G>)bindable;
            thisItem.MainObject!.Drew = (bool)newValue;
        }
        public static readonly BindableProperty BackGroundColorProperty = BindableProperty.Create(propertyName: "BackGroundColor", defaultValue: cs.Transparent, returnType: typeof(string), declaringType: typeof(BaseDeckGraphicsXF<CA, G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: BackGroundColorPropertyChanged);
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
        private static void BackGroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseDeckGraphicsXF<CA, G>)bindable;
            thisItem.Mains.BackgroundColor = (string)newValue;
        }
        public static readonly BindableProperty ObjectSizeProperty = BindableProperty.Create(propertyName: "ObjectSize", defaultValue: new SKSize(), returnType: typeof(SKSize), declaringType: typeof(BaseDeckGraphicsXF<CA, G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: ObjectSizePropertyChanged);
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
        private static void ObjectSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseDeckGraphicsXF<CA, G>)bindable;
            var oNewValue = (SKSize)newValue;
            thisItem.Mains.ActualHeight = oNewValue.Height;
            thisItem.Mains.ActualWidth = oNewValue.Width; // try this way.
            thisItem.ThisDraw.InvalidateSurface(); // try this too.
            thisItem.OnObjectSizeChanged();
        }
        protected virtual void OnObjectSizeChanged() { }
        public static readonly BindableProperty IsUnknownProperty = BindableProperty.Create(propertyName: "IsUnknown", defaultValue: false, returnType: typeof(bool), declaringType: typeof(BaseDeckGraphicsXF<CA, G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsUnknownPropertyChanged);
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
        private static void IsUnknownPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseDeckGraphicsXF<CA, G>)bindable;
            thisItem.Mains.IsUnknown = (bool)newValue;
            thisItem.OnUnknownChange();
        }
        protected virtual void OnUnknownChange() { }
        protected bool HasAngles = true; //i think.  if i am wrong, rethink.
        public void Repaint()
        {
            ThisDraw.InvalidateSurface();
        }
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (HasAngles == false)
                return base.OnMeasure(widthConstraint, heightConstraint);
            var TempAngle = Angle;
            Size ThisSize;
            if ((int)TempAngle == (int)RotateExtensions.EnumRotateCategory.FlipAndRotate270 || (int)TempAngle == (int)RotateExtensions.EnumRotateCategory.RotateOnly90)
                ThisSize = new Size(ObjectSize.Height, ObjectSize.Width); // its been proven to get to this line of code.
            else
                ThisSize = new Size(ObjectSize.Width, ObjectSize.Height);
            return new SizeRequest(ThisSize);
        }
        protected override void ChangeSizeAtStart() { }// can't do anything for sizes because of the possibility of angles being used.
        public static readonly BindableProperty AngleProperty = BindableProperty.Create(propertyName: "Angle", defaultValue: RotateExtensions.EnumRotateCategory.None, returnType: typeof(RotateExtensions.EnumRotateCategory), declaringType: typeof(BaseDeckGraphicsXF<CA, G>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: AnglePropertyChanged);
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
        private static void AnglePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (BaseDeckGraphicsXF<CA, G>)bindable;
            thisItem.Mains.Angle = (RotateExtensions.EnumRotateCategory)newValue;
            thisItem.DoRaiseEvent();
            thisItem.InvalidateMeasure();
            thisItem.ThisDraw.InvalidateSurface();
        }
        private void DoRaiseEvent()
        {
            AngleChanged?.Invoke(this); // so the frames can update its sizes accordingly.
        }
        protected abstract void DoCardBindings();
        protected override void BeforePaint(int paintHeight, int paintWidth)
        {
            var thisCard = (CA)BindingContext;
            Mains.OriginalSize = thisCard.DefaultSize;
            if (Angle == RotateExtensions.EnumRotateCategory.FlipAndRotate270 || Angle == RotateExtensions.EnumRotateCategory.RotateOnly90)
            {
                base.BeforePaint(paintWidth, paintHeight);
                return;
            }
            base.BeforePaint(paintHeight, paintWidth);
        }
        private void FirstBindings()
        {
            Binding thisBind = new Binding(nameof(IDeckObject.Visible)); // should be okay
            SetBinding(IsVisibleProperty, thisBind);
            SetBinding(DrewProperty, new Binding(nameof(IDeckObject.Drew)));
            SetBinding(IsUnknownProperty, new Binding(nameof(IDeckObject.IsUnknown)));
            SetBinding(IsSelectedProperty, new Binding(nameof(IDeckObject.IsSelected)));
            SetBinding(AngleProperty, new Binding(nameof(IDeckObject.Angle)));
        }
        protected virtual void FirstSetUp() { }
        public BaseDeckGraphicsXF()
        {
            FirstSetUp();
            FirstBindings();
            DoCardBindings();
        }
    }
}