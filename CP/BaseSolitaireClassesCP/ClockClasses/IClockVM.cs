using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BaseSolitaireClassesCP.ClockClasses
{
    public interface IClockVM : IBasicGameVM
    {
        Task ClockClickedAsync(int index); //will be 0 based.
    }
}