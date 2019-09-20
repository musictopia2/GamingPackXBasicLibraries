using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses
{
    public class StandardProportion : IProportionBoard, IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == DataClasses.EnumScreen.Desktop)
                    return 2.3f; //uwp may require something different but don't know yet.
                if (ScreenUsed == DataClasses.EnumScreen.LargeTablet)
                    return 1.7f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallTablet)
                    return 1.3f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallPhone)
                    return .75f;
                throw new BasicBlankException("Screen not supported");
            }
        }
        float IProportionBoard.Proportion //may eventually do another implementation which is intended for xamarin forms (not sure).
        {
            get
            {
                if (ScreenUsed == DataClasses.EnumScreen.Desktop)
                    return 1.5f;
                if (ScreenUsed == DataClasses.EnumScreen.LargeTablet)
                    return 1.5f; //iffy.
                if (ScreenUsed == DataClasses.EnumScreen.SmallTablet)
                    return 1.2f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallPhone)
                    return 0.6f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}