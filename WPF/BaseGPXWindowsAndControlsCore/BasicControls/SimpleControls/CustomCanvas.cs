using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using System.Windows;
using System.Windows.Controls;
namespace BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls
{
    public class CustomCanvas : Canvas, ICanvas
    {
        public void SetLocation(ISelectableObject thisImage, double x, double y)
        {
            SetLeft((UIElement)thisImage, x);
            SetTop((UIElement)thisImage, y);
        }
        public void Clear()
        {
            Children.Clear();
        }
        public void AddChild(ISelectableObject thisImage)
        {
            Children.Add((UIElement)thisImage);
        }
    }
}