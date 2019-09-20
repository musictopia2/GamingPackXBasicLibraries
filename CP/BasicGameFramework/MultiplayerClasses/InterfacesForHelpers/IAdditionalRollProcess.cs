using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.InterfacesForHelpers
{
    public interface IAdditionalRollProcess
    {
        Task<bool> CanRollAsync();
        Task BeforeRollingAsync();
    }
}