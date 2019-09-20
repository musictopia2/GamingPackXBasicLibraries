using System.Threading.Tasks;
namespace BasicGameFramework.CommonInterfaces
{
    public interface ISaveSinglePlayerClass
    {
        Task<bool> CanOpenSavedSinglePlayerGameAsync(); //could be async even for this part (we never know).
        Task DeleteSinglePlayerGameAsync(); //deleting it could even be just setting occurance to null so i can go back to it if necessary.
        Task<T> RetrieveSinglePlayerGameAsync<T>();
        Task SaveSimpleSinglePlayerGameAsync(object thisObject); //decided that object is fine for this.
    }
}