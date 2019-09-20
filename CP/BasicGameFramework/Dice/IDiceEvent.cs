using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks;
namespace BasicGameFramework.Dice
{
    public interface IDiceEvent<D> : IBasicGameVM where D :
        IStandardDice, new()
    {
        Task DiceClicked(D thisDice);
    }
}