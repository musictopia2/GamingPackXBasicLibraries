using System;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public abstract class PlayerBoardGame<E> : SimplePlayer, IPlayerBoardGame<E>
        where E : struct, Enum
    {
        private E _Color;
        public E Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value)) { }
            }
        }
        public abstract bool DidChooseColor { get; }
        public abstract void Clear();
    }
}