using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
namespace BasicGamingUIWPFLibrary.BasicControls.TrickUIs
{
    public class TrickCanvas : Canvas, ITrickCanvas
    {
        public UIElement GetCard(int index)
        {
            int x = 0; //0 based
            foreach (var thisChild in Children)
            {
                if (!(thisChild is TextBlock Temps) == true)
                {
                    if (x == index)
                        return (UIElement)thisChild!;
                    x += 1;
                }
            }
            throw new BasicBlankException($"{index} not found for getting the card image");
        }
        public void SetLocation(int index, double x, double y)
        {
            var thisChild = GetCard(index);
            SetLeft(thisChild, x);
            SetTop(thisChild, y);
        }
        public SKPoint GetStartingPoint(int index)
        {
            var thisChild = GetCard(index);
            double x;
            double y;
            x = GetLeft(thisChild);
            y = GetTop(thisChild);
            return new SKPoint((float)x, (float)y);
        }
    }
}
