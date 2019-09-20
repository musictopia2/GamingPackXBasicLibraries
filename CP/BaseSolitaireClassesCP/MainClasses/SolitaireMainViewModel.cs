using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileInterfaces;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BaseSolitaireClassesCP.MainClasses
{
    public abstract class SolitaireMainViewModel<S> : SimpleGameVM, IBasicSolitaireVM, IDeckClick where S : SolitaireSavedClass, new()
    {
        private SolitaireGameClass<S>? _mainGame; //attempt this without generics.
        public SolitaireMainViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    if (_mainGame!.SaveRoot == null)
                        return;
                    _mainGame.SaveRoot.Score = value; //try this.
                }
            }
        }
        public override bool CanEnableBasics()
        {
            return true;
        }
        public bool CanStartNewGameImmediately { get; set; } = true;
        public PlainCommand? AutoMoveCommand { get; set; }
        public DeckViewModel<SolitaireCard>? DeckPile { get; set; }
        public PileViewModel<SolitaireCard>? MainDiscardPile { get; set; }
        public IMain? MainPiles1 { get; set; }
        public IWaste? WastePiles1 { get; set; }
        public override void Init()
        {
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile = MainContainer!.Resolve<DeckViewModel<SolitaireCard>>();
            EventAggregator thisE = MainContainer.Resolve<EventAggregator>();
            MainDiscardPile = new PileViewModel<SolitaireCard>(thisE, this);
            MainPiles1 = MainContainer.Resolve<IMain>(); //should register this too.
            WastePiles1 = MainContainer.Resolve<IWaste>();
            _mainGame = MainContainer.Resolve<SolitaireGameClass<S>>();
            AutoMoveCommand = new PlainCommand(async items =>
            {
                await _mainGame.MakeAutoMovesToMainPilesAsync();
            }, items => _mainGame.GameGoing, this, CommandContainer);
            MainDiscardPile.PileClickedAsync += MainDiscardPile_PileClickedAsync; //we don't have double for now.
            WastePiles1.PileSelectedAsync += WastePiles1_PileSelectedAsync;
            WastePiles1.DoubleClickAsync += WastePiles1_DoubleClickAsync;
            MainPiles1.PileSelectedAsync += MainPiles1_PileSelectedAsync; //this does not support double for now.
            DeckPile.NeverAutoDisable = true;
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                if (_mainGame.DealsRemaining == 0 && DeckPile.IsEndOfDeck())
                    return false;
                else if (_mainGame.NoCardsToShuffle)
                    return false;
                return true;
            });
        }
        private async Task MainPiles1_PileSelectedAsync(int index)
        {
            await _mainGame!.MainPileSelectedAsync(index);
        }
        private bool _didDoubleClick;
        private async Task WastePiles1_DoubleClickAsync(int Index)
        {
            _didDoubleClick = true;
            await _mainGame!.WasteToMainAsync(Index);
        }
        private async Task WastePiles1_PileSelectedAsync(int Index)
        {
            if (_didDoubleClick == true)
            {
                _didDoubleClick = false;
                return;
            }
            await _mainGame!.WasteSelectedAsync(Index);
        }
        private async Task MainDiscardPile_PileClickedAsync()
        {
            //no need for double click since its not supported for now.
            await Task.CompletedTask;
            _mainGame!.SelectDiscard();
        }
        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            if (AutoMoveCommand == null)
                return;
            if (_mainGame == null || _mainGame.SaveRoot == null || _mainGame.GameGoing == false)
                return;
            CommandExecutingChanged(); //this is needed so for games like eagle wings, can determine whether to enable the heel or not (?)
            await _mainGame.SaveGameAsync();//i think
        }
        protected virtual void CommandExecutingChanged() { }
        public override async Task StartNewGameAsync()
        {
            await _mainGame!.InitAsync(); //i think.
            NewGameVisible = true; //most of the time, make visible.   i can manually mark it not that if necessary.
        }
        async Task IDeckClick.DeckClicked()
        {
            await Task.CompletedTask;
            if (DeckPile!.DeckStyle == DeckViewModel<SolitaireCard>.EnumStyleType.AlwaysKnown)
            {
                if (WastePiles1!.OneSelected() > 0)
                    return;
                DeckPile.IsSelected = !DeckPile.IsSelected;
                return;
            }
            _mainGame!.DrawCard();
        }
    }
}