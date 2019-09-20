using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerHandPlusTempCards<D> : IPlayerSingleHand<D>
        where D : IDeckObject, new()
    {
        DeckObservableDict<D> AdditionalObjects { get; set; }
    }
}