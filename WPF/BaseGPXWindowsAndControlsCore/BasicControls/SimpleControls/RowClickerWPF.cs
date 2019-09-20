using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
namespace BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls
{
    public class RowClickerWPF : UserControl
    {
        public ICommand? Command; // no need for bindings for this one.  also no need for commandparameter since it will be the bindingcontext.
        public RowClickerWPF()
        {
            Grid thisGrid = new Grid();
            thisGrid.Background = Brushes.Transparent;
            Content = thisGrid;
            MouseUp += RowClickerWPF_MouseUp;
        } // no need for init for this one.
        private void RowClickerWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Command == null)
                return;// because no command
            if (DataContext == null)
                return;// because no data context
            if (Command.CanExecute(DataContext) == false)
                return;
            Command.Execute(DataContext);
        }
    }
}