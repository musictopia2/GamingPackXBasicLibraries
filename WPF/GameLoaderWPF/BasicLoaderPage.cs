using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using BasicGameFramework.NetworkingClasses.Misc;

namespace GameLoaderWPF
{
    public abstract class BasicLoaderPage<VM> : Window
        where VM : ILoaderVM, new()
    {
        public int TotalColumns { get; set; }
        protected virtual void StartUp() { }
        protected bool Multiplayer { get; }
        protected virtual Size DefaultWindowSize()
        {
            return new Size(1800, 950);
        }
        private readonly LoaderStartServerClass? _loadServer;
        public BasicLoaderPage(IStartUp starts, bool multiplayer)
        {
            Background = Brushes.Navy;
            if (multiplayer)
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
                _loadServer.PossibleStartServer();
            }
            Multiplayer = multiplayer;
            StartUp();
            OS = EnumOS.WindowsDT; //this part is okay.
            var tempSize = DefaultWindowSize();
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            WindowHelper.SetSize(tempSize.Width, tempSize.Height);
            VM thisMod = new VM();
            thisMod.Init(this, starts);
            ListChooserWPF lists = new ListChooserWPF();
            lists.Orientation = Orientation.Horizontal;
            lists.ItemWidth += 50;
            if (TotalColumns == 0)
                TotalColumns = 5;
            lists.TotalColumns = TotalColumns; //this should be fine so i can test something else.
            if (thisMod.PackagePicker!.TextList.Count == 0)
                throw new BasicBlankException("No items");
            lists.LoadLists(thisMod.PackagePicker!);
            Content = lists;
            Show();
        }
    }
}