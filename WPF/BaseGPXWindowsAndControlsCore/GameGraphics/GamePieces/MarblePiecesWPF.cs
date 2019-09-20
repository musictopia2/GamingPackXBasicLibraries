using System;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using System.Windows;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces
{
    public class MarblePiecesWPF<E> : BaseGraphicsWPF<MarblePiecesCP<E>>
        where E : struct, Enum
    {
        public static readonly DependencyProperty UseTroubleProperty = DependencyProperty.Register("UseTrouble", typeof(bool), typeof(MarblePiecesWPF<E>), new FrameworkPropertyMetadata(new PropertyChangedCallback(UseTroublePropertyChanged)));
        public bool UseTrouble
        {
            get
            {
                return (bool)GetValue(UseTroubleProperty);
            }
            set
            {
                SetValue(UseTroubleProperty, value);
            }
        }
        private static void UseTroublePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MarblePiecesWPF<E>)sender;
            thisItem.Mains.UseTrouble = (bool)e.NewValue;
        }
    }
}