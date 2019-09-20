using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    /// <summary>
    /// this is used in cases where you are restoring a saved game.
    /// </summary>
    public interface IRestoreMultiPlayerGame
    {
        /// <summary>
        /// this restores the game back to where it was when you first started playing it.
        /// only works if you set the save type to restore only.  otherwise, it will not work.
        /// if that changes, rethink.
        /// </summary>
        /// <returns></returns>
        Task RestoreGameAsync(); //this is all it needs.
        //this is solely responsible for getting back to the state you were when you started playing that game.
        /// <summary>
        /// this is used when sending information to the other player for restoring.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        Task RestoreMessageAsync(string data);
    }
}