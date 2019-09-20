using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public interface ISavedCardList<D> where D : class, IDeckObject, new()
    {
        DeckRegularDict<D>? CardList { get; set; }
        D? CurrentCard { get; set; }
    }
}