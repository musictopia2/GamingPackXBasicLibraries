using BasicGameFramework.BasicDrawables.Interfaces;
using System;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerTrick<S, T> : IPlayerSingleHand<T>
        where S : Enum
        where T : ITrickCard<S>, new()
    {
        int TricksWon { get; set; } //this is common on all trick taking games.
    }
}