using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MainViewModels
{
    public class SimpleBoardGameVM<E, O, P, G, M> : BasicMultiplayerVM<P, G>, ISimpleBoardVM<E, O, P>
        where E : struct, Enum
        where O : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
        where G : class, ISimpleBoardGameProcesses<E, M>, IBasicGameProcesses<P>, IEndTurn
    {
        private string _Instructions = "";
        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value)) { }
            }
        }
        public BoardGamesColorPicker<E, O, P>? ColorChooser { get; set; }
        public bool ColorVisible => ColorChooser!.Visible;
        public SimpleBoardGameVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public void NotifyColorChange()
        {
            OnPropertyChanged(nameof(ColorVisible));
        }
        protected override void EndInit()
        {
            base.EndInit();
            ColorChooser = new BoardGamesColorPicker<E, O, P>(this);
            ColorChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent; //forgot this part.
            ColorChooser.ItemClickedAsync += ColorChooser_ItemClickedAsync;
        }
        private async Task ColorChooser_ItemClickedAsync(E thisPiece)
        {
            await MainGame!.ChoseColorAsync(thisPiece);
        }
    }
}