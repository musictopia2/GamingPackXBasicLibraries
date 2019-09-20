using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks;
namespace BasicGameFramework.MainViewModels
{
    public abstract class BasicMultiplayerVM<P, G> : BaseViewModel, IBasicGameVM,
        ISimpleMultiPlayerVM, IRoundCommand, IEnableAlways, IBasicEnableProcess
        where P : class, IPlayerItem, new()
        where G : class, IBasicGameProcesses<P>, IEndTurn

    {
        private bool _NewRoundVisible;
        public bool NewRoundVisible
        {
            get { return _NewRoundVisible; }
            set
            {
                if (SetProperty(ref _NewRoundVisible, value))
                    NewRoundCommand.ReportCanExecuteChange();
            }
        }
        public PlainCommand? NewGameCommand { get; set; } //this one for sure is plain.
        public PlainCommand NewRoundCommand { get; set; }
        public BasicGameCommand EndTurnCommand { get; set; }
        public virtual bool CanEndTurn()
        {
            return true; //on default can end turn.  but there are exceptions.
        }
        public CommandContainer? CommandContainer { get; set; }
        public IGamePackageResolver? MainContainer { get; set; }
        protected BasicData? ThisData;
        private IRequestNewGameRound? _thisLoad;
        public virtual bool CanEnableAlways()
        {
            return true;
        }
        public virtual bool CanEnableBasics()
        {
            if (NewGameVisible == true || NewRoundVisible == true)
                return false;
            return true;
        }
        private bool _NewGameVisible;
        public bool NewGameVisible
        {
            get { return _NewGameVisible; }
            set
            {
                if (SetProperty(ref _NewGameVisible, value))
                    NewGameCommand!.ReportCanExecuteChange();
            }
        }
        private string _Status = "";
        public string Status
        {
            get { return _Status; }
            set
            {
                if (SetProperty(ref _Status, value)) { }
            }
        }

        private string _NormalTurn = "";
        public string NormalTurn
        {
            get { return _NormalTurn; }
            set
            {
                if (SetProperty(ref _NormalTurn, value)) { }
            }
        }
        private bool _MainOptionsVisible;
        public bool MainOptionsVisible
        {
            get { return _MainOptionsVisible; }
            set
            {
                if (SetProperty(ref _MainOptionsVisible, value)) { }
            }
        }
        protected override Task CustomErrorHandler(Exception ex)
        {
            if (ThisTest!.ShowErrorMessageBoxes == true)
            {
                ThisMessage.ShowError(ex.Message);
                return Task.CompletedTask;
            }
            return base.CustomErrorHandler(ex);
        }
        public async Task ShowGameMessageAsync(string thisStr)
        {
            if (ThisTest!.NoCommonMessages == true)
                return; //because you chose no common messages.
            await ThisMessage.ShowMessageBox(thisStr);
        }
        protected INetworkMessages? ThisNet;
        protected G? MainGame;
        protected EventAggregator? ThisE;
        protected TestOptions? ThisTest;
        public BasicMultiplayerVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) //if you have other interface definitions, add here
        {
            ThisMessage = tempUI;
            MainContainer = tempC; //still need it for the new game command.
            CommandContainer = MainContainer.Resolve<CommandContainer>();
            CommandContainer.ManuelFinish = true;
            CommandContainer.IsExecuting = true; //has to be proven false.
            NewGameCommand = new PlainCommand(async items =>
            {
                await _thisLoad!.NewGameFromCommandAsync();
            }, items =>
            {
                if (thisData.MultiPlayer == true && thisData.Client == true)
                    return false;
                return NewGameVisible;
            }, this, CommandContainer);
            NewRoundCommand = new PlainCommand(async items =>
            {
                await _thisLoad!.NewRoundFromCommandAsync();
            }, items =>
            {
                if (thisData.MultiPlayer == true && thisData.Client == true)
                    return false;
                return NewRoundVisible;
            }, this, CommandContainer);
            EndTurnCommand = new BasicGameCommand(this, async items =>
            {
                await EndTurnProcess();
            }, items => CanEndTurn(), this, CommandContainer);
        }
        protected virtual async Task EndTurnProcess() // games like tile rummy has to do something else for the end turn.
        {
            if (ThisData!.MultiPlayer == true)
            {
                await PreviewEndTurnMultiplayerAsync(); // sometimes this is needed but not always
                await ThisNet!.SendEndTurnAsync();
            }
            await MainGame!.EndTurnAsync();
        }
        protected virtual Task PreviewEndTurnMultiplayerAsync()
        {
            return Task.CompletedTask;
        }
        public void Init()
        {
            MainGame = MainContainer!.Resolve<G>();
            ThisE = MainContainer.Resolve<EventAggregator>();
            ThisData = MainContainer.Resolve<BasicData>();
            if (ThisData.GamePackageMode == EnumGamePackageMode.None)
                throw new BasicBlankException("You need to specify whether its debug or reals");
            IGamePackageDIContainer thisTemp = MainContainer.Resolve<IGamePackageDIContainer>();
            MainGame.RegisterOpening(thisTemp);
            thisTemp.RegisterType<RestoreVM>(true); //i think here too.
            ThisTest = MainContainer.Resolve<TestOptions>();
            if (ThisData.GamePackageMode == EnumGamePackageMode.Production)
            {
                if (ThisTest.AllowAnyMove == true)
                    throw new BasicBlankException("Can't allow any move because its production");
                if (ThisTest.AutoNearEndOfDeckBeginning == true)
                    throw new BasicBlankException("Can't be near the end of deck at beginning because its production");
                if (ThisTest.CardsToPass != 0)
                    throw new BasicBlankException("Cannot pass a special number of cards becuase its production");
                if (ThisTest.ComputerEndsTurn == true)
                    throw new BasicBlankException("The computer cannot just end turn because its production.  Try setting another property");
                if (ThisTest.ComputerNoCards == true)
                    throw new BasicBlankException("The computer has to have cards because its production");
                if (ThisTest.DoubleCheck == true)
                    throw new BasicBlankException("No double checking anything because its production");
                if (ThisTest.ImmediatelyEndGame == true)
                    throw new BasicBlankException("Cannot immediately end game because its production");
                if (ThisTest.NoAnimations == true)
                    throw new BasicBlankException("Animations are required in production.");
                if (ThisTest.NoCommonMessages == true)
                    throw new BasicBlankException("Must have common messages because its production");
                if (ThisTest.NoComputerPause == true)
                    throw new BasicBlankException("The computer cannot pause because its production");
                if (ThisTest.PlayCategory != EnumPlayCategory.Normal)
                    throw new BasicBlankException("The play category must be none because its production");
                if (ThisTest.SaveOption != EnumTestSaveCategory.Normal)
                    throw new BasicBlankException("The save mode must be normal because its production");
                if (ThisTest.ShowErrorMessageBoxes == false)
                    throw new BasicBlankException("Must show error message boxes because its production");
                if (ThisTest.WhoStarts != 1)
                    throw new BasicBlankException("WhoStarts must start with 1 because its production");
            }
            ThisNet = MainContainer.Resolve<INetworkMessages>();
            _thisLoad = MainContainer.Resolve<IRequestNewGameRound>();
            MainGame.Init(); //decided to do here.  that way it can stop some of the overflow errors.
            EndInit();
        }
        protected virtual void EndInit() { }
    }
}