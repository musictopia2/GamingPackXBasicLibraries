using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class DiceGameClass<D, P, S> : BasicGameClass<P, S>
        , IStandardRoller<D, P>, IProcessHoldNM,
        IDiceMainProcesses<D, P>
        where D : IStandardDice, new()
         where P : class, IPlayerItem, new()
        where S : BasicSavedDiceClass<D, P>, new()
    {
        private readonly IDiceVM<D, P> _thisMod;
        public DiceGameClass(IGamePackageResolver _Container) : base(_Container)
        {
            _thisMod = MainContainer.Resolve<IDiceVM<D, P>>(); //has to use this instead since we need more details.
        }
        public DiceCup<D>? ThisCup => _thisMod.ThisCup;
        public StandardRollProcesses<D, P>? ThisRoll; //needs this because computerturn may need it
        protected virtual bool ShowDiceUponAutoSave => true;
        protected void LoadUpDice()
        {
            if (IsLoaded == true)
                throw new BasicBlankException("Should not load the dice if its already loaded.  Otherwise, rethink");
            SaveRoot!.ThisMod = _thisMod;
            ThisRoll = MainContainer.Resolve<StandardRollProcesses<D, P>>();
        }
        protected void SetUpDice()
        {
            _thisMod.LoadCup(SaveRoot!, false); //i think.
            SaveRoot!.RollNumber = 1;
            SaveRoot.DiceList.MainContainer = MainContainer; //maybe has to be here.
        }
        protected void ProtectedStartTurn()
        {
            SaveRoot!.RollNumber = 1;
        }
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
            else
                _thisMod.ThisCup!.CanShowDice = false;
            _thisMod.RollNumber = SaveRoot.RollNumber; //i think here too.
            SaveRoot.ThisMod = _thisMod; //i think.
        }
        protected async Task SendHoldMessageAsync(int index)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("processhold", index);
        }
        public virtual async Task HoldUnholdDiceAsync(int index)
        {
            await SendHoldMessageAsync(index);
            ThisCup!.HoldUnholdDice(index);
            await AfterHoldUnholdDiceAsync();
        }
        public async Task ProcessHoldReceivedAsync(int id)
        {
            await HoldUnholdDiceAsync(id);
        }
        public virtual async Task AfterHoldUnholdDiceAsync()
        {
            await ContinueTurnAsync();
        }
        public async Task AfterRollingAsync() //done.
        {
            SaveRoot!.RollNumber++; //try there instead.
            await ProtectedAfterRollingAsync();
        }
        abstract protected Task ProtectedAfterRollingAsync();
        public virtual async Task AfterSelectUnselectDiceAsync()
        {
            await ContinueTurnAsync();
        }
    }
}