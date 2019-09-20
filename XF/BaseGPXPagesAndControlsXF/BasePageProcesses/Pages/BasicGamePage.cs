using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.NetworkingClasses.Misc;
using BasicGameFramework.SimpleMiscClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXPagesAndControlsXF.BasePageProcesses.Pages
{
    public enum EnumOrientation
    {
        Landscape, Portrait
    }
    public abstract class BasicGamePage<VM> : ContentPage, ISimpleUI,
        IHandleAsync<LoadEventModel>,
        IHandleAsync<UpdateEventModel>,
        IHandleAsync<SocketErrorEventModel>,
        IHandleAsync<DisconnectEventModel>
        where VM : BaseViewModel, IBasicGameVM //somehow did not need new
    {
        protected VM? ThisMod;
        protected GamePackageDIContainer OurContainer;
        protected readonly IGamePlatform CustomPlatform;
        protected IGameInfo? ThisGame;
        protected IStandardScreen? Screen;
        private readonly IStartUp _startInfo;
        public void MakeGameButtonSmaller(Button smallerButton)
        {
            GameButton!.FontSize = smallerButton.FontSize;
            GameButton.HeightRequest = smallerButton.HeightRequest;
        }
        public BasicGamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) //i think that this will directly do the buildxaml now.
        {
            NavigationPage.SetHasNavigationBar(this, false);
            OurContainer = new GamePackageDIContainer();
            _startInfo = starts; //something needs to have this too.
            CustomPlatform = customPlatform;
            CustomPlatform.SetUp(OurContainer); //this can get them setup too.
            BuildXAML(mode);
        }
        protected virtual void StartUp() { }
        protected virtual void AfterCreateViewModel() { } //most of the time, it was not needed
        protected abstract void RegisterInterfaces(); //this is where you do all the registrations.
        protected BasicData? ThisData; //i think this will become more important.
        protected TestOptions? ThisTest; //this is very important to have too.
        protected virtual void RegisterTests() { }
        protected Button? GameButton; //this worked out great.
        private async void BuildXAML(EnumGamePackageMode mode)
        {
            BackgroundColor = Color.Navy;
            Padding = new Thickness(3, 3, 3, 3);
            StartUp();
            cons = OurContainer;
            FirstRegister(mode);
            RegisterInterfaces();
            RegisterTests();
            _startInfo.StartVariables(ThisData!);
            if (UseMultiplayerProcesses) //you have to register the classes before doing the view model.
            {
                OurContainer.RegisterType<BasicMessageProcessing>(true);
                IRegisterNetworks tempnets = OurContainer.Resolve<IRegisterNetworks>();
                tempnets.RegisterMultiplayerClasses(OurContainer);
            }
            ThisMod = cons.Resolve<VM>(); //i think
            BindingContext = ThisMod;
            SetBinding(TitleProperty, new Binding(nameof(BaseViewModel.Title)));
            ThisMod.Init(); //sometimes its needed for more complex setups.
            ThisGame = cons.Resolve<IGameInfo>();
            Screen = OurContainer.Resolve<IStandardScreen>();
            if (Screen.CanPlay(ThisGame) == false)
            {
                CustomPlatform.CloseApp();
                return; //i know i could not show messagebox.  so it just has to close (because game is not supported for given platform).
            }
            AfterCreateViewModel();
            CustomPlatform.SupportedOrientation(ThisGame);
            if (UseSmallerButton == false)
                GameButton = GetGamingButton("New Game", nameof(IBasicGameVM.NewGameCommand));
            else
                GameButton = GetSmallerButton("New Game", nameof(IBasicGameVM.NewGameCommand));
            GameButton.SetBinding(IsVisibleProperty, new Binding(nameof(IBasicGameVM.NewGameVisible)));
            OtherCommonButtons();
            await AfterGameButtonAsync();
        }
        protected virtual bool UseSmallerButton => false;
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
            ThisData.IsXamarinForms = true; //in this case, its xamarin forms.  so anything that needs to know its xamarin forms can act accordingly.
            ThisData.GamePackageMode = mode;
            OurContainer.RegisterSingleton(ThisData);
            ThisTest = new TestOptions();
            OurContainer.RegisterSingleton(ThisTest);
            _startInfo.RegisterCustomClasses(OurContainer, UseMultiplayerProcesses, ThisData);
        }
        protected virtual void OtherCommonButtons() { }
        protected abstract Task AfterGameButtonAsync(); //this most common has some async stuff.  might as well have it here.
        void IClose.CloseProgram()
        {
            CustomPlatform.CloseApp();
        }
        async void IError.ShowError(string message)
        {
            await PrivateShowErrorAsync(message);
        }
        private async Task PrivateShowErrorAsync(string message)
        {
            await DisplayAlert("Error", message, "okay");
            CustomPlatform.CloseApp();
        }
        async Task IMessage.ShowMessageBox(string message)
        {
            await DisplayAlert(Title, message, "Okay");
            CustomPlatform.ResetPopups();
        }
        public async Task HandleAsync(SocketErrorEventModel message)
        {
            switch (message.Category)
            {
                case EnumSocketCategory.Client:
                    await DisplayAlert("Client Socket Error", message.Message, "Okay");
                    break;
                case EnumSocketCategory.Server:
                    await DisplayAlert("Server Socket Error", message.Message, "Okay");
                    break;
                default:
                    await PrivateShowErrorAsync(message.Message);
                    break;
            }
        }
        public async Task HandleAsync(DisconnectEventModel Message)
        {
            await DisplayAlert("Game Package", "Disconnected", "Okay");
            CustomPlatform.CloseApp();
        }
        public abstract Task HandleAsync(LoadEventModel message); //this is for the information being loaded.
        public abstract Task HandleAsync(UpdateEventModel message);
        protected RestoreVM? ThisRestore;
        protected void AddVerticalLabelGroup(string header, string bindingPath, StackLayout thisStack, bool boldHeader = false)
        {
            Label thisLabel = GetDefaultLabel();
            thisLabel.Text = $"{header}:";
            if (boldHeader == true)
                thisLabel.FontAttributes = FontAttributes.Bold;
            thisStack.Children.Add(thisLabel);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(Label.TextProperty, bindingPath); //unfortunately, this has no ability to wrap.  has to see what i did before in order to resolve this.
            thisStack.Children.Add(thisLabel);
        }
        protected void AddRestoreCommand(StackLayout thisStack)
        {
            if (ThisRestore == null)
                ThisRestore = OurContainer!.Resolve<RestoreVM>();
            Button thisBut = GetGamingButton("Restore Game", nameof(RestoreVM.RestoreCommand));
            thisBut.BindingContext = ThisRestore;
            thisBut.SetBinding(IsVisibleProperty, new Binding(nameof(RestoreVM.Visible)));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(thisBut); //we can't set the context to the stack because other things can be on it.
        }
    }
}