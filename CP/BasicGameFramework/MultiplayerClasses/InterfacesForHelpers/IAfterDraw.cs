using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfacesForHelpers
{
    public interface IAfterDraw<P> : IBasicGameProcesses<P>
        where P : class, IPlayerItem, new()
    {
        Task AfterDrawingAsync();
    }
}