using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using Xamarin.Forms;
namespace BasicGamingUIXFLibrary.BasicControls.TrickUIs
{
    public class TrickCanvasXF : AbsoluteLayout, ITrickCanvas
    {
        public View GetCard(int index)
        {
            int x = 0; //0 based
            foreach (var thisChild in Children)
                if (!(thisChild is Label Temps) == true)
                {
                    if (x == index)
                        return thisChild!;
                    x += 1;
                }
            throw new BasicBlankException($"{index} not found for getting the card image");
        }
        public void SetLocation(int index, double x, double y)
        {
            var thisChild = GetCard(index);
            var thisRect = new Rectangle(x, y, thisChild.WidthRequest, thisChild.HeightRequest);
            if (thisChild.Width == 0)
                throw new BasicBlankException("The width can't be 0");
            SetLayoutBounds(thisChild, thisRect);
        }
        public SKPoint GetStartingPoint(int index)
        {
            var thisChild = GetCard(index);
            var thisRect = GetLayoutBounds(thisChild);
            return new SKPoint((float)thisRect.X, (float)thisRect.Y);
        }
    }
}