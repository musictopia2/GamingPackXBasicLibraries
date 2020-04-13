using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;

namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses
{
    public class LargeWidthHeight : IWidthHeight
    {
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 80;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 140;
                return 180;
            }
        }
    }
    public class SmallWidthHeight : IWidthHeight
    {
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 20;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 35;
                return 45;
            }
        }
    }
    public class StandardWidthHeight : IWidthHeight
    {
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 40;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 70;
                return 90;
            }
        }
    }
}