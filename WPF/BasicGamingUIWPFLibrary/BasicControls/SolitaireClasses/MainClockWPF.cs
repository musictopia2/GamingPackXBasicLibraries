using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using System.Windows.Controls;
namespace BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses
{
    public class MainClockWPF : UserControl, ISpecialSolitaireReposition
    {
        ClockObservable? _thisMod;
        Canvas? _thisCanvas;
        public void LoadControls(ClockObservable thisMod)
        {
            _thisMod = thisMod;
            _thisCanvas = new Canvas();
            thisMod.PositionUI = this;
            Content = _thisCanvas;
            if (_thisMod.ClockList != null && _thisMod.ClockList.Count > 0)
                PrivateReposition();
        }
        private void PrivateReposition()
        {
            _thisCanvas!.Children.Clear();
            _thisMod!.ClockList!.ForEach(thisClock =>
            {
                IndividualClockWPF clockUI = new IndividualClockWPF();
                clockUI.Init(thisClock, _thisMod);
                Canvas.SetLeft(clockUI, thisClock.Location.X);
                Canvas.SetTop(clockUI, thisClock.Location.Y);
                _thisCanvas.Children.Add(clockUI);
            });
        }
        void ISpecialSolitaireReposition.RepositionCardsOnUI()
        {
            PrivateReposition();
        }
    }
}
