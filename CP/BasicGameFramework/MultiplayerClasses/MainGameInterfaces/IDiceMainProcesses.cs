using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface IDiceMainProcesses<D, P> : IBasicGameProcesses<P>
        where P : class, IPlayerItem, new()
        where D : IStandardDice, new()
    {
        Task HoldUnholdDiceAsync(int index);
    }
}