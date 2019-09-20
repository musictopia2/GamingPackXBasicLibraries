using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IProcessHoldNM
    {
        Task ProcessHoldReceivedAsync(int iD); //this for sure is an integer.
    }
}