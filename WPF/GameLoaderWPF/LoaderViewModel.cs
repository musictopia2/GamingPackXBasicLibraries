using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Bootstrappers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace GameLoaderWPF
{
    public abstract class LoaderViewModel : Screen, ILoaderVM
    {
        public GamePackageLoaderPickerCP? PackagePicker { get; set; }
        //protected Window? Window;
        protected IStartUp Starts;
        protected EnumGamePackageMode Mode;
        protected CustomBasicList<string> GameList = new CustomBasicList<string>();

        public LoaderViewModel()
        {
            Starts = cons!.Resolve<IStartUp>();
            //BasicLoaderPage.Multiplayer = Multiplayer;
            GenerateGameList();
            PackagePicker = new GamePackageLoaderPickerCP();
            PackagePicker.LoadTextList(GameList);
            Mode = EnumGamePackageMode.Production; //can now test the production processes.
            PackagePicker.ItemSelectedAsync += PackagePicker_ItemSelectedAsync;
        }

        protected override Task ActivateAsync()
        {



            return base.ActivateAsync();
        }


        protected abstract void GenerateGameList();

        //protected abstract bool Multiplayer { get; }
        protected abstract IGameBootstrapper ChooseGame(string gameChosen);

        //protected abstract Window ChooseGame(string gameChosen);
        private async Task PackagePicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {

            IGameBootstrapper _ = ChooseGame(selectedText); //just needs it period.
            await CloseViewAsync();
        }
    }
}