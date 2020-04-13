using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;

namespace BasicGameFrameworkLibrary.ViewModels
{
    public interface IBasicMultiplayerShellViewModel
    {
        IMainScreen? MainVM { get; set; }
        INewGameVM? NewGameScreen { get; set; }
        INewRoundVM? NewRoundScreen { get; set; }
        string NickName { get; set; }
        IMultiplayerOpeningViewModel? OpeningScreen { get;}
    }
}