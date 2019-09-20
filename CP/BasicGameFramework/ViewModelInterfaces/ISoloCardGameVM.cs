using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface ISoloCardGameVM<D> : IBasicGameVM, IDeckClick where D : IDeckObject, new()
    {
        DeckViewModel<D>? DeckPile { get; set; }  //this is needed too.
    }
}