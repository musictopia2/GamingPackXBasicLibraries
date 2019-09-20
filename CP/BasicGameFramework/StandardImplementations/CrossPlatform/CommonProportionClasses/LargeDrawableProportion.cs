using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses
{ 
    public class LargeDrawableProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == DataClasses.EnumScreen.Desktop || ScreenUsed == DataClasses.EnumScreen.LargeTablet)
                    return 2.9f; //uwp may require something different but don't know yet.
                if (ScreenUsed == DataClasses.EnumScreen.SmallTablet)
                    return 2.0f;
                if (ScreenUsed == DataClasses.EnumScreen.SmallPhone)
                    return 1.5f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}
