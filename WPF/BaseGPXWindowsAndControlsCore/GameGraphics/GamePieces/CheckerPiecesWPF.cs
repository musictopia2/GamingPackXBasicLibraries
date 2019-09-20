using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using System.Windows;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces
{
    public class CheckerPiecesWPF : BaseGraphicsWPF<CheckerPiecesCP>
    {
        public static readonly DependencyProperty BlankColorProperty = DependencyProperty.Register("BlankColor", typeof(string), typeof(CheckerPiecesWPF), new FrameworkPropertyMetadata(cs.White, new PropertyChangedCallback(BlankColorPropertyChanged)));
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
        private static void BlankColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CheckerPiecesWPF)sender;
            thisItem.Mains.BlankColor = (string)e.NewValue;
        }
        public static readonly DependencyProperty HasImageProperty = DependencyProperty.Register("HasImage", typeof(bool), typeof(CheckerPiecesWPF), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(HasImagePropertyChanged)));
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
        private static void HasImagePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CheckerPiecesWPF)sender;
            thisItem.Mains.HasImage = (bool)e.NewValue;
        }
        public static readonly DependencyProperty IsCrownedProperty = DependencyProperty.Register("IsCrowned", typeof(bool), typeof(CheckerPiecesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsCrownedPropertyChanged)));
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
        private static void IsCrownedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CheckerPiecesWPF)sender;
            thisItem.Mains.IsCrowned = (bool)e.NewValue;
        }
        public static readonly DependencyProperty FlatPieceProperty = DependencyProperty.Register("FlatPiece", typeof(bool), typeof(CheckerPiecesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(FlatPiecePropertyChanged)));
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
        private static void FlatPiecePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CheckerPiecesWPF)sender;
            thisItem.Mains.FlatPiece = (bool)e.NewValue;
        }
    }
}