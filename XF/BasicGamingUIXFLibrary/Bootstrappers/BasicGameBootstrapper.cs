using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.NetworkingClasses.Misc;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.Views;
using BasicXFControlsAndPages.MVVMFramework.PlatformClasses;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace BasicGamingUIXFLibrary.Bootstrappers
{
    public abstract class BasicGameBootstrapper<TViewModel, TView> : ILoadScreen, IGameBootstrapper, IHandleAsync<SocketErrorEventModel>,
        IHandleAsync<DisconnectEventModel>,
        IHandleAsync<IUIView>
        where TViewModel : IMainGPXShellVM
        where TView : IUIView
    {


        private readonly IStartUp? _startInfo;
        private readonly EnumGamePackageMode _mode;
        protected readonly IGamePlatform CustomPlatform;
        public BasicGameBootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode)
        {
            BeginningColorDimensions.GraphicsHeight = 0;
            BeginningColorDimensions.GraphicsWidth = 0;
            AssemblyLinker.ExtraViewModelLocations.Clear();
            //AssemblyLinker.ExtraViewModelLocations.Add(Assembly.GetCallingAssembly()!); //needs that too.
            AssemblyLinker.ExtraViewLocations.Clear();
            ViewLocator.ManuelVMList.Clear();
            JsonSettingsGlobals.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None; //try this way.  because otherwise, does not work if not everybody is .net core unfortunately.
            JsonSettingsGlobals.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None; //try this as well.  otherwise, gets hosed with .net core and xamarin forms.
            _startInfo = starts;
            _mode = mode;
            CustomPlatform = customPlatform;
            InitalizeAsync(); //maybe its okay to be async void.  doing the other is causing too many problems with the game package.
        }

        /// <summary>
        /// The application.
        /// </summary>
        protected Application? Application { get; set; }

        protected virtual void BeforeLoadingPage(ContentPage page) { }

        bool _isInitialized;





        protected async Task DisplayRootViewForAsync()
        {
            try
            {
                OurContainer!.RegisterType<TViewModel>(true);
                OurContainer!.RegisterType<TView>(true);
                //i think that the shell stuff needs to be singleton.
                //if i am wrong, rethink.
                object item = cons!.Resolve<TViewModel>()!;
                TView view = cons!.Resolve<TView>();
                view.DataContext = item;
                await ShowPageAsync(view, item);
            }
            catch (Exception ex)
            {
                if (TestData!.ShowErrorMessageBoxes == false)
                {
                    CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.VBCompat.Stop(); //so i can see the stacktrace, etc.
                }
                if (ex.InnerException != null)
                {
                    UIPlatform.ShowError(ex.InnerException.Message);
                    return;
                }
                //if we need the stack trace, rethink.


                UIPlatform.ShowError(ex.Message); //at least i get hints.
            }

        }

        //can't do some of the event stuff for the application because its intended to be loaded by something else.


        /// <summary>
        /// Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnExit(object sender, EventArgs e)
        {
        }


        XamarinFormsThread? _thread;
        ViewLocator? _view;
        public async void InitalizeAsync()
        {
            if (_isInitialized)
                return;
            _thread = new XamarinFormsThread();
            _view = new ViewLocator();
            _isInitialized = true;
            UIPlatform.CurrentThread = _thread;
            UIPlatform.ViewLocator = _view;
            UIPlatform.ScreenLoader = this;
            GlobalDelegates.RefreshSubscriptions = (a =>
            {
                a.Subscribe(this);
                UIPlatform.ScreenLoader = this; //this was needed too.
                UIPlatform.ViewLocator = _view;
                UIPlatform.CurrentThread = _thread;

            }); //this is still needed though
            //hopefully this simple.
            await StartRuntimeAsync();

            
        }

        protected BasicData? GameData;
        protected TestOptions? TestData; //this is very important to have too.

        /// <summary>
        /// Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected async Task StartRuntimeAsync()
        {
            if (_mode == EnumGamePackageMode.None)
            {
                //no messageboxes are allowed.
                return;
            }
            Application = Application.Current;
            StartUp();
            OurContainer = new GamePackageDIContainer();
            CustomPlatform.SetUp(OurContainer);
            //IScreen screen = OurContainer.Resolve<IScreen>();
            IGamePackageScreen screen = OurContainer.Resolve<IGamePackageScreen>();
            screen.CalculateScreens(); //try here.
            cons = OurContainer;
            FirstRegister();
            await ConfigureAsync();
            if (_mode == EnumGamePackageMode.Debug)
            {
                await RegisterTestsAsync();
            }
            if (UseMultiplayerProcesses)
            {
                ///MessageBox.Show("Needs to figure out the multiplayer processes");
                //UIPlatform.ExitApp();
                //return;
                OurContainer.RegisterType<BasicMessageProcessing>(true);
                IRegisterNetworks tempnets = OurContainer.Resolve<IRegisterNetworks>();
                tempnets.RegisterMultiplayerClasses(OurContainer);
            }
            LinkViewModelViewAssemblies();
            await DisplayRootViewForAsync(); //has to do here now.
        }

        protected virtual Task RegisterTestsAsync() { return Task.CompletedTask; }

        //we still need this function because something else needs it to decide how to register.
        protected abstract bool UseMultiplayerProcesses { get; }
        protected virtual void StartUp() { }
        private void FirstRegister()
        {
            //OurContainer!.RegisterSingleton(this); //hopefully its now smart enough without having to register for all types.
            //we can't register itself anymore.
            OurContainer!.RegisterSingleton(_startInfo); //because something else is asking for it.

            EventAggregator thisEvent = new EventAggregator();
            thisEvent.Subscribe(this); //maybe this was an issue.
            OurContainer!.RegisterSingleton(thisEvent); //put to list so if anything else needs it, can get from the container.
            CommandContainer thisCommand = new CommandContainer();
            OurContainer.RegisterSingleton(thisCommand);
            OurContainer.RegisterType<NewGameViewModel>(false); //i think we always need a new game view model.
            MiscRegisterFirst();
            //we may even need one for rounds if the game is in rounds.  that will come later.
            //OurContainer.RegisterType<VM>(true);
            OurContainer.RegisterSingleton(OurContainer);
            OurContainer.RegisterSingleton<IAsyncDelayer, AsyncDelayer>(); //for testing, will use a mock version.
            thisEvent.Subscribe(this); //no tags.
            GameData = new BasicData();
            GameData.GamePackageMode = _mode;
            GameData.IsXamarinForms = true; //this is xamarin forms.
            OurContainer.RegisterSingleton(GameData);
            TestData = new TestOptions();
            OurContainer.RegisterSingleton(TestData);
            _startInfo!.RegisterCustomClasses(OurContainer, UseMultiplayerProcesses, GameData); //for now this way.  could change later.
        }

        protected virtual void MiscRegisterFirst() { }

        /// <summary>
        /// if we need custom registrations but still need standard, then override but do the regular functions too.
        /// </summary>
        /// <returns></returns>
        protected abstract Task ConfigureAsync();

        protected GamePackageDIContainer? OurContainer;
        private ContentPage? _mainPage;

        protected async Task ShowPageAsync(IUIView view, object rootModel)
        {
            ContentPage? page = null;
            if (view is ContentPage window1)
            {
                page = window1;


                //await Application!.MainPage.Navigation.PushAsync(page);


                if (_mode == EnumGamePackageMode.Debug)
                {
                    Application!.MainPage = page; //hopefully its this simple (?)
                }
                else
                {
                    await Application!.MainPage.Navigation.PushAsync(page);

                }
                BeforeLoadingPage(page);
                //risk not even register the view now since it should already be done elsewhere.
                //OurContainer!.RegisterSingleton(view); //risked this way.  hopefully i don't regret this.
                _mainPage = page;

            }
            if (rootModel is IScreen screen)
                await screen.ActivateAsync(view);
            //i think that after activate will it set the display bindings.  that way we have every chance possible of doing the bindings.
            if (page != null)
            {
                if (string.IsNullOrEmpty(page.Title) && rootModel is IHaveDisplayName && !ViewModelBinder.HasBinding(page, Page.TitleProperty))
                {
                    //if you don't implement the idisplay, then this will not run.  so you have flexibility of what to pass in for view model.
                    //this allows you to use as little or as much as you want for the windows.

                    var binding = new Binding("DisplayName") { Mode = BindingMode.OneWay }; //i like this way better.
                    page.SetBinding(Page.TitleProperty, binding);
                }
                GamePackageViewModelBinder.Bind(rootModel, page);
            }
        }

        protected virtual bool NeedExtraLocations { get; } = true;
        protected virtual void LinkViewModelViewAssemblies()
        {
            AssemblyLinker.ViewModelAssembly = Assembly.GetAssembly(typeof(TViewModel)); //its more common the view model could be different location but the view is the proper location.
            AssemblyLinker.ViewAssembly = Assembly.GetAssembly(typeof(TView));
            if (NeedExtraLocations)
            {
                AssemblyLinker.ExtraViewModelLocations.Add(Assembly.GetAssembly(typeof(NewGameViewModel))!);
                //AssemblyLinker.ExtraViewModelLocations.Add(Assembly.GetCallingAssembly()!); //needs that too.
                AssemblyLinker.ExtraViewLocations.Add(Assembly.GetAssembly(typeof(NewGameView))!);
            }
        }

        Task ILoadScreen.LoadScreenAsync(object parentViewModel, IUIView parentViewScreen, object childViewModel, IUIView childViewScreen)
        {
            //if (GamePackageViewModelBinder.StopRun)
            //{
            //    return Task.CompletedTask;
            //}
            if (childViewModel is IScreen childScreen && childViewScreen is ContentView childui && parentViewScreen is VisualElement parentui)
            {
                var item = childui.Content;

                GamePackageViewModelBinder.Bind(childViewModel, childui, item);

                GamePackageViewModelBinder.HookParentContainers(parentViewModel, parentui, childScreen, childViewScreen);

            }
            return Task.CompletedTask;
        }

        async Task IHandleAsync<SocketErrorEventModel>.HandleAsync(SocketErrorEventModel message)
        {
            if (_mainPage == null)
            {
                throw new BasicBlankException("No main page was done.  Rethink");
            }
            switch (message.Category)
            {
                case EnumSocketCategory.Client:
                    await _mainPage.DisplayAlert("Client Socket Error", message.Message, "Okay");
                    break;
                case EnumSocketCategory.Server:
                    await _mainPage.DisplayAlert("Server Socket Error", message.Message, "Okay");
                    break;
                default:
                    await PrivateShowErrorAsync(message.Message);
                    break;
            }
        }

        private async Task PrivateShowErrorAsync(string message)
        {
            if (_mainPage == null)
            {
                throw new BasicBlankException("No main page was done.  Rethink");
            }
            await _mainPage.DisplayAlert("Error", message, "okay");
            UIPlatform.ExitApp();
        }

        Task IHandleAsync<DisconnectEventModel>.HandleAsync(DisconnectEventModel message)
        {
            UIPlatform.ShowError("Disconnected");
            return Task.CompletedTask;
        }
        async Task IHandleAsync<IUIView>.HandleAsync(IUIView message)
        {
            await Task.CompletedTask;
            //await Task.Delay(50);
            if (!(message is View control))
            {
                return;
            }




            //DependencyObject nextItem;
            //if (control.Content is ContentControl content)
            //{
            //    nextItem = (DependencyObject)content.Content;
            //}
            //else if (control.Content is Grid grid)
            //{
            //    //int count = VisualTreeHelper.GetChildrenCount(grid);
            //    //var xx = control.FindName("ChangeFlag");
            //    //GamePackageViewModelBinder.StepThrough = true;
            //    nextItem = grid;
            //    //VisualTreeHelper.GetChildrenCount(depObj); i++)
            //    //throw new NotImplementedException();
            //}
            //else
            //{
            //    nextItem = (DependencyObject)control.Content;
            //}
            //ContentControl content = (ContentControl)control.Content;

            //var 
            //count = VisualTreeHelper.GetChildrenCount(nextItem);

            //needs more complex when its rebinding.

            //hopefully no need for the content.   could be very iffy.  lots of rethinking could be required.  including the manuellist.


            GamePackageViewModelBinder.Bind(message.DataContext, control); //this can rescan the bindings.  hopefully no problem.
            //GamePackageViewModelBinder.ManuelElements.Clear(); //hopefully won't hurt anything if i clear out.




        }

    }
}
