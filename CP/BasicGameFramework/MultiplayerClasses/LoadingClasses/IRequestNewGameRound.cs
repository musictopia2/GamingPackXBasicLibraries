using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    /// <summary>
    /// This handles everything when a player hits the commands to request new game or new round.
    /// </summary>
    public interface IRequestNewGameRound
    {
        Task NewGameFromCommandAsync();
        Task NewRoundFromCommandAsync();
    }
}