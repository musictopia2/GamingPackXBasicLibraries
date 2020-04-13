using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses
{
    public class MultiplayerNoSave : IMultiplayerSaveState
    {
        //this will mean no autoresume.
        //go ahead and use this.  so if no implementation, then use this which means no autoresume.
        public Task DeleteGameAsync()
        {
            return Task.CompletedTask;
        }
        public Task<EnumRestoreCategory> MultiplayerRestoreCategoryAsync()
        {
            return Task.FromResult(EnumRestoreCategory.NoRestore);
        }
        public Task<string> SavedDataAsync()
        {
            return Task.FromResult(""); // a blank string means no autoresume.
        }

        public Task<string> SavedDataAsync<T>() where T : IMappable, new()
        {
            return Task.FromResult(""); // a blank string means no autoresume.throw new System.NotImplementedException();
        }

        public Task SaveStateAsync<T>(T thisState) where T : IMappable, new()
        {
            return Task.CompletedTask;
        }

        public Task<EnumRestoreCategory> SinglePlayerRestoreCategoryAsync()
        {
            return Task.FromResult(EnumRestoreCategory.NoRestore);
        }
        

        Task<string> IMultiplayerSaveState.TempMultiSavedAsync()
        {
            return Task.FromResult("");
        }
    }
}
