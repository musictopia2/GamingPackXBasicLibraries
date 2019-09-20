using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
namespace BasicGameFramework.ColorCards
{
    public interface IColorCard : IDeckObject, ISimpleValueObject<int>
        , IColorObject<EnumColorTypes>
    {
        EnumColorTypes Color { get; set; }
        string Display { get; set; } //this is needed so the ui can draw properly what it is.
    }
}