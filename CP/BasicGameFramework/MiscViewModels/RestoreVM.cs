using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MiscViewModels
{
    /// <summary>
    /// this is used in cases where you are in a middle of a game and the save option is restore only.
    /// in that case, when you use it, restores to the point everytime (without having to close out and go back in again).
    /// useful for fixing hidden bugs.
    /// 
    /// actually this should be observable object.
    /// because its part of a larger area (not the main one).  just like the opening view models.
    /// </summary>
    public class RestoreVM : ObservableObject, IHandleAsync<LoadEventModel>
    {
        public PlainCommand RestoreCommand { get; set; } //for now, this is fine.  i may have to rethink later.
        public RestoreVM(EventAggregator thisE, IGamePackageResolver thisContainer, TestOptions thisTest,
            BasicData ThisData, IBasicGameVM thisMod)
        {
            thisE.Subscribe(this);
            _thisTest = thisTest;
            _thisData = ThisData;
            RestoreCommand = new PlainCommand(async items =>
            {
                IRestoreMultiPlayerGame thisRestore = thisContainer.Resolve<IRestoreMultiPlayerGame>();
                await thisRestore.RestoreGameAsync();
            },
            items => Visible, thisMod, thisMod.CommandContainer!);
            RestoreCommand.BusyCategory = EnumCommandBusyCategory.Limited;
        }
        private bool _Visible;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value)) { }
            }
        }
        private readonly TestOptions _thisTest;
        private readonly BasicData _thisData;
        public Task HandleAsync(LoadEventModel message)
        {
            if (_thisData.Client == true && _thisData.MultiPlayer == true)
                return Task.CompletedTask;
            if (_thisTest.SaveOption != EnumTestSaveCategory.RestoreOnly)
                return Task.CompletedTask;
            Visible = true; //this means you can now 
            return Task.CompletedTask;
        }
    }
}