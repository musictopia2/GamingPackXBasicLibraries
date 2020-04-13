using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using SkiaSharp;
using SkiaSharp.Views.Forms;
namespace BasicGamingUIXFLibrary.BasicControls.GameBoards
{
    public class SkiaSharpGameBoardXF : SKCanvasView, ISkiaSharpGameBoard
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
        public SkiaSharpGameBoardXF()
        {
            EnableTouchEvents = true; //this needs to be automatic when using this control.
        }
    }
}