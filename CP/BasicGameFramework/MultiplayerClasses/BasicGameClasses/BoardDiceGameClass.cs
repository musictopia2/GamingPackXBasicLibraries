using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class BoardDiceGameClass<E, G, P, S, M> : SimpleBoardGameClass<E, G, P, S, M>
        , IStandardRoller<SimpleDice, P>
        where E : struct, Enum
        where P : class, IPlayerBoardGame<E>, new()
        where S : BasicSavedBoardDiceGameClass<E, G, P>, new()
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
    {
        public DiceCup<SimpleDice>? ThisCup => _thisMod.ThisCup;
        public StandardRollProcesses<SimpleDice, P>? ThisRoll;
        private readonly IBoardVM<E, G, P> _thisMod;
        public BoardDiceGameClass(IGamePackageResolver container) : base(container)
        {
            _thisMod = MainContainer.Resolve<IBoardVM<E, G, P>>();
        }
        public Task AfterSelectUnselectDiceAsync()
        {
            return Task.CompletedTask;
        }
        public abstract Task AfterRollingAsync();
        protected void LoadUpDice()
        {
            if (IsLoaded == true)
                throw new BasicBlankException("Should not load the dice if its already loaded.  Otherwise, rethink");
            ThisRoll = MainContainer.Resolve<StandardRollProcesses<SimpleDice, P>>();
        }
        protected void SetUpDice()
        {
            _thisMod.LoadCup(SaveRoot!, false); //i think.
            SaveRoot!.DiceList.MainContainer = MainContainer; //maybe has to be here.
        }
        protected virtual bool ShowDiceUponAutoSave => true;
        protected void AfterRestoreDice()
        {
            _thisMod.LoadCup(SaveRoot!, true); //this is if its autoresume.
            SaveRoot!.DiceList.MainContainer = MainContainer; //maybe has to be here.
            if (ShowDiceUponAutoSave == true)
            {
                _thisMod.ThisCup!.CanShowDice = true;
                _thisMod.ThisCup.ShowDiceListAlways = true;
                _thisMod.ThisCup.Visible = true;
            }
            SaveRoot.ThisMod = MainContainer.Resolve<ISimpleBoardVM<E, G, P>>();
        }
    }
}