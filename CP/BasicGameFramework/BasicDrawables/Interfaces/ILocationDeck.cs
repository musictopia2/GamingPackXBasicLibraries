using SkiaSharp;
namespace BasicGameFramework.BasicDrawables.Interfaces
{
    public interface ILocationDeck : IDeckObject
    {
        SKPoint Location { get; set; } //location will be needed for the scattering pieces.
    }
}