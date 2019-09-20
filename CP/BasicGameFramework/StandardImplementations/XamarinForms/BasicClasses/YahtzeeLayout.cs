﻿using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
namespace BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses
{
    public class YahtzeeLayout : IYahtzeeLayout
    {
        double IYahtzeeLayout.FooterFontSize
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 12;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 16;
                return 20;
            }
        }
        double IYahtzeeLayout.DescriptionFontSize
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 8;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 12;
                return 16;
            }
        }
        int IYahtzeeLayout.GetPixelHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 32;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 70;
                return 85;
            }
        }
        double IYahtzeeLayout.StandardFontSize
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
    }
}