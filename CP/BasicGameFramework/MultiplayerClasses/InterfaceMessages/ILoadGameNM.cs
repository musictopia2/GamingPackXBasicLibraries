using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface ILoadGameNM
    {
        Task LoadGameAsync(string data); //this is the data being sent.
    }
}
