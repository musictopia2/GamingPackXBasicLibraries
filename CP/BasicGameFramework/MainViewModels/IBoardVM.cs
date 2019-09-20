using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System;
namespace BasicGameFramework.MainViewModels
{
    public interface IBoardVM<E, G, P>
        where E : struct, Enum
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
    {
        DiceCup<SimpleDice>? ThisCup { get; set; }
        void LoadCup(BasicSavedBoardDiceGameClass<E, G, P> saveRoot, bool autoResume);
    }
}