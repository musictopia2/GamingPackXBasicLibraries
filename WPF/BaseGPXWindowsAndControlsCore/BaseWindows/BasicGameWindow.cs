using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.NetworkingClasses.Misc;
using BasicGameFramework.SimpleMiscClasses;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BaseWindows
{
    public abstract class BasicGameWindow<VM> : Window, ISimpleUI
        , IHandleAsync<LoadEventModel>,
        IHandleAsync<UpdateEventModel>,
        IHandleAsync<SocketErrorEventModel>,
        IHandleAsync<DisconnectEventModel>
        where VM : BaseViewModel, IBasicGameVM
    {
        protected VM? ThisMod;
        public void CloseProgram()
        {
            Cursor = Cursors.Arrow;
            Application.Current.Shutdown(); //hopefully this works as well.
        }
        public virtual void ShowError(string message)
        {
            MessageBox.Show(message);
            CloseProgram();
        }
        public Task ShowMessageBox(string message)
        {
            MessageBox.Show(message);
            return Task.CompletedTask;
        }

        protected Button? GameButton; //this worked out great.
        protected virtual void AfterCreateViewModel() { } //most of the time, it was not needed
        protected abstract void RegisterInterfaces(); //this is where you do all the registrations.
        protected GamePackageDIContainer? OurContainer;
        protected IPersonalSetting ThisPersonal = new BlankPersonal();
        protected IGameInfo? ThisGame;
        protected BasicData? ThisData; //i think this will become more important.
        protected TestOptions? ThisTest; //this is very important to have too.
        protected virtual void StartUp() { }

        protected virtual Size DefaultWindowSize()
        {
            return new Size(1800, 950);
        }
        protected virtual void RegisterTests() { }
        private IStartUp? _startInfo;
        protected void BuildXAML(IStartUp starts, EnumGamePackageMode mode)
        {
            _startInfo = starts;
            if (mode == EnumGamePackageMode.None)
                throw new BasicBlankException("Must choose either debug or production for game package modes");
            OurContainer = new GamePackageDIContainer();
            StartUp();
            OS = EnumOS.WindowsDT; //this part is okay.
            cons = OurContainer;
            ShowCurrentUser();
            FirstRegister(mode);
            RegisterInterfaces();
            if (mode == EnumGamePackageMode.Debug)
                RegisterTests(); //if not debug, then can't register tests period.
            if (UseMultiplayerProcesses)
            {
                OurContainer.RegisterType<BasicMessageProcessing>(true);
                IRegisterNetworks tempnets = OurContainer.Resolve<IRegisterNetworks>();
                tempnets.RegisterMultiplayerClasses(OurContainer);
            }
            ThisMod = cons.Resolve<VM>(); //i think
            ThisMod.Init(); //sometimes its needed for more complex setups.
            ThisGame = cons.Resolve<IGameInfo>();
            AfterCreateViewModel();
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            WindowHelper.SetTitleBindings(ThisMod);
            var TempSize = DefaultWindowSize();
            WindowHelper.SetSize(TempSize.Width, TempSize.Height);
            Background = Brushes.Navy;
            Title = ThisGame.GameName;
            GameButton = GetGamingButton("New Game", nameof(IBasicGameVM.NewGameCommand));
            GameButton.SetBinding(VisibilityProperty, GetVisibleBinding(nameof(IBasicGameVM.NewGameVisible)));
            OtherCommonButtons();
            starts.StartVariables(ThisData!); //this is for nick names and any other relevent info needed.
            Show(); //try this way.
            AfterGameButton();
        }
        protected virtual void OtherCommonButtons() { }
        protected abstract void AfterGameButton();
        protected void ShowCurrentUser()
        {
            ThisPersonal.ShowCurrentUser(); //instead of overriding, just create a new behavior.
        }
        protected abstract bool UseMultiplayerProcesses { get; } //games like tic tac toe would be true even if you are doing pass and play.  this influences wh
        private void FirstRegister(EnumGamePackageMode mode)
        {
            OurContainer!.RegisterSingleton(this); //hopefully its now smart enough without having to register for all types.
            EventAggregator ThisEvent = new EventAggregator();
            OurContainer.RegisterSingleton(ThisEvent); //put to list so if anything else needs it, can get from the container.
            CommandContainer ThisCommand = new CommandContainer();
            OurContainer.RegisterSingleton(ThisCommand);
            OurContainer.RegisterType<VM>(true);
            OurContainer.RegisterSingleton(OurContainer);
            OurContainer.RegisterSingleton<IAsyncDelayer, AsyncDelayer>(); //for testing, will use a mock version.
            ThisEvent.Subscribe(this); //no tags.
            ThisData = new BasicData();
            ThisData.GamePackageMode = mode;
            OurContainer.RegisterSingleton(ThisData);
            ThisTest = new TestOptions();
            OurContainer.RegisterSingleton(ThisTest);
            _startInfo!.RegisterCustomClasses(OurContainer, UseMultiplayerProcesses, ThisData); //for now this way.  could change later.
        }
        public abstract Task HandleAsync(LoadEventModel message); //this is for the information being loaded.
        Task IHandleAsync<SocketErrorEventModel>.HandleAsync(SocketErrorEventModel message)
        {
            if (message.Category == EnumSocketCategory.Client)
                MessageBox.Show($"Client Socket Error. The message was {message.Message}");
            else if (message.Category == EnumSocketCategory.Server)
                MessageBox.Show($"Server Socket Error. The message was {message.Message}");
            else
                ShowError("No Category Found For Socket Error");
            return Task.CompletedTask;
        }
        Task IHandleAsync<DisconnectEventModel>.HandleAsync(DisconnectEventModel message)
        {
            MessageBox.Show("Disconnected");
            CloseProgram();
            return Task.CompletedTask;
        }
        protected void AddVerticalLabelGroup(string header, string bindingPath, StackPanel thisStack, bool boldHeader = false)
        {
            TextBlock thisLabel = GetDefaultLabel();
            thisLabel.Text = $"{header}:";
            if (boldHeader == true)
                thisLabel.FontWeight = FontWeights.Bold;
            thisStack.Children.Add(thisLabel);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(TextBlock.TextProperty, bindingPath);
            thisLabel.TextWrapping = TextWrapping.Wrap;
            thisStack.Children.Add(thisLabel);
        }
        public abstract Task HandleAsync(UpdateEventModel message);
        protected RestoreVM? ThisRestore;
        protected void AddRestoreCommand(StackPanel thisStack)
        {
            if (ThisRestore == null)
                ThisRestore = OurContainer!.Resolve<RestoreVM>();
            Button thisBut = GetGamingButton("Restore Game", nameof(RestoreVM.RestoreCommand));
            thisBut.DataContext = ThisRestore;
            thisBut.SetBinding(VisibilityProperty, GetVisibleBinding(nameof(RestoreVM.Visible)));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(thisBut); //we can't set the context to the stack because other things can be on it.
        }
    }
}