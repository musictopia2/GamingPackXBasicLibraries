using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BaseSolitaireClassesCP.TriangleClasses
{
    public interface ITriangleVM : IBasicGameVM
    {
        Task CardClickedAsync(SolitaireCard thisCard);
    }
}