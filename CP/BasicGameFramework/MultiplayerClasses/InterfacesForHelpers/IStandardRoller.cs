using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.InterfacesForHelpers
{
    /// <summary>
    /// this is everything the roll processes need to perform the work.
    /// part of it is the cup.  
    /// </summary>
    public interface IStandardRoller<D, P> : IBasicGameProcesses<P>
        where D : IStandardDice, new()
        where P : class, IPlayerItem, new()
    {
        DiceCup<D>? ThisCup { get; } //for sure needs to know about a cup which is used to roll the dice.
        Task AfterRollingAsync();
        Task AfterSelectUnselectDiceAsync();
        IGamePackageResolver MainContainer { get; set; }
    }
}