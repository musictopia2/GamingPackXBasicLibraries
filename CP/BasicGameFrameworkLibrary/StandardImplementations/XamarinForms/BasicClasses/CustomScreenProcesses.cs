using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;

namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses
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