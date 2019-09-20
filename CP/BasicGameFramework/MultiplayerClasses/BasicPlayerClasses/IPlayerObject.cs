using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerObject<D> : IPlayerItem where D : IDeckObject, new()
    {
        DeckObservableDict<D> MainHandList { get; set; }
        DeckRegularDict<D> StartUpList { get; set; }
    }
}