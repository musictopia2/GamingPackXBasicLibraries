using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface ITrickNM
    {
        Task TrickPlayReceivedAsync(int deck);
    }
}