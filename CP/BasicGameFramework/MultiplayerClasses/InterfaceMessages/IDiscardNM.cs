using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IDiscardNM
    {
        Task DiscardReceivedAsync(string data);
    }
}
