using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels
{
    public abstract class DiceGamesVM<D> : BasicMultiplayerMainVM
        where D : IStandardDice, new()

    {
        public DiceGamesVM(CommandContainer commandContainer,
            IHoldUnholdProcesses mainGame,
            IBasicDiceGamesData<D> viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses rollProcesses
            ) : base(commandContainer,
                mainGame,
                viewModel,
                basicData,
                test,
                resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _rollProcesses = rollProcesses;
            if (_model.Cup == null)
            {
                throw new BasicBlankException("There was no cup.  Rethink");
            }
            _model.Cup.SendEnableProcesses(this, CanEnableDice);
            _model.Cup.DiceClickedAsync += Cup_DiceClickedAsync;
            //no finish cup this time.  the data for the cup has to be in the vmdata.
        }

        private readonly IHoldUnholdProcesses _mainGame;
        private readonly IBasicDiceGamesData<D> _model;
        private readonly IStandardRollProcesses _rollProcesses;

        //hopefully can do away from generics here.

        private int _rollNumber;
        [VM]
        public int RollNumber
        {
            get { return _rollNumber; }
            set
            {
                value--; //try this way.
                if (SetProperty(ref _rollNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        //the vmdata had to do the loadcup.  since the view model is not even open when its needed.

        private async Task Cup_DiceClickedAsync(D arg)
        {
            if (_model.Cup!.ShowHold)
            {
                await _mainGame.HoldUnholdDiceAsync(arg.Index);
            }
            else
            {
                await _rollProcesses.SelectUnSelectDiceAsync(arg.Index);
            }
        }

        public virtual bool CanRollDice()
        {
            return true;  //can be false in some cases (?)
        }
        [Command(EnumCommandCategory.Game)]
        public async Task RollDiceAsync()
        {
            await _rollProcesses.RollDiceAsync();
        }

        protected override Task TryCloseAsync()
        {
            if (_model.Cup == null)
            {
                return Task.CompletedTask;
            }
            _model.Cup.DiceClickedAsync -= Cup_DiceClickedAsync;
            return base.TryCloseAsync();
        }

        
        protected abstract bool CanEnableDice();

    }
}
