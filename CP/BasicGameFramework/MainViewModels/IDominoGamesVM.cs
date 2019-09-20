using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks;
namespace BasicGameFramework.MainViewModels
{
    public interface IDominoGamesVM<D> : IBasicGameVM, ISimpleMultiPlayerVM
        where D : IDominoInfo, new()
    {
        HandViewModel<D>? PlayerHand1 { get; set; }
        Task DrewDominoAsync(D thisDomino);
        DominosBoneYardClass<D>? BoneYard { get; set; }
    }
}