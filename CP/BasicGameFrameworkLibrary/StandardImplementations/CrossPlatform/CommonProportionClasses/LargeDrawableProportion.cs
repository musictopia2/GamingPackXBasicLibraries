using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses
{ 
    public class LargeDrawableProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.Desktop || ScreenUsed == EnumScreen.LargeTablet)
                    return 2.9f; //uwp may require something different but don't know yet.
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 2.0f;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.5f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}
