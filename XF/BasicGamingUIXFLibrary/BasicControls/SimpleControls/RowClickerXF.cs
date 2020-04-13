using BasicGamingUIXFLibrary.GameGraphics.Base;
using SkiaSharp;
using System.Windows.Input;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.BasicControls.SimpleControls
{
    public class RowClickerXF : GraphicsCommand
    {
        //hopefully this simple.

        //only needed the firsttouch to prove something.
        public RowClickerXF()
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill; //for cases i am using this, needs to be filled period.
        }
        protected override void FirstTouch()
        {
            base.FirstTouch();
        }

    }
}