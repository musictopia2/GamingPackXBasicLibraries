using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;
namespace BasicGameFrameworkLibrary.RegularDeckOfCards
{
    public class RegularTrickCard : RegularSimpleCard, ITrickCard<EnumSuitList>
    {
        public int Player { get; set; } //i don't think this needs binding.
        private SKPoint _location;

        public SKPoint Location
        {
            get { return _location; }
            set
            {
                if (SetProperty(ref _location, value)) { }
            }
        }
        public virtual int GetPoints => Points; //different games can have different formulas for calculating points.
        public object CloneCard()
        {
            return MemberwiseClone(); //hopefully this simple (?)
        }
    }
}