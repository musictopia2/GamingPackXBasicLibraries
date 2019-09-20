using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IEndTurnNM
    {
        Task EndTurnReceivedAsync(string data); //sometimes there is data.  needs to keep the option open.
    }
}