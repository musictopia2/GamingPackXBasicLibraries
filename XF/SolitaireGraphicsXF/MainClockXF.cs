using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.ClockClasses;
using BaseSolitaireClassesCP.MiscClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using SkiaSharp;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SolitaireGraphicsXF
{
    public class MainClockXF : ContentView, ISpecialSolitaireReposition
    {
        ClockViewModel? _thisMod;
        AbsoluteLayout? _thisCanvas;
        SKSize _size;
        public void LoadControls(ClockViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisCanvas = new AbsoluteLayout();
            thisMod.PositionUI = this;
            Content = _thisCanvas;
            SolitaireCard card = new SolitaireCard();
            GamePackageDIContainer tt = Resolve<GamePackageDIContainer>();
            IProportionImage pp = tt.Resolve<IProportionImage>(ts.TagUsed);
            _size = card.DefaultSize.GetSizeUsed(pp.Proportion);
            if (_thisMod.ClockList != null && _thisMod.ClockList.Count > 0)
                PrivateReposition();
        }
        private void PrivateReposition()
        {
            _thisCanvas!.Children.Clear();
            _thisMod!.ClockList!.ForEach(thisClock =>
            {
                IndividualClockXF clockUI = new IndividualClockXF();
                clockUI.Init(thisClock, _thisMod);
                _thisCanvas.Children.Add(clockUI);
                var thisRect = new Rectangle(thisClock.Location.X, thisClock.Location.Y, _size.Width, _size.Height);
                AbsoluteLayout.SetLayoutBounds(clockUI, thisRect);
            });
        }
        void ISpecialSolitaireReposition.RepositionCardsOnUI()
        {
            PrivateReposition();
        }
    }
}