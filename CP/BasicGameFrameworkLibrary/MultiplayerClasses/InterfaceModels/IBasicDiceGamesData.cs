using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using System;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels
{
    public interface IBasicDiceGamesData<D> : IViewModelData, ICup<D>
        where D : IStandardDice, new()
    {
        int RollNumber { get; set; }

        //Action<ISavedDiceList<D>, bool>? LoadCup { get; set; } //decided to do as delegate.  this is better this time.
        //has to use this after all.
        void LoadCup(ISavedDiceList<D> saveRoot, bool autoResume);
    }
}