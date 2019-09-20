using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface INewGameNM
    {
        Task NewGameReceivedAsync(string data);
    }
}