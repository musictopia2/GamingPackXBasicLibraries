using BasicGameFramework.CommonInterfaces;
using SkiaSharp;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
namespace BasicGameFramework.BasicDrawables.Interfaces
{
    public interface IDeckObject : ICommonObject, IPopulateObject<int>
    {
        int Deck { get; set; }
        bool Drew { get; set; }
        bool IsUnknown { get; set; }
        EnumRotateCategory Angle { get; set; }
        SKSize DefaultSize { get; set; }
        void Reset(); //sometimes needs to be reset.  everything can need it.
    }
}