using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
namespace BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses
{
    public class CustomScreenProcesses : IStandardScreen
    {
        bool IStandardScreen.IsSmallest => ScreenUsed == EnumScreen.SmallPhone;
        bool IStandardScreen.CanPlay(IGameInfo game)
        {
            if (game.SmallestSuggestedSize == EnumSmallestSuggested.AnyDevice)
                return true; //means any can play.
            if (ScreenUsed == EnumScreen.SmallPhone)
                return false;
            EnumSmallestSuggested size = game.SmallestSuggestedSize;
            return size switch
            {
                EnumSmallestSuggested.AnyTablet => true,
                EnumSmallestSuggested.LargeDevices => ScreenUsed >= EnumScreen.LargeTablet,
                EnumSmallestSuggested.DesktopOnly => false,
                _ => throw new BasicBlankException("Unsure if it can play.  Rethink"),
            };
        }
    }
}