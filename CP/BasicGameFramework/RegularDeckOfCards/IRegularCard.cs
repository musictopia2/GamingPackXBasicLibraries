using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
namespace BasicGameFramework.RegularDeckOfCards
{
    public interface IRegularCard : IDeckObject, ISimpleValueObject<int>, IWildObject
        , ISuitObject<EnumSuitList>, IColorObject<EnumColorList>, IAdvancedDIContainer
    {
        EnumColorList Color { get; set; }
        EnumSuitList Suit { get; set; }
        EnumCardValueList Value { get; set; }
        EnumSuitList DisplaySuit { get; set; }
        EnumCardValueList DisplayNumber { get; set; } //this means it can show differently than what it really is.
        int Section { get; }
        EnumCardTypeList CardType { get; set; } //this is needed after all.
    }
}