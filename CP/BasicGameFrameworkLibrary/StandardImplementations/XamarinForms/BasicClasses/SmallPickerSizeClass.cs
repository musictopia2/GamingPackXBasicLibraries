using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;

namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses
{
    public class SmallPickerSizeClass : IEnumPickerSize
    {
        float IEnumPickerSize.NormalGraphicsWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 30;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 65;
                return 80;
            }
        }
        float IEnumPickerSize.SmallGraphicsWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 20;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 50;
                return 70; //can always adjust as needed.
            }
        }
    }
}
