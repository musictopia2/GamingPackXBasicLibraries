using SkiaSharp;
using SkiaSharp.Views.WPF;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameBoards
{
    public class SkiaSharpGameBoard : SKElement, ISkiaSharpGameBoard
    {
        public event SingleClickBoardEventHandler? SingleClickBoard;
        public event CPPaintEventHandler? CPPaint;
        public void StartClick(double x, double y)
        {
            SingleClickBoard?.Invoke(x, y);
        }
        public void StartInvalidate(SKCanvas canvas, double width, double height)
        {
            CPPaint?.Invoke(canvas, width, height);
        }
    }
}