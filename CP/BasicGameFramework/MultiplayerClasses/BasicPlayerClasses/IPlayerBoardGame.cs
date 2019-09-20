using System;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerBoardGame<E> : IPlayerItem
        where E : Enum
    {
        E Color { get; set; }
        bool DidChooseColor { get; }
        /// <summary>
        /// this is to show no color was chosen.
        /// </summary>
        void Clear();
    }
}