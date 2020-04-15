using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.Shells
{
    /// <summary>
    /// this is the starting point for the game package for the shell.
    /// </summary>
    public abstract class BasicGameMainShellView : ContentPage, IUIView
    {

        private readonly IStartUp _start;
        private readonly IStandardScreen _screen;

        protected IGameInfo GameData { get; }
        public BasicData BasicData { get; }
        protected readonly IGamePlatform CustomPlatform;


        protected virtual bool UseSmallerButton => false;

        public BasicGameMainShellView(IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen)
        {

            UIPlatform.ShowMessageAsync = (async message =>
            {
                await DisplayAlert("Message", message, "Okay");
                customPlatform.ResetPopups();
            });
            UIPlatform.ShowError = (async message =>
            {
                await DisplayAlert("Error", message, "Okay");
                UIPlatform.ExitApp();
            });

            NavigationPage.SetHasNavigationBar(this, false);
            GameData = gameData;
            BasicData = basicData;
            _start = start;
            _screen = screen;
            CustomPlatform = customPlatform;
            BuildXAMLAsync().Wait();
        }

        protected async Task BuildXAMLAsync()
        {
            BackgroundColor = Color.Navy;
            Padding = new Thickness(3, 3, 3, 3);
            if (_screen.CanPlay(GameData) == false)
            {
                UIPlatform.ExitApp();
                return; //i know i could not show messagebox.  so it just has to close (because game is not supported for given platform).
            }


            CustomPlatform.SupportedOrientation(GameData);
            OtherCommonButtons(); //not sure if we need but may.
            _start.StartVariables(BasicData); //this is for nick names and any other relevent info needed.
            PrepUI();
            await PopulateUIAsync();
            FinalizeUI();

        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }


        /// <summary>
        /// this is intended for overrided versions to just do the prep work so the main class can do its custom stuff.
        /// </summary>
        protected virtual void PrepUI() { }
        protected virtual void FinalizeUI() { }
        protected abstract Task PopulateUIAsync();

        protected virtual void OtherCommonButtons() { }

        protected async virtual Task TryCloseAsync() { await Task.CompletedTask; UIPlatform.ExitApp(); } //hopefully this simple (?)

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

    }
}
