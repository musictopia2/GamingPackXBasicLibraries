using SkiaSharp;
namespace BasicGameFrameworkLibrary.BasicDrawables.Interfaces
{
    public interface ILocationDeck : IDeckObject
    {
        SKPoint Location { get; set; } //location will be needed for the scattering pieces.
    }
}