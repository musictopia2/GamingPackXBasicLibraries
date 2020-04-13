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

namespace GameLoaderXF
{
    public abstract class BasicLoaderPage<VM> : ContentPage
        where VM : ILoaderVM, new()
    {
        protected IGamePlatform CustomPlatform;
        protected IStartUp Starts;
        private readonly IForceOrientation _forces;
        protected override void OnAppearing()
        {
            _forces.ForceOrientation();
            base.OnAppearing();
        }
        public int TotalColumns { get; set; } 
        protected virtual void StartUp() { }
        private readonly LoaderStartServerClass? _loadServer;
        public BasicLoaderPage(IGamePlatform platform, IStartUp starts, IForceOrientation forces, IGamePackageScreen screen, bool multiPlayer)
        {
            BackgroundColor = Color.Navy; //do this as well
            Starts = starts; //can't test the orientation part because we don't have igameinfo.  has to take some risks.
            _forces = forces;
            CustomPlatform = platform;
            NavigationPage.SetHasNavigationBar(this, false);
            if (multiPlayer)
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
            VM thisMod = new VM();
            StartUp();
            BindingContext = thisMod;
            thisMod.Init(platform, starts, Navigation);
            ListChooserXF lists = new ListChooserXF();
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                lists.ItemWidth = 170; //try 150.
                lists.ItemHeight = 20;
                if (TotalColumns == 0)
                    TotalColumns = 3;
            }
            else
            {
                lists.ItemWidth = 303;
                lists.ItemHeight = 33;
                if (TotalColumns == 0)
                    TotalColumns = 4; //can always be tweaked as necessary.
            }
            if (thisMod.PackagePicker!.TextList.Count == 0)
                throw new BasicBlankException("No items was loaded.  Rethink");
            lists.TotalColumns = TotalColumns; //i forgot this too.
            lists.LoadLists(thisMod.PackagePicker!);
            StackLayout stack = new StackLayout();
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;
            thisScroll.Content = lists;
            stack.Children.Add(thisScroll); //this is intended to be a first sample.
            Button button = GetSmallerButton("Launch Selected Game", nameof(LoaderViewModel.ChooseAsync));
            stack.Children.Add(button);
            Content = stack;
        }
    }
}