using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
namespace BasicGamingUIWPFLibrary.Shells
{
    public abstract class BasicBoardGamesShellView : MultiplayerBasicShellView
    {
        public BasicBoardGamesShellView(IGameInfo gameData, BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {
        }

        protected override void AddOtherStartingScreens()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(IBasicBoardGamesShellViewModel.ColorScreen)
            };
            AddMain(parent);
        }

    }
}