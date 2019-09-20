using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MainViewModels
{
    public abstract class BasicCardGamesVM<D, P, G> : BasicMultiplayerVM<P, G>, IBasicCardGameVM<D>, IDeckClick
        where D : IDeckObject, new()
        where P : class, IPlayerSingleHand<D>, new()
        where G : class, ICardGameMainProcesses<D>, IBasicGameProcesses<P>, IEndTurn
    {
        async Task IDeckClick.DeckClicked() //done.
        {
            if (PlayerHand1!.ObjectSelected() > 0)
            {
                await ThisMessage.ShowMessageBox("You have to unselect the card before drawing to prevent drawing by mistake");
                return;
            }
            if (ThisData!.MultiPlayer == true && CanSendDrawMessage == true)
                await ThisNet!.SendAllAsync("drawcard");
            MainGame!.PlayerDraws = 0;
            MainGame.LeftToDraw = 0;
            await MainGame.DrawAsync();
        }
        protected bool CanSendDrawMessage = true; // for games like dutch blitz, cannot send the message for drawing card.
        public DeckViewModel<D>? Deck1 { get; set; }
        public PileViewModel<D>? Pile1 { get; set; }
        public HandViewModel<D>? PlayerHand1 { get; set; }
        public BasicCardGamesVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected abstract bool CanEnableDeck();
        protected abstract bool CanEnablePile1();
        protected virtual bool CanEnableHand()
        {
            return false; // most likely won't be this simple.
        }
        protected virtual bool AlwaysEnableHand()
        {
            return true; // most of the time, you can enable hand.  if you can't then will be here
        }
        protected abstract Task ProcessDiscardClickedAsync();
        protected override void EndInit()
        {
            Deck1 = MainContainer!.Resolve<DeckViewModel<D>>();
            Pile1 = new PileViewModel<D>(ThisE!, this);
            PlayerHand1 = new HandViewModel<D>(this);
            Pile1.PileClickedAsync += ProcessDiscardClickedAsync;
            Deck1.SendEnableProcesses(this, () =>
            {
                return CanEnableDeck(); // so you have to override this one.
            });
            Pile1.SendEnableProcesses(this, () =>
            {
                return CanEnablePile1();
            });
            if (AlwaysEnableHand() == false)
            {
                PlayerHand1.SendEnableProcesses(this, () =>
                {
                    return CanEnableHand();
                });
                PlayerHand1.IsEnabled = false; // start with false
            }
            else
            {
                PlayerHand1.SendAlwaysEnable(this);// will handle this part
            }

            Deck1.Visible = true;
            Pile1.Visible = true;
            PlayerHand1.Visible = true;
            PlayerHand1.Text = "Your Cards";
            PlayerHand1.ObjectClickedAsync += ProcessHandClickedAsync; //done.
            PlayerHand1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync; //done
            PlayerHand1.BeforeAutoSelectObjectAsync += BeforeUnselectCardFromHandAsync; //done
            PlayerHand1.AutoSelectedOneCompletedAsync += OnAutoSelectedHandAsync; //done.
        }
        protected virtual Task BeforeUnselectCardFromHandAsync()
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnAutoSelectedHandAsync()
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnConsiderSelectOneCardAsync(D thisObject)
        {
            return Task.CompletedTask;
        }
        protected virtual Task ProcessHandClickedAsync(D thisCard, int index)
        {
            return Task.CompletedTask;
        }
    }
}