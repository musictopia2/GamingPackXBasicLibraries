using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using System;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers
{
    public class BeginningColorModel<E, O, P> : IBeginningColorModel<E, O>
        where E : struct, Enum
        where O : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
    {
        public BoardGamesColorPicker<E, O, P> ColorChooser { get; set; }
        SimpleEnumPickerVM<E, O> IBeginningColorModel<E, O>.ColorChooser => ColorChooser;

        public BeginningColorModel(CommandContainer command)
        {
            ColorChooser = new BoardGamesColorPicker<E, O, P>(command, new ColorListChooser<E>());
        }
    }
}