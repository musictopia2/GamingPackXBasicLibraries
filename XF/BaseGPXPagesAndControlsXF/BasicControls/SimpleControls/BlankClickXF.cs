using SkiaSharp.Views.Forms;
using System.Windows.Input;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.SimpleControls
{
    public class BlankClickXF : ContentView
    {
        private SKCanvasView? _thisCanvas;
        private ICommand? _thisCommand;
        private object? _thisObject;
        public void Init(ICommand command, object thisObject)
        {
            _thisCommand = command;
            _thisObject = thisObject;
            _thisCanvas = new SKCanvasView();
            _thisCanvas.EnableTouchEvents = true;
            _thisCanvas.Touch += CanvasTouch;
            Content = _thisCanvas;
        }
        private void CanvasTouch(object sender, SKTouchEventArgs e)
        {
            if (_thisCommand == null)
                return;
            if (_thisCommand.CanExecute(_thisObject) == false)
                return;
            _thisCommand.Execute(_thisObject);
        }
    }
}