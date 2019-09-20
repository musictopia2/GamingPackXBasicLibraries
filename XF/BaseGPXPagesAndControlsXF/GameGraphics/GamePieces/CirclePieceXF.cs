﻿using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using System;
namespace BaseGPXPagesAndControlsXF.GameGraphics.GamePieces
{
    public class CirclePieceXF<E> : BaseGraphicsXF<CirclePieceCP<E>> where E : Enum
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
        protected override bool IsCircle => true;
    }
}