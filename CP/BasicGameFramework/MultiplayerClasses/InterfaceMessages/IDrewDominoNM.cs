using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IDrewDominoNM
    {
        Task DrewDominoReceivedAsync(int deck);
    }
}