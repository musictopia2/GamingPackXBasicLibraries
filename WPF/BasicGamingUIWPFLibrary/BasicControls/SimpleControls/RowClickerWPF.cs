using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.SimpleControls
{
    public class RowClickerWPF : GraphicsCommand
    {
        //public ICommand? Command; // no need for bindings for this one.  also no need for commandparameter since it will be the bindingcontext.
        public RowClickerWPF()
        {
            Grid thisGrid = new Grid();
            thisGrid.Background = Brushes.Transparent;
            Content = thisGrid;
            //MouseUp += RowClickerWPF_MouseUp;
        } // no need for init for this one.
        //private void RowClickerWPF_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (Command == null)
        //        return;// because no command
        //    if (DataContext == null)
        //        return;// because no data context
        //    if (Command.CanExecute(DataContext) == false)
        //        return;
        //    Command.Execute(DataContext);
        //}
    }
}
