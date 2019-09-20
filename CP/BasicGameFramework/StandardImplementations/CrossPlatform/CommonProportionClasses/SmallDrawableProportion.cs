using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses
{
    public class SmallDrawableProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == DataClasses.EnumScreen.Desktop)
                    return 1.1f; //uwp may require something different but don't know yet.
                if (ScreenUsed == DataClasses.EnumScreen.LargeTablet)
                    return .85f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallTablet)
                    return .65f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallPhone)
                    return .35f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}