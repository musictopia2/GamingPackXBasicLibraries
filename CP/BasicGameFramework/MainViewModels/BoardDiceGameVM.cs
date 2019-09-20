using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MainViewModels
{
    public abstract class BoardDiceGameVM<E, O, P, G, M> : SimpleBoardGameVM<E, O, P, G, M>
        , IDiceEvent<SimpleDice>, IBoardVM<E, O, P>
        where E : struct, Enum
        where O : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
        where G : class, ISimpleBoardGameProcesses<E, M>, IBasicGameProcesses<P>, IEndTurn
    {
        private StandardRollProcesses<SimpleDice, P>? _rollProcs;
        protected virtual bool CanRollDice()
        {
            return true;  //can be false in some cases (?)
        }
        protected async Task RollDiceProcessAsync() //some games require something else.
        {
            await _rollProcs!.RollDiceAsync();
        }
        public DiceCup<SimpleDice>? ThisCup { get; set; }
        public BoardDiceGameVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected abstract void FinishCup();
        protected abstract bool CanEnableDice();
        public BasicGameCommand? RollCommand { get; set; }
        public void LoadCup(BasicSavedBoardDiceGameClass<E, O, P> saveRoot, bool autoResume)
        {
            if (ThisCup != null && autoResume == false)
                return; //try this way.
            ThisCup = new DiceCup<SimpleDice>(saveRoot.DiceList, this);
            ThisCup.SendEnableProcesses(this, CanEnableDice);
            if (autoResume == true)
            {
                ThisCup.CanShowDice = true;
            }
            FinishCup();
        }
        protected override void EndInit()
        {
            base.EndInit();
            _rollProcs = MainContainer!.Resolve<StandardRollProcesses<SimpleDice, P>>(); //i think
            RollCommand = new BasicGameCommand(this, async items =>
            {
                await RollDiceProcessAsync();
            }, Items => CanRollDice(), this, CommandContainer!);
        }
        Task IDiceEvent<SimpleDice>.DiceClicked(SimpleDice thisDice)
        {
            return Task.CompletedTask; //this could be even better.
        }
    }
}