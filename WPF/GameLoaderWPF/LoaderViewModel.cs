using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks;
using System.Windows;
namespace GameLoaderWPF
{
    public abstract class LoaderViewModel : BaseViewModel, ILoaderVM
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
        protected abstract Window ChooseGame(string gameChosen);
        private Task PackagePicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {

            Window newPage = ChooseGame(selectedText);
            Window!.Close();
            newPage.Show();
            return Task.CompletedTask;
        }
    }
}