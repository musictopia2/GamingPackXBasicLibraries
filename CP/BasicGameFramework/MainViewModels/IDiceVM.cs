using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace BasicGameFramework.MainViewModels
{
    public interface IDiceVM<D, P> : ISimpleMultiPlayerVM
        where D : IStandardDice, new()
        where P : class, IPlayerItem, new()
    {
        int RollNumber { get; set; }
        DiceCup<D>? ThisCup { get; set; }
        void LoadCup(BasicSavedDiceClass<D, P> saveRoot, bool autoResume);
    }
}