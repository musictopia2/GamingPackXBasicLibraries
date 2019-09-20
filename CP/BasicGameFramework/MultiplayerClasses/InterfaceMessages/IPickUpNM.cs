using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IPickUpNM
    {
        Task PickUpReceivedAsync(string data);
    }
}