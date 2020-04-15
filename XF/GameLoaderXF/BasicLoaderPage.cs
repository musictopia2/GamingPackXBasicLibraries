using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Misc;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using System.Reflection;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;

namespace GameLoaderXF
{
    public abstract class BasicLoaderPage : ContentPage, IUIView
        
    {
        protected IGamePlatform CustomPlatform;
        protected IStartUp Starts;
        private readonly IForceOrientation _forces;
        protected override void OnAppearing()
        {
            _forces.ForceOrientation();
            base.OnAppearing();
        }

        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }


        public static bool? Multiplayer { get; set; }

        public int TotalColumns { get; set; }

        protected virtual void StartUp() { }

        Task IUIView.TryCloseAsync()
        {

            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            LoaderViewModel mod = (LoaderViewModel)BindingContext;

            if (mod.PackagePicker!.TextList.Count == 0)
                throw new BasicBlankException("No items was loaded.  Rethink");
            _lists.LoadLists(mod.PackagePicker!);

            MethodInfo fun = mod.GetPrivateMethod(nameof(LoaderViewModel.CanChoose));
            MethodInfo method = mod.GetPrivateMethod(nameof(LoaderViewModel.Choose));
            _button.Command = new ReflectiveCommand(mod, method, fun);

            return Task.CompletedTask;
        }

        //most likely no titles.


        private readonly LoaderStartServerClass? _loadServer;
        private readonly ListChooserXF _lists;
        private readonly Button _button;
        public BasicLoaderPage(IGamePlatform platform, IStartUp starts, IForceOrientation forces, IGamePackageScreen screen)
        {
            _lists = new ListChooserXF();
            if (Multiplayer.HasValue == false)
            {
                throw new BasicBlankException("Must specify whether its single player or not");
            }
            BackgroundColor = Color.Navy; //do this as well
            Starts = starts; //can't test the orientation part because we don't have igameinfo.  has to take some risks.
            _forces = forces;
            CustomPlatform = platform;
            _button = GetSmallerButton("Launch Selected Game", "");
            NavigationPage.SetHasNavigationBar(this, false);
            if (Multiplayer!.Value)
            {
                if (GlobalDataLoaderClass.HasSettings(true) == false)
                {
                    Label label = GetDefaultLabel();
                    label.Text = "You must use the settings app in order to populate the settings so you have at least a nick name";
                    Content = label;
                    return;
                }
                _loadServer = new LoaderStartServerClass(true); //needs same thing for xamarin forms but will be true.
                _loadServer.PossibleStartServer();
            }
            screen.CalculateScreens();
            SendFont(new StandardButtonFontClass());
            StartUp();

            
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _lists.ItemWidth = 170; //try 150.
                _lists.ItemHeight = 20;
                if (TotalColumns == 0)
                    TotalColumns = 3;
            }
            else
            {
                _lists.ItemWidth = 303;
                _lists.ItemHeight = 33;
                if (TotalColumns == 0)
                    TotalColumns = 4; //can always be tweaked as necessary.
            }
            _lists.TotalColumns = TotalColumns; //i forgot this too.
            StackLayout stack = new StackLayout();
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;
            thisScroll.Content = _lists;
            stack.Children.Add(thisScroll); //this is intended to be a first sample.
            

            


            stack.Children.Add(_button);
            Content = stack;
        }
    }
}