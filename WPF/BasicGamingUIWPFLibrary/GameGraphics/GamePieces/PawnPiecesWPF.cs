using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System;
namespace BasicGamingUIWPFLibrary.GameGraphics.GamePieces
{
    public class PawnPiecesWPF<E> : BaseGraphicsWPF<PawnPiecesCP<E>>
       where E : struct, Enum
    { }
}