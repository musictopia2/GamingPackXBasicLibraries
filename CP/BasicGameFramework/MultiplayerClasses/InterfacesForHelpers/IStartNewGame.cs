using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.InterfacesForHelpers
{
    /// <summary>
    /// this interface is needed in cases where the game has rounds and a person chose new game.
    /// i think every game needs something to reset because its truly different for every game.
    /// if a game has nothing, then just do nothing then.
    /// </summary>
    public interface IStartNewGame
    {
        Task ResetAsync();
    }
}