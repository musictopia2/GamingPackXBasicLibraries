using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using System.Threading.Tasks;

namespace BasicGamingUIXFLibrary.Shells
{
    public class YahtzeeShellView : MultiplayerBasicShellView
    {

        public YahtzeeShellView(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen
            ) : base(customPlatform, gameData, basicData, start, screen)
        {
        }

        protected override Task PopulateUIAsync()
        {
            return Task.CompletedTask;
        }

    }
}
