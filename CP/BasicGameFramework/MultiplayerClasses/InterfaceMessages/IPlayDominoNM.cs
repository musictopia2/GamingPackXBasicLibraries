using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IPlayDominoNM
    {
        Task PlayDominoMessageAsync(int deck); //needs to be integer for sure.  used for domino games.
    }
}