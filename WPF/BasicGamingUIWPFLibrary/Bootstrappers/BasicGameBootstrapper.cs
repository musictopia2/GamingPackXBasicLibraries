using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicControlsAndWindowsCore.MVVMFramework.PlatformClasses;
using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.NetworkingClasses.Misc;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Helpers;
using BasicGamingUIWPFLibrary.Views;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Bootstrappers
{
    //this is just the beginning.
    //its intended to now actually use a loader with it (since i was able to prove it works)
    public abstract class BasicGameBootstrapper<TViewModel, TView> : ILoadScreen, IGameBootstrapper, IHandleAsync<SocketErrorEventModel>,
        IHandleAsync<DisconnectEventModel>,
        IHandleAsync<IUIView>
        where TViewModel : IMainGPXShellVM
        where TView : IUIView
    {
        //no need for new because it does it differently anyways.

        private readonly IStartUp? _startInfo;
        private readonly EnumGamePackageMode _mode;
        public BasicGameBootstrapper(IStartUp starts, EnumGamePackageMode mode)
        {
            BeginningColorDimensions.GraphicsHeight = 0;
            BeginningColorDimensions.GraphicsWidth = 0;
            JsonSettingsGlobals.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None; //try this way.  because otherwise, does not work if not everybody is .net core unfortunately.
            JsonSettingsGlobals.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None; //try this as well.  otherwise, gets hosed with .net core and xamarin forms.
            _startInfo = starts;
            _mode = mode;
            InitalizeAsync(); //maybe its okay to be async void.  doing the other is causing too many problems with the game package.
        }

        /// <summary>
        /// The application.
        /// </summary>
        protected Application? Application { get; set; }

        protected virtual void BeforeLoadingWindow(Window window) { }

        bool _isInitialized;

        private void CloseApp()
        {
            Application.Current.Shutdown();
        }

        // <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {

            Application!.DispatcherUnhandledException += OnUnhandledException;

            Application.Exit += OnExit;
        }

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
                await ShowWindowAsync(view, item);
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

        /// <summary>
        /// Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show(e.Exception.Message); //that should be default.
            //also figure out how to close as well.

            //try to ignore this for now.
        }



        public async void InitalizeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            UIPlatform.CurrentThread = new WPFThread(); //always has to use that one.
            UIPlatform.ViewLocator = new ViewLocator(); //most of the time has to use real.  otherwise, games like yahtzee does not work on desktop.
            UIPlatform.ScreenLoader = this;

            UIPlatform.ShowMessageAsync = (message) =>
            {
                if (TestData!.NoCommonMessages)
                {
                    Console.WriteLine(message);
                    return Task.CompletedTask;
                }
                MessageBox.Show(message);
                return Task.CompletedTask;
            };
            UIPlatform.DesktopValidationError = (message) =>
            {
                MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            UIPlatform.ExitApp = CloseApp;
            UIPlatform.ShowError = (message) =>
            {
                MessageBox.Show(message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error); //let the user know its an error
                CloseApp();
            };
            GlobalDelegates.RefreshSubscriptions = (a =>
            {
                a.Subscribe(this);
            });
            await StartRuntimeAsync();

            //await ConfigureAsync();
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
                MessageBox.Show("The mode must be debug or production.  Therefore, closing out");
                CloseApp();
                return;
            }
            Application = Application.Current;
            StartUp();
            OS = EnumOS.WindowsDT;
            SetPersonalSettings(); //i think
            PrepareApplication();
            OurContainer = new GamePackageDIContainer();
            cons = OurContainer;
            ShowCurrentUser();
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

        protected IPersonalSetting ThisPersonal = new BlankPersonal(); //was going to force it to be done but chose not to.
        protected GamePackageDIContainer? OurContainer;
        protected void ShowCurrentUser()
        {
            ThisPersonal.ShowCurrentUser(); //instead of overriding, just create a new behavior.
        }

        protected virtual void SetPersonalSettings() { }

        protected async Task ShowWindowAsync(IUIView view, object rootModel)
        {
            Window? window = null;
            if (view is Window window1)
            {
                window = window1;
                BeforeLoadingWindow(window);
                Application!.MainWindow = window;
                //do work with the titles.
                if (view is IUISetting ui)
                {
                    OurContainer!.RegisterSingleton(ui);
                }
                window.Show();

            }
            if (rootModel is IScreen screen)
                await screen.ActivateAsync(view);
            //i think that after activate will it set the display bindings.  that way we have every chance possible of doing the bindings.
            if (window != null)
            {
                if (string.IsNullOrEmpty(window.Title) && rootModel is IHaveDisplayName && !ViewModelBinder.HasBinding(window, Window.TitleProperty))
                {
                    //if you don't implement the idisplay, then this will not run.  so you have flexibility of what to pass in for view model.
                    //this allows you to use as little or as much as you want for the windows.

                    var binding = new Binding("DisplayName") { Mode = BindingMode.OneWay }; //i like this way better.
                    window.SetBinding(Window.TitleProperty, binding);
                }
                //ViewModelBinder.Bind(childViewModel, childui, (DependencyObject)item);
                GamePackageViewModelBinder.Bind(rootModel, window, (DependencyObject)window.Content);
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
            if (childViewModel is IScreen childScreen && childViewScreen is UserControl childui && parentViewScreen is UIElement parentui)
            {
                var item = childui.Content;

                GamePackageViewModelBinder.Bind(childViewModel, childui, (DependencyObject)item);

                GamePackageViewModelBinder.HookParentContainers(parentViewModel, parentui, childScreen, childViewScreen);

            }
            return Task.CompletedTask;
        }

        Task IHandleAsync<SocketErrorEventModel>.HandleAsync(SocketErrorEventModel message)
        {
            if (message.Category == EnumSocketCategory.Client)
            {
                MessageBox.Show($"Client Socket Error. The message was {message.Message}");
            }
            else if (message.Category == EnumSocketCategory.Server)
            {
                MessageBox.Show($"Server Socket Error. The message was {message.Message}");
            }
            else
            {
                UIPlatform.ShowError("No Category Found For Socket Error");
            }
            return Task.CompletedTask;
        }

        Task IHandleAsync<DisconnectEventModel>.HandleAsync(DisconnectEventModel message)
        {
            MessageBox.Show("Disconnected");
            CloseApp();
            return Task.CompletedTask;
        }
        async Task IHandleAsync<IUIView>.HandleAsync(IUIView message)
        {
            await Task.CompletedTask;
            //await Task.Delay(50);
            if (!(message is UserControl control))
            {
                return;
            }
            //GamePackageViewModelBinder.StepThrough = false;
            //if (!(message ))
            //childViewScreen is UserControl childui
            //var item = (DependencyObject) control.Content;
            //VisualTreeHelper.GetChildrenCount(depObj)

            //var count = VisualTreeHelper.GetChildrenCount(item);
            //well see how iffy this is.
            DependencyObject nextItem;
            if (control.Content is ContentControl content)
            {
                nextItem = (DependencyObject)content.Content;
            }
            else if (control.Content is Grid grid)
            {
                //int count = VisualTreeHelper.GetChildrenCount(grid);
                //var xx = control.FindName("ChangeFlag");
                //GamePackageViewModelBinder.StepThrough = true;
                nextItem = grid;
                //VisualTreeHelper.GetChildrenCount(depObj); i++)
                //throw new NotImplementedException();
            }
            else
            {
                nextItem = (DependencyObject)control.Content;
            }
            //ContentControl content = (ContentControl)control.Content;

            //var 
            //count = VisualTreeHelper.GetChildrenCount(nextItem);

            //needs more complex when its rebinding.


            GamePackageViewModelBinder.Bind(message.DataContext, control, nextItem); //this can rescan the bindings.  hopefully no problem.
            GamePackageViewModelBinder.ManuelElements.Clear(); //hopefully won't hurt anything if i clear out.
            

            
            
        }
    }
}