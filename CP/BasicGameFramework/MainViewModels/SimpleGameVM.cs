using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks;
using static BasicGameFramework.ViewModelInterfaces.BasicViewModelHelpers;
namespace BasicGameFramework.MainViewModels
{
    public abstract class SimpleGameVM : BaseViewModel, IBasicGameVM, IBasicEnableProcess, ISinglePlayerVM
    {
        private bool _NewGameVisible;
        public bool NewGameVisible
        {
            get { return _NewGameVisible; }
            set
            {
                if (SetProperty(ref _NewGameVisible, value))
                    NewGameCommand!.ReportCanExecuteChange();
            }
        }
        public PlainCommand? NewGameCommand { get; set; } //this one for sure is plain.
        public CommandContainer? CommandContainer { get; set; }
        public IGamePackageResolver? MainContainer { get; set; }
        public async Task ShowGameMessageAsync(string thisStr)
        {
            await ThisMessage.ShowMessageBox(thisStr);
        }
        public SimpleGameVM(ISimpleUI tempUI, IGamePackageResolver tempC) //if you have other interface definitions, add here
        {
            ThisMessage = tempUI;
            MainContainer = tempC;
            SetUpBasicViewModel(this); //i like still having the shared function.
        }
        public abstract void Init();
        public abstract Task StartNewGameAsync();
        public virtual bool CanEnableBasics() //we have to make it virtual after all because single player games can still interact even though you have choice for new game.
        {
            return !NewGameVisible;
        }
    }
}