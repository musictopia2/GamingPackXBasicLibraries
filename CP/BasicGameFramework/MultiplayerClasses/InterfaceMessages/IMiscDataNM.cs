using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IMiscDataNM
    {
        Task MiscDataReceived(string status, string content);
    }
}