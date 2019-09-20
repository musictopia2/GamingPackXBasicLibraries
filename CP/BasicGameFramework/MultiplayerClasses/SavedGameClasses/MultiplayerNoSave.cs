using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
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
        public Task SaveStateAsync(object thisState)
        {
            return Task.CompletedTask;
        }
        public Task<EnumRestoreCategory> SinglePlayerRestoreCategoryAsync()
        {
            return Task.FromResult(EnumRestoreCategory.NoRestore);
        }
        public Task<string> TempMultiSavedAsync()
        {
            return Task.FromResult("");
        }
    }
}