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
using System.Windows;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using System.Windows.Media;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFrameworkLibrary.CommonInterfaces;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Shells
{
    /// <summary>
    /// this is the starting point for the game package for the shell.
    /// </summary>
    public abstract class BasicGameMainShellView : Window, IUIView
    {
        private readonly IStartUp _start;

        protected IGameInfo GameData { get; }
        public BasicData BasicData { get; }

        public BasicGameMainShellView(IGameInfo gameData, BasicData basicData, IStartUp start)
        {
            GameData = gameData;
            BasicData = basicData;
            _start = start;
            BuildXAMLAsync().Wait();
        }

        protected Size DefaultWindowSize()
        {
            return new Size(1800, 950);
        }

        protected async Task BuildXAMLAsync()
        {
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            var tempSize = DefaultWindowSize();
            WindowHelper.SetSize(tempSize.Width, tempSize.Height);
            Background = Brushes.Navy;
            OtherCommonButtons(); //not sure if we need but may.
            _start.StartVariables(BasicData); //this is for nick names and any other relevent info needed.
            Show(); //try this way.
            PrepUI();
            await PopulateUIAsync();
            FinalizeUI();
        }
        /// <summary>
        /// this is intended for overrided versions to just do the prep work so the main class can do its custom stuff.
        /// </summary>
        protected virtual void PrepUI() { }
        protected virtual void FinalizeUI() { }
        protected abstract Task PopulateUIAsync();

        protected virtual void OtherCommonButtons() { }

        protected async virtual Task TryCloseAsync() { await Task.CompletedTask; Close(); }

        async Task IUIView.TryCloseAsync()
        {
            await TryCloseAsync();

        }
        //when its a window that is closing, i think it should literally close.  but you can override and tell it no as well.
        protected virtual Task TryActivateAsync() { return Task.CompletedTask; } //its optional but not required.
        Task IUIView.TryActivateAsync()
        {
            return TryActivateAsync();
        }

        //restoring is iffy for now.


    }
}
