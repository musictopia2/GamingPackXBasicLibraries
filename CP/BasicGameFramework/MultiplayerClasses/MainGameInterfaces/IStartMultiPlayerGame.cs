using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface IStartMultiPlayerGame<P>
        where P : class, IPlayerItem, new()
    {
        Task LoadNewGameAsync(PlayerCollection<P> startList);
        Task LoadSavedGameAsync(); //this means there has to be saved data period.
    }
}