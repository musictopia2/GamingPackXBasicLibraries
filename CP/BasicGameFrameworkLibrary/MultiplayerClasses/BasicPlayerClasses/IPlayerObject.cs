﻿using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
namespace BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerObject<D> : IPlayerItem where D : IDeckObject, new()
    {
        DeckObservableDict<D> MainHandList { get; set; }
        DeckRegularDict<D> StartUpList { get; set; }
    }
}