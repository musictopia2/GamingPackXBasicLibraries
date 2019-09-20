using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public interface IMultiplayerTrick<S, T, P>
        where S : Enum
        where T : ITrickCard<S>, new()
        where P : IPlayerSingleHand<T>, new()
    {
        CustomBasicList<TrickCoordinate>? ViewList { get; set; }
        P GetSpecificPlayer(int id);
    }
}
