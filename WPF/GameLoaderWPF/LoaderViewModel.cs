using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Bootstrappers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using System.Windows;
namespace GameLoaderWPF
{
    public abstract class LoaderViewModel : Screen, ILoaderVM
    {
        public GamePackageLoaderPickerCP? PackagePicker { get; set; }
        protected Window? Window;
        protected IStartUp? Starts;
        protected EnumGamePackageMode Mode;
        protected CustomBasicList<string> GameList = new CustomBasicList<string>();
        public void Init(Window window, IStartUp starts)
        {
            Window = window;
            GenerateGameList();
            PackagePicker = new GamePackageLoaderPickerCP();
            PackagePicker.LoadTextList(GameList);
            PackagePicker.ItemSelectedAsync += PackagePicker_ItemSelectedAsync;
            Starts = starts;
            Mode = EnumGamePackageMode.Production; //can now test the production processes.
        }
        protected abstract void GenerateGameList();


        protected abstract IGameBootstrapper ChooseGame(string selectedText);

        //protected abstract Window ChooseGame(string gameChosen);
        private Task PackagePicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {

            IGameBootstrapper _ = ChooseGame(selectedText); //just needs it period.
            Window!.Close();
            
            return Task.CompletedTask;
        }
    }
}