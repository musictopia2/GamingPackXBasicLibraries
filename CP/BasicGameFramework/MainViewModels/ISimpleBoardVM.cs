using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System;
namespace BasicGameFramework.MainViewModels
{
    public interface ISimpleBoardVM<E, G, P> : ISimpleMultiPlayerVM
        where E : struct, Enum
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
    {
        string Instructions { get; set; }
        bool ColorVisible { get; } //readonly
        void NotifyColorChange(); //to notify its changed.
        BoardGamesColorPicker<E, G, P>? ColorChooser { get; set; } //this is needed too.
    }
}