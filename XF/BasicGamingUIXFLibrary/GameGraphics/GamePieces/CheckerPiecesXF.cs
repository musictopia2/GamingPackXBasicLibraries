using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGamingUIXFLibrary.GameGraphics.GamePieces
{
    public class CheckerPiecesXF : BaseGraphicsXF<CheckerPiecesCP>
    {
        public static readonly BindableProperty BlankColorProperty = BindableProperty.Create(propertyName: "BlankColor", returnType: typeof(string), declaringType: typeof(CheckerPiecesXF), defaultValue: cs.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: BlankColorPropertyChanged);
        public string BlankColor
        {
            get
            {
                return (string)GetValue(BlankColorProperty);
            }
            set
            {
                SetValue(BlankColorProperty, value);
            }
        }
        private static void BlankColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CheckerPiecesXF)bindable;
            thisItem.Mains.BlankColor = (string)newValue;
        }
        public static readonly BindableProperty HasImageProperty = BindableProperty.Create(propertyName: "HasImage", defaultValue: true, returnType: typeof(bool), declaringType: typeof(CheckerPiecesXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: HasImagePropertyChanged);
        public bool HasImage
        {
            get
            {
                return (bool)GetValue(HasImageProperty);
            }
            set
            {
                SetValue(HasImageProperty, value);
            }
        }
        private static void HasImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CheckerPiecesXF)bindable;
            thisItem.Mains.HasImage = (bool)newValue;
        }
        public static readonly BindableProperty IsCrownedProperty = BindableProperty.Create(propertyName: "IsCrowned", defaultValue: false, returnType: typeof(bool), declaringType: typeof(CheckerPiecesXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsCrownedPropertyChanged);
        public bool IsCrowned
        {
            get
            {
                return (bool)GetValue(IsCrownedProperty);
            }
            set
            {
                SetValue(IsCrownedProperty, value);
            }
        }
        private static void IsCrownedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CheckerPiecesXF)bindable;
            thisItem.Mains.IsCrowned = (bool)newValue;
        }
        public static readonly BindableProperty FlatPieceProperty = BindableProperty.Create(propertyName: "FlatPiece", defaultValue: false, returnType: typeof(bool), declaringType: typeof(CheckerPiecesXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: FlatPiecePropertyChanged);
        public bool FlatPiece
        {
            get
            {
                return (bool)GetValue(FlatPieceProperty);
            }
            set
            {
                SetValue(FlatPieceProperty, value);
            }
        }
        private static void FlatPiecePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CheckerPiecesXF)bindable;
            thisItem.Mains.FlatPiece = (bool)newValue;
        }
    }
}