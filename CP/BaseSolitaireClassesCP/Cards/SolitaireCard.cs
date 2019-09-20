using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace BaseSolitaireClassesCP.Cards
{
    public class SolitaireCard : RegularSimpleCard
    {
        public SKPoint Location { get; set; } //not sure if it needs binding or not (?)
    }
}