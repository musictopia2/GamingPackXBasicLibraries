using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IChoosePieceNM
    {
        Task ChoosePieceReceivedAsync(string data); //keep the option open for other possibilities.
    }
}