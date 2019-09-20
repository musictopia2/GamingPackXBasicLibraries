using System;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces
{
    public class CirclePieceWPF<E> : BaseGraphicsWPF<CirclePieceCP<E>> where E : Enum
    {
        public bool NeedsWhiteBorders //risks paid off this time.
        {
            get { return Mains.NeedsWhiteBorders; }
            set { Mains.NeedsWhiteBorders = value; }
        }
        public string SetColor
        {
            get { return Mains.MainColor; }
            set { Mains.MainColor = value; }
        }
    }
}