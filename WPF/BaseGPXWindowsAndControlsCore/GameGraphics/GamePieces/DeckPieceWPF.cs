using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using System.Windows;
using BasicGameFramework.RegularDeckOfCards;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces
{
    public class DeckPieceWPF : BaseGraphicsWPF<DeckPieceCP>
    {
        public static readonly DependencyProperty SuitProperty = DependencyProperty.Register("Suit", typeof(EnumSuitList), typeof(DeckPieceWPF), new FrameworkPropertyMetadata(EnumSuitList.None, new PropertyChangedCallback(SuitPropertyChanged)));
        public EnumSuitList Suit
        {
            get
            {
                return (EnumSuitList)GetValue(SuitProperty);
            }
            set
            {
                SetValue(SuitProperty, value);
            }
        }
        private static void SuitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckPieceWPF)sender;
            thisItem.Mains.Suit = (EnumSuitList)e.NewValue;
        }
    }
}