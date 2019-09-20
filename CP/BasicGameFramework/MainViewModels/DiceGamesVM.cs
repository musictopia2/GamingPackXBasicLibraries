using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MainViewModels
{
    public abstract class DiceGamesVM<D, P, G> : BasicMultiplayerVM<P, G>
        , IDiceVM<D, P>, IDiceEvent<D>
        where D : IStandardDice, new()
        where P : class, IPlayerItem, new()
        where G : class, IDiceMainProcesses<D, P>, IEndTurn
    {
        private StandardRollProcesses<D, P>? _rollProcs;
        private int _RollNumber;
        public int RollNumber
        {
            get { return _RollNumber; }
            set
            {
                value--; //i think
                if (SetProperty(ref _RollNumber, value)) { }
            }
        }
        protected virtual bool CanRollDice()
        {
            return true;  //can be false in some cases (?)
        }
        protected async Task RollDiceProcessAsync() //some games require something else.
        {
            await _rollProcs!.RollDiceAsync();
        }
        public DiceCup<D>? ThisCup { get; set; }
        public DiceGamesVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public void LoadCup(BasicSavedDiceClass<D, P> saveRoot, bool autoResume)
        {
            if (ThisCup != null && autoResume == false)
                return; //try this way.
            ThisCup = new DiceCup<D>(saveRoot.DiceList, this);
            ThisCup.SendEnableProcesses(this, CanEnableDice);
            if (autoResume == true)
            {
                ThisCup.CanShowDice = true;
            }
            FinishCup();
        }
        /// <summary>
        /// This is what happens after the cup is loaded.
        /// determine how many dice, plus other settings to load the cup.
        /// this is where you can specify you can't even click on dice on games like 21 dice game.
        /// </summary>
        protected abstract void FinishCup();
        protected abstract bool CanEnableDice();
        public BasicGameCommand? RollCommand { get; set; }
        //this means you do the base in addition to other things.
        protected override void EndInit()
        {
            _rollProcs = MainContainer!.Resolve<StandardRollProcesses<D, P>>(); //i think
            RollCommand = new BasicGameCommand(this, async items =>
            {
                await RollDiceProcessAsync();
            }, Items => CanRollDice(), this, CommandContainer!);
        }
        async Task IDiceEvent<D>.DiceClicked(D thisDice)
        {
            if (ThisCup!.ShowHold == true)
                await MainGame!.HoldUnholdDiceAsync(thisDice.Index); //i think
            else
                await _rollProcs!.SelectUnSelectDiceAsync(thisDice.Index); //i think.
        }
    }
}