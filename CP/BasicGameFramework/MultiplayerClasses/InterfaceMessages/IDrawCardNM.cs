using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IDrawCardNM
    {
        Task DrawCardReceivedAsync(string data); //needs to keep the option open for data being sent.
    }
}