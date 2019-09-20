using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
namespace BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses
{
    public class StandardPickerSizeClass : IEnumPickerSize
    { //for now, only normal and small no large.  could add large if necessary (?)  of course, you are open 
        float IEnumPickerSize.NormalGraphicsWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 50;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 80;
                return 100;
            }
        }
        float IEnumPickerSize.SmallGraphicsWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 40;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 70;
                return 90; //can always adjust as needed.
            }
        }
    }
}