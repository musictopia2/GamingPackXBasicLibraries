using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFrameworkLibrary.NetworkingClasses.Misc;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace GameLoaderWPF
{
    public abstract class BasicLoaderPage : Window, IUIView
    {
        public int TotalColumns { get; set; }
        protected virtual void StartUp() { }

        protected virtual Size DefaultWindowSize()
        {
            return new Size(1800, 950);
        }

        Task IUIView.TryCloseAsync()
        {
            Close();
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            ILoaderVM mod = (ILoaderVM)DataContext;
            if (mod.PackagePicker!.TextList.Count == 0)
                throw new BasicBlankException("No items");
            _list.LoadLists(mod.PackagePicker!);
            DisplayProcesses();
            return Task.CompletedTask;
        }

        protected virtual void DisplayProcesses()
        {

        }

        private readonly LoaderStartServerClass? _loadServer;
        //eventually need to do without the separate app for settings.  but not quite yet.
        public static bool? Multiplayer { get; set; }
        private readonly ListChooserWPF _list = new ListChooserWPF();
        public BasicLoaderPage()
        {
            if (Multiplayer.HasValue == false)
            {
                throw new BasicBlankException("Needs to set whether its multiplayer");
            }
            Background = Brushes.Navy;
            if (Multiplayer!.Value)
            {
                if (GlobalDataLoaderClass.HasSettings(false) == false)
                {
                    TextBlock label = GetDefaultLabel();
                    label.Text = "You must use the settings app in order to populate the settings so you have at least a nick name";
                    Content = label;
                    Show(); //looks like i have to do the show method.  otherwise, nothing.
                    return;
                }
                _loadServer = new LoaderStartServerClass(false); //needs same thing for xamarin forms but will be true.
                _loadServer.PossibleStartServer(); //problem is here.
            }
            StartUp();
            OS = EnumOS.WindowsDT; //this part is okay.
            var tempSize = DefaultWindowSize();
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            WindowHelper.SetSize(tempSize.Width, tempSize.Height);

            _list.Orientation = Orientation.Horizontal;
            _list.ItemWidth += 50;
            if (TotalColumns == 0)
                TotalColumns = 5;
            _list.TotalColumns = TotalColumns; //this should be fine so i can test something else.

            Content = _list;
            Show();
        }

    }
}