using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using cc = CommonBasicStandardLibraries.MVVMHelpers;
namespace GameLoaderXF
{
    public abstract class LoaderViewModel : BaseViewModel, ILoaderVM
    {
        public GamePackageLoaderPickerCP? PackagePicker { get; set; }
        protected IStartUp? Starts;
        protected EnumGamePackageMode Mode;
        protected CustomBasicList<string> GameList = new CustomBasicList<string>();
        protected INavigation? Navigation;
        protected IGamePlatform? Platform;
        public cc.Command? ChooseGameCommand { get; set; }
        private string _GameChosen = "";
        public string GameChosen
        {
            get { return _GameChosen; }
            set
            {
                if (SetProperty(ref _GameChosen, value))
                {
                    //can decide what to do when property changes
                    ChooseGameCommand!.ReportCanExecuteChange();
                }
            }
        }
        public void Init(IGamePlatform platform, IStartUp starts, INavigation navigation)
        {
            Starts = starts;
            Platform = platform; //needed for something else.
            Navigation = navigation;
            Mode = EnumGamePackageMode.Production; //until i have production ready.
            GenerateGameList();
            PackagePicker = new GamePackageLoaderPickerCP();
            PackagePicker.LoadTextList(GameList);
            PackagePicker.ItemSelectedAsync += PackagePicker_ItemSelectedAsync;
            ChooseGameCommand = new cc.Command(async items =>
            {
                await ChooseAsync();
            }, items => GameChosen != "", this);
        }
        protected abstract void GenerateGameList();
        protected abstract Task ChooseAsync();
        private Task PackagePicker_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            GameChosen = selectedText;
            return Task.CompletedTask;
        }
        public LoaderViewModel() { }
    }
}