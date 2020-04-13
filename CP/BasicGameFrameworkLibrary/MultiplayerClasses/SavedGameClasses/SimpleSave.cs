using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
namespace BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses
{
    public abstract class SimpleSave : ObservableObject
    {
        //until we have something even set up, will return false.
        public bool CanPrivateSave { get; set; }

        public string GameID { get; set; } = "";
        public void GetNewID()
        {
            GameID = Guid.NewGuid().ToString();
        }
        //2 choices.  either for each section or for all.
        //probably has to be for all.
        //if getting previous state one, then no privates.
    }
}