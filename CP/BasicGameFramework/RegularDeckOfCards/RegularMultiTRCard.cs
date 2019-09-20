using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using SkiaSharp;
namespace BasicGameFramework.RegularDeckOfCards
{
    public class RegularMultiTRCard : RegularSimpleCard, ITrickCard<EnumSuitList>, IRummmyObject<EnumSuitList, EnumColorList>
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
        int IRummmyObject<EnumSuitList, EnumColorList>.GetSecondNumber => (int)SecondNumber; //decided that even for rummy games, it will lean towards low.  if i am wrong, rethink.  for cases there is a choice, lean towards low.
        bool IIgnoreObject.IsObjectIgnored => false; //i can't think of a single game where we can ignore a card.
        public EnumCardValueList SecondNumber //since i use low ace, here, use there too.
        {
            get
            {
                if (Value != EnumCardValueList.HighAce)
                    return Value;
                return EnumCardValueList.LowAce; //second seemed to lean towards low.
            }
        }
    }
}