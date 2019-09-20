using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface ISelectDiceNM
    {
        Task SelectDiceReceivedAsync(int iD);
    }
}