using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public interface IMultiplayerSaveState
    {
        Task DeleteGameAsync();
        Task SaveStateAsync(object thisState);
        Task<string> SavedDataAsync(); //blank means no data.
        Task<string> TempMultiSavedAsync(); //we need this part now.
        Task<EnumRestoreCategory> SinglePlayerRestoreCategoryAsync();
        Task<EnumRestoreCategory> MultiplayerRestoreCategoryAsync();
    }
}