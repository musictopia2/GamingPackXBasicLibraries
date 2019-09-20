using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfaceMessages
{
    public interface IColorNM
    {
        Task ColorSentAsync(string data);
    }
}
