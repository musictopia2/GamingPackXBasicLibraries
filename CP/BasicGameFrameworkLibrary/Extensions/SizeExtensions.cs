using BasicGameFrameworkLibrary.CommonInterfaces;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.Extensions
{
    public static class SizeExtensions
    {
        public static void CalculateSizes(this IDefaultSize imageSize, float proportion)
        {
            imageSize.SizeUsed = imageSize.DefaultSize.GetSizeUsed(proportion);
        }
        public static SKSize GetSizeUsed(this SKSize thisSize, float proportion)
        {
            return new SKSize(thisSize.Width * proportion, thisSize.Height * proportion);
        }
    }
}
