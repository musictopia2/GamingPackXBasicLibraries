using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels
{
    //the old view model has to be put under data classes or i break every game
    public interface IDominoGamesData<D> : IViewModelData, IBasicEnableProcess, IEnableAlways
        where D : IDominoInfo, new()
    {
        //i think this should do delegates.  because the game class can't reference this at all.

        Func<D, Task>? DrewDominoAsync { get; set; }

        HandObservable<D> PlayerHand1 { get; set; }
        DominosBoneYardClass<D> BoneYard { get; set; } //hopefully i don't regret this.
        //the main view model is responsible for the canenable boneyard.
        Func<Task>? PlayerBoardClickedAsync { get; set; }
        Func<D, int, Task>? HandClickedAsync { get; set; }
    }
}
