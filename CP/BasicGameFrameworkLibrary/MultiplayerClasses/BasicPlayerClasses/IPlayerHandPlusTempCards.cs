using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
namespace BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerHandPlusTempCards<D> : IPlayerSingleHand<D>
        where D : IDeckObject, new()
    {
        DeckObservableDict<D> AdditionalObjects { get; set; }
    }
}