using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.SimpleControls
{
    public class CustomCanvasXF : AbsoluteLayout, ICanvas
    {
        public void SetLocation(ISelectableObject thisImage, double x, double y)
        {
            View thisView;
            thisView = (View)thisImage;
            Rectangle thisRect;
            thisRect = new Rectangle(x, y, thisView.WidthRequest, thisView.HeightRequest);
            if (thisView.Width == 0)
                throw new BasicBlankException("The width can't be 0");
            SetLayoutBounds((View)thisImage, thisRect);
        }
        public void Clear()
        {
            Children.Clear();
        }
        public void AddChild(ISelectableObject thisImage)
        {
            Children.Add((View)thisImage);
        }
    }
}