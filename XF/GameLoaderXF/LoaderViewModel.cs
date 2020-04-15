using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

using Xamarin.Forms;
namespace GameLoaderXF
{
    public abstract class LoaderViewModel : Screen, ILoaderVM
    {
        public GamePackageLoaderPickerCP? PackagePicker { get; set; }
        protected IStartUp Starts;
        protected EnumGamePackageMode Mode;
        protected CustomBasicList<string> GameList = new CustomBasicList<string>();
        //protected INavigation Navigation;
        protected IGamePlatform Platform;
        //public cc.Command? ChooseGameCommand { get; set; }
        private string _gameChosen = "";
        public string GameChosen
        {
            get { return _gameChosen; }
            set
            {
                if (SetProperty(ref _gameChosen, value))
                {
                    //if we need something here, rethink.
                    //can decide what to do when property changes
                    //ChooseGameCommand!.ReportCanExecuteChange();
                }
            }
        }



        protected abstract void GenerateGameList();
        public bool CanChoose () => GameChosen != "";
        public abstract void Choose();
        private Task PackagePicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            GameChosen = selectedText;

            return Task.CompletedTask;
        }
        //attempt no navigation.
        public LoaderViewModel()
        {
            Starts = cons!.Resolve<IStartUp>();
            Platform = cons.Resolve<IGamePlatform>();

            //Navigation = navigation;
            Mode = EnumGamePackageMode.Production; //needs some testing for now unfortunately for the loader.
            GenerateGameList();
            PackagePicker = new GamePackageLoaderPickerCP();
            PackagePicker.LoadTextList(GameList);
            PackagePicker.ItemSelectedAsync += PackagePicker_ItemSelectedAsync;
        }
    }
}