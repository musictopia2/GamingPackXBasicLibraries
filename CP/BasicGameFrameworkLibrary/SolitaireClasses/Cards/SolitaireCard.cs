using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SkiaSharp;
namespace BasicGameFrameworkLibrary.SolitaireClasses.Cards
{
    public class SolitaireCard : RegularSimpleCard
    {
        public SKPoint Location { get; set; } //not sure if it needs binding or not (?)
    }
}