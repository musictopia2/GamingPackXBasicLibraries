using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Dominos;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.Dominos
{
    public class DominosWPF<D> : BaseDeckGraphicsWPF<D, DominosCP>
         where D : IDominoInfo, new()
    {
        public static readonly DependencyProperty CurrentFirstProperty = DependencyProperty.Register("CurrentFirst", typeof(int), typeof(DominosWPF<D>), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(CurrentFirstPropertyChanged)));
        public int CurrentFirst
        {
            get
            {
                return (int)GetValue(CurrentFirstProperty);
            }
            set
            {
                SetValue(CurrentFirstProperty, value);
            }
        }
        private static void CurrentFirstPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DominosWPF<D>)sender;
            thisItem.MainObject!.CurrentFirst = (int)e.NewValue;
        }
        public static readonly DependencyProperty CurrentSecondProperty = DependencyProperty.Register("CurrentSecond", typeof(int), typeof(DominosWPF<D>), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(CurrentSecondPropertyChanged)));
        public int CurrentSecond
        {
            get
            {
                return (int)GetValue(CurrentSecondProperty);
            }
            set
            {
                SetValue(CurrentSecondProperty, value);
            }
        }
        private static void CurrentSecondPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DominosWPF<D>)sender;
            thisItem.MainObject!.CurrentSecond = (int)e.NewValue;
        }
        public static readonly DependencyProperty HighestDominoProperty = DependencyProperty.Register("HighestDomino", typeof(int), typeof(DominosWPF<D>), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(HighestDominoPropertyChanged)));
        public int HighestDomino
        {
            get
            {
                return (int)GetValue(HighestDominoProperty);
            }
            set
            {
                SetValue(HighestDominoProperty, value);
            }
        }
        private static void HighestDominoPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DominosWPF<D>)sender;
            thisItem.MainObject!.HighestDomino = (int)e.NewValue;
        }
        protected override void PopulateInitObject()
        {
            if (MainObject == null)
                throw new BasicBlankException("Must have the main object before initializing domino");
            MainObject.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CurrentFirstProperty, nameof(IDominoInfo.CurrentFirst));
            SetBinding(CurrentSecondProperty, nameof(IDominoInfo.CurrentSecond));
            SetBinding(HighestDominoProperty, nameof(IDominoInfo.HighestDomino));
        }
    }
}