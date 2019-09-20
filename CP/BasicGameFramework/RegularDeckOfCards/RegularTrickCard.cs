using BasicGameFramework.BasicDrawables.Interfaces;
using SkiaSharp;
namespace BasicGameFramework.RegularDeckOfCards
{
    public class RegularTrickCard : RegularSimpleCard, ITrickCard<EnumSuitList>
    {
        public int Player { get; set; } //i don't think this needs binding.
        private SKPoint _Location;

        public SKPoint Location
        {
            get { return _Location; }
            set
            {
                if (SetProperty(ref _Location, value)) { }
            }
        }
        public virtual int GetPoints => Points; //different games can have different formulas for calculating points.
        public object CloneCard()
        {
            return MemberwiseClone(); //hopefully this simple (?)
        }
    }
}