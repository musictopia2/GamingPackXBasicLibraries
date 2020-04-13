using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;

namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses
{
    public class StandardButtonFontClass : IFontProcesses
    {
        double IFontProcesses.SmallFontSize
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 8;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 18;
                return 24;
            }
        }
        double IFontProcesses.NormalFontSize
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 20;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 30;
                return 40;
            }
        }
        double IFontProcesses.HeightRequest
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 30;
                return 0;
            }
        }
    }
}