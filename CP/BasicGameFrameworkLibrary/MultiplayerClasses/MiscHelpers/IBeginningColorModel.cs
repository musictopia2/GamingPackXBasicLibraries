using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers
{
    public interface IBeginningColorModel<E, O>
        where E : struct, Enum
        where O : BaseGraphicsCP, IEnumPiece<E>, new()
    {
        SimpleEnumPickerVM<E,O>  ColorChooser { get; }
    }
}