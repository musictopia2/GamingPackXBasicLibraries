using SkiaSharp.Views.Forms;
using System.Windows.Input;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.SimpleControls
{
    public class RowClickerXF : ContentView
    {
        public ICommand? Command;
        private readonly SKCanvasView _thisControl;
        public RowClickerXF()
        {
            _thisControl = new SKCanvasView();
            _thisControl.EnableTouchEvents = true;
            _thisControl.Touch += Touch;
            Content = _thisControl;
        }
        private void Touch(object sender, SKTouchEventArgs e)
        {
            if (Command == null || BindingContext == null)
                return; //you have to have command and the binding context.
            if (Command.CanExecute(BindingContext) == false)
                return;
            Command.Execute(BindingContext);
        }
    }
}