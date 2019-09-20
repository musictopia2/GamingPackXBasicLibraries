using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MainViewModels
{
    public abstract class DominoGamesVM<D, P, G> : BasicMultiplayerVM<P, G>,
        IDominoGamesVM<D>
        where D : IDominoInfo, new()
        where P : class, IPlayerSingleHand<D>, new()
        where G : class, IDominosMainProcesses<D, P>, IEndTurn
    {
        public DominoGamesVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public HandViewModel<D>? PlayerHand1 { get; set; }
        public DominosBoneYardClass<D>? BoneYard { get; set; }
        public async Task DrewDominoAsync(D thisDomino)
        {
            await MainGame!.DrawDominoAsync(thisDomino);
        }
        public Task ScatteringCompleted()
        {
            throw new BasicBlankException("Somehow never did the scattering completed.  Rethink");
        }
        protected virtual bool AlwaysEnableHand()
        {
            return true; // most of the time, you can enable hand.  if you can't then will be here
        }
        protected virtual bool CanEnableHand()
        {
            return true;
        }
        protected abstract bool CanEnableBoneYard();
        protected override void EndInit()
        {
            PlayerHand1 = new HandViewModel<D>(this);
            if (AlwaysEnableHand() == false)
            {
                PlayerHand1.SendEnableProcesses(this, () =>
                {
                    return CanEnableHand();
                });
            }
            else
            {
                PlayerHand1.SendAlwaysEnable(this);// will handle this part
            }

            BoneYard = new DominosBoneYardClass<D>(this);
            BoneYard.SendEnableProcesses(this, () =>
            {
                return CanEnableBoneYard();
            });
            PlayerHand1.ObjectClickedAsync += PlayerHand1_ObjectClickedAsync;
            PlayerHand1.BoardClickedAsync += PlayerHand1_BoardClickedAsync;
        }
        private async Task PlayerHand1_BoardClickedAsync()
        {
            await PlayerBoardClickedAsync();
        }
        protected virtual Task PlayerBoardClickedAsync()
        {
            return Task.CompletedTask;
        }
        protected virtual Task HandClicked(D thisDomino, int index)
        {
            return Task.CompletedTask;
        }
        private async Task PlayerHand1_ObjectClickedAsync(D thisObject, int index)
        {
            await HandClicked(thisObject, index);
        }
        public override bool CanEndTurn()
        {
            if (BoneYard!.IsEnabled == true)
                return false;
            else
                return base.CanEndTurn();
        }
    }
}