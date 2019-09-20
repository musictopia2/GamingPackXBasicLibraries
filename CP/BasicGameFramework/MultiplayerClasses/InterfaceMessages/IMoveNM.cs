using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IMoveNM
    {
        Task MoveReceivedAsync(string data);
    }
}