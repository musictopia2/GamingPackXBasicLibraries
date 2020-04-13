using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicXFControlsAndPages.MVVMFramework.Controls;

namespace BasicGamingUIXFLibrary.Shells
{
    public abstract class BasicBoardGamesShellView : MultiplayerBasicShellView
    {
        public BasicBoardGamesShellView(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
        }

        protected override void AddOtherStartingScreens()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(IBasicBoardGamesShellViewModel.ColorScreen));
            AddMain(parent);
        }

    }
}
