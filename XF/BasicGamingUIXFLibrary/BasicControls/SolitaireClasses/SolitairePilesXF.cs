using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BasicGamingUIXFLibrary.BasicControls.SolitaireClasses
{
    public class SolitairePilesXF : ContentView
    {
        private StackLayout? _thisStack;
        private SolitairePilesCP? _thisMod;
        private IndividualSolitairePileXF? FindControl(PileInfoCP thisPile)
        {
            foreach (var firstControl in _thisStack!.Children)
            {
                var thisGraphics = firstControl as IndividualSolitairePileXF;
                if (thisGraphics!.BindingContext.Equals(thisPile))
                    return thisGraphics;
            }
            return null; //you can have null this time.
        }
        //public void ReloadSavedBoard()
        //{
        //    _thisMod!.PileList.ForEach(thisPile =>
        //    {
        //        var thisG = FindControl(thisPile);
        //        thisG!.ThisPile = thisPile;
        //        thisG.LoadSavedBoard();
        //    });
        //}
        //public void Dispose()
        //{
        //    _thisMod!.PileList.ForEach(thisPile =>
        //    {
        //        var thisG = FindControl(thisPile);
        //        thisG!.Dispose();
        //    });
        //}

        private static bool _needsPopulating = true;
        public void Init(SolitairePilesCP thisMod)
        {
            _thisStack = new StackLayout();
            SolitaireCard card = new SolitaireCard();
            DeckOfCardsXF<SolitaireCard> temps = new DeckOfCardsXF<SolitaireCard>();
            temps.SendSize(ts.TagUsed, card);
            var size = temps.Measure(double.PositiveInfinity, double.PositiveInfinity);
            if (size.Request.Height == 0 || size.Request.Width == 0)
                throw new BasicBlankException("Was unable to get the width or height request.  Rethink");
            else
                SolitaireOverlapLayoutXF.MinWidth = size.Request.Width + 9;
            _thisStack.Orientation = StackOrientation.Horizontal;
            _thisStack.Spacing = 0;
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Horizontal;
            _thisMod = thisMod;
            if (thisMod.PileList.Count == 0)
                throw new BasicBlankException("Must have at least one pile.  Otherwise, not worth even using this");
            thisMod.PileList.ForEach(thisPile =>
            {
                IndividualSolitairePileXF thisG = new IndividualSolitairePileXF();
                thisG.MainMod = thisMod;
                thisG.ThisPile = thisPile;
                thisG.Margin = new Thickness(0, 0, 0, 0); //try this
                _thisStack.Children.Add(thisG);
                thisG.Init(_needsPopulating);
            });
            _needsPopulating = false;
            thisScroll.Content = _thisStack;
            Content = thisScroll;
        }
    }
}