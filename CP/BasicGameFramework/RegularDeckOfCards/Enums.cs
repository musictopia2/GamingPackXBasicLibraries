namespace BasicGameFramework.RegularDeckOfCards
{
    public enum EnumSuitList
    {
        None, Clubs, Diamonds, Spades, Hearts
    }
    public enum EnumColorList
    {
        None, Red, Black
    }
    public enum EnumCardTypeList
    {
        None = -1, Regular, Joker, Stop, Continue
    }
    public enum EnumCardValueList
    {
        None, LowAce, Two, Three, Four, Five, Six,
        Seven, Eight, Nine, Ten,
        Jack, Queen, King, HighAce, Joker = 20, Stop, Continue
    }
    public enum EnumSortCategory
    {
        SuitNumber, NumberSuit
    }
}