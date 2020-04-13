using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System.Linq;
using Xamarin.Forms;
namespace BasicGamingUIXFLibrary.BasicControls.Layouts
{
    public class GameOverlapLayout<CA, GC> : Layout<BaseDeckGraphicsXF<CA, GC>>
        where CA : IDeckObject, new() where GC : class, IDeckGraphicsCP, new()
    {
        public StackOrientation Orientation { get; set; } = StackOrientation.Horizontal;// defaults to horizontal
        public double Spacing { get; set; } = 0;
        public double Divider { get; set; } = 1;
        public int MaximumCards { get; set; }
        public double ExtraControlSpace { get; set; } = 0.0;
        private double _calculateMaxWidth;
        private double _calculateMaxHeight;
        private readonly Size _objectSize;
        public BaseDeckGraphicsXF<CA, GC>? FindControl(CA thisCard)
        {
            foreach (var thisDeck in Children)
                if (thisDeck.BindingContext.Equals(thisCard))
                    return thisDeck;
            return null;
        }
        public GameOverlapLayout(SKSize sizeUsed)
        {
            _objectSize = new Size(sizeUsed.Width, sizeUsed.Height);
        }
        private CustomBasicList<Rectangle> ComputeNaiveLayout()//try with no try/catch for now.
        {
            _calculateMaxHeight = 0;
            _calculateMaxWidth = 0;
            CustomBasicList<Rectangle> output = new CustomBasicList<Rectangle>();
            double y = 0;
            double x = Padding.Left;
            double calWidth;
            double calHeight;
            int lefts = MaximumCards;
            foreach (var child in Children)
            {
                var thisRect = new Rectangle();
                thisRect.X = x;
                thisRect.Y = y;
                thisRect.Height = child.ObjectSize.Height;
                thisRect.Width = child.ObjectSize.Width;
                calWidth = thisRect.Right;
                calHeight = thisRect.Bottom;
                if (Orientation == StackOrientation.Horizontal)
                {
                    var adds = x + (thisRect.Width + Spacing) / Divider;
                    x = adds;
                    if (calWidth > _calculateMaxWidth)
                        _calculateMaxWidth = calWidth;
                }
                else
                {
                    y += (thisRect.Height + Spacing) / Divider;
                    if (calHeight > _calculateMaxHeight)
                        _calculateMaxHeight = calHeight;
                }
                lefts--;
                output.Add(thisRect);
            }
            Size tempSize;
            if (Children.Count > 0)
                tempSize = Children.First().Measure(double.PositiveInfinity, double.PositiveInfinity).Request;
            else
                tempSize = _objectSize;
            var loopTo = lefts;
            for (var z = 1; z <= loopTo; z++)
            {
                Rectangle newRect = new Rectangle();
                newRect.X = x;
                newRect.Y = y;
                newRect.Width = tempSize.Width;
                newRect.Height = tempSize.Height;
                calWidth = newRect.Right;
                calHeight = newRect.Bottom;
                if (Orientation == StackOrientation.Horizontal)
                {
                    x += (newRect.Width + Spacing) / Divider;
                    if (calWidth > _calculateMaxWidth)
                        _calculateMaxWidth = calWidth;
                }
                else
                {
                    y += (newRect.Height + Spacing) / Divider;
                    if (calHeight > _calculateMaxHeight)
                        _calculateMaxHeight = calHeight;
                }
            }
            if (Orientation == StackOrientation.Horizontal)
            {
                _calculateMaxHeight = tempSize.Height + ExtraControlSpace;
                _calculateMaxWidth += 2;
            }
            else
            {
                _calculateMaxWidth = tempSize.Width + ExtraControlSpace;
                _calculateMaxHeight += 2;
            }
            return output;
        }
        public float GetHeight => (float)_calculateMaxHeight; //try this way
        public float GetWidth => (float)_calculateMaxWidth;
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            ComputeNaiveLayout(); // may have to recalculate here but don't know. its better to be safe than sorry
            return new SizeRequest(new Size(_calculateMaxWidth, _calculateMaxHeight));
        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            int i = 0;
            CustomBasicList<Rectangle> layout = ComputeNaiveLayout();
            foreach (var region in layout)
            {
                var child = Children[i];
                LayoutChildIntoBoundingRegion(child, region);
                i++;
            }
        }
    }
}