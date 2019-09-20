using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IRolledNM
    {
        Task RollReceivedAsync(string data);
    }
}