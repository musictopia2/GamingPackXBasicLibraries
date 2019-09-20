using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace SolitaireGraphicsXF
{
    public class SolitaireOverlapLayoutXF : Layout<DeckOfCardsXF<SolitaireCard>>
    {
        public bool IsKlondike { get; set; }
        internal static double MinWidth; // i think this should be shared.  because cards are not going to be different sizes.
        public DeckOfCardsXF<SolitaireCard>? FindControl(SolitaireCard thisCard)
        {
            foreach (var thisDeck in Children)
            {
                if (thisDeck.BindingContext.Equals(thisCard) == true)
                    return thisDeck;
            }
            return null;
        }
        private double _calculateMaxWidth;
        private double _calculateMaxHeight;
        private CustomBasicList<Rectangle> ComputeNaiveLayout()
        {
            _calculateMaxWidth = 0;
            _calculateMaxHeight = 0; // i think this may allow the scrollbars to work right.
            CustomBasicList<Rectangle> result = new CustomBasicList<Rectangle>();
            double y = 0;
            int x = 3;
            double calWidth;
            double calHeight;
            if (Children.Count == 0)
            {
                _calculateMaxWidth = MinWidth;
                _calculateMaxHeight = 100; // at least 100 in this case.
                return new CustomBasicList<Rectangle>();
            }
            int z = 0;
            foreach (var child in Children)
            {
                z += 1;
                Rectangle thisRect = new Rectangle();
                if (y < 0)
                    throw new BasicBlankException("Y cannot be less than 0");
                thisRect.X = x;
                thisRect.Y = y;
                var thisSize = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
                thisRect.Height = thisSize.Request.Height;
                thisRect.Width = thisSize.Request.Width;
                calWidth = thisRect.Right + 6;
                calHeight = thisRect.Bottom;
                if ((IsKlondike == false) | (child.IsUnknown == false))
                    y += (thisRect.Height / 4); // if we need further adjustments, can make
                else
                    y += 7;
                if (calWidth > _calculateMaxWidth)
                    _calculateMaxWidth = calWidth;
                if (calHeight > _calculateMaxHeight)
                    _calculateMaxHeight = calHeight;
                result.Add(thisRect);
            }
            return result;
        }
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            ComputeNaiveLayout();
            return new SizeRequest(new Size(_calculateMaxWidth, _calculateMaxHeight));
        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            int i = 0;
            CustomBasicList<Rectangle> Layout;
            Layout = ComputeNaiveLayout();
            foreach (var Region in Layout)
            {
                var Child = Children[i];
                LayoutChildIntoBoundingRegion(Child, Region);
                i += 1;
            }
        }
        public SolitaireOverlapLayoutXF()
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
        }
    }
}