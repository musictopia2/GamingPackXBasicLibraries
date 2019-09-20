using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
namespace BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses
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