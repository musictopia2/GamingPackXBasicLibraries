using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public class BasicSavedBoardDiceGameClass<E, G, P> : BasicSavedPlainBoardGameClass<E, G, P>
        where P : class, IPlayerBoardGame<E>, new()
        where E : struct, Enum
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
    {
        public DiceList<SimpleDice> DiceList = new DiceList<SimpleDice>();
    }
}