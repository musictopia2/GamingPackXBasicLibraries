using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.ViewModelInterfaces;
namespace BasicGameFramework.MainViewModels
{
    public interface IBasicCardGameVM<D> : IBasicGameVM, ISimpleMultiPlayerVM where D : IDeckObject, new()
    {
        DeckViewModel<D>? Deck1 { get; set; } //i think
        PileViewModel<D>? Pile1 { get; set; }
        HandViewModel<D>? PlayerHand1 { get; set; }
    }
}