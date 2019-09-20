using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    /// <summary>
    /// this class is used for basic games.  well see if everything derives from this or not.
    /// if the game has rounds, then the game itself has to implement the interface for rounds.
    /// </summary>
    public abstract class BasicGameClass<P, S>
        : IGameSetUp<P, S>, IEndTurn, IEndTurnNM
         where P : class, IPlayerItem, new()

        where S : BasicSavedGameClass<P>, new()
    {
        public BasicGameClass(IGamePackageResolver container)
        {
            MainContainer = container;
            _thisMod = MainContainer.Resolve<ISimpleMultiPlayerVM>(); //if the mod has to be something else, the inherited version will cast it.
            ThisE = MainContainer.Resolve<EventAggregator>();
        }
        private bool _computerEndsTurn = false;
        bool IGameSetUp<P, S>.ComputerEndsTurn
        {
            set
            {
                _computerEndsTurn = value;
            }
        }
        public IGamePackageResolver MainContainer { get; set; }
        public S? SaveRoot { get; set; }
        protected bool IsLoaded; //so when loaded, then can act differently.
        protected readonly EventAggregator ThisE;
        private readonly ISimpleMultiPlayerVM _thisMod; //decided to keep it as is for now.
        public BasicGameLoader<P, S>? ThisLoader { get; set; }
        public PlayerCollection<P>? PlayerList { get; set; }
        protected IMultiplayerSaveState? ThisState; //not sure yet.
        //games like three letter fun needed to know the information about it.
        public BasicData? ThisData { private set; get; } //this way others can access this data.
        public IMessageChecker? ThisCheck { get; set; }
        public INetworkMessages? ThisNet { get; set; } //has to be completely open now.
        public TestOptions? ThisTest { private set; get; }
        public IAsyncDelayer? Delay;
        ISimpleMultiPlayerVM IBasicGameProcesses<P>.CurrentMod => _thisMod;
        public P? SingleInfo { get; set; } //this is whose turn it is.
#pragma warning disable IDE0052 // Remove unread private members
        public int WhoTurn //some games require it to be public like pass out dice game.
#pragma warning restore IDE0052 // Remove unread private members
        {
            set
            {
                SaveRoot!.PlayOrder.WhoTurn = value;
            }
            get
            {
                return SaveRoot!.PlayOrder.WhoTurn;
            }
        }
        protected int WhoStarts
        {
            get
            {
                return SaveRoot!.PlayOrder.WhoStarts;
            }
            set
            {
                SaveRoot!.PlayOrder.WhoStarts = value;
            }
        }
        public virtual Task PopulateSaveRootAsync()
        {
            return Task.CompletedTask; //most of the time, nothing but can be something
        }
        public virtual bool CanMakeMainOptionsVisibleAtBeginning => true; //so others can decide not to after all.
        /// <summary>
        /// this is after getting saved data.
        /// </summary>
        /// <returns></returns>
        public abstract Task FinishGetSavedAsync(); //changed my mind.
        public virtual Task EndTurnAsync() //this could change.
        {
            return Task.CompletedTask; //its up to each implementation to decide what to do.
        }
        /// <summary>
        /// you need to remember to have the loader finish up.  however, you can decide to call privatefinishasync as well.
        /// </summary>
        /// <param name="IsBeginning"></param>
        /// <returns></returns>
        public abstract Task SetUpGameAsync(bool isBeginning);
        protected async Task PrivateFinishAsync(bool isBeginning)
        {
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected virtual Task ComputerTurnAsync()
        {
            return Task.CompletedTask; //if you do nothing, will be stuck.
        }
        protected virtual void GetPlayerToContinueTurn()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
        }
        protected virtual Task LoadPossibleOtherScreensAsync() { return Task.CompletedTask; } //most of the time nothing but we reserve the option to do something if necessary including async.
        protected Task ShowHumanCanPlayAsync() //reserve the right to be async.
        {
            _thisMod.CommandContainer!.IsExecuting = false;
            _thisMod.CommandContainer.ManuelFinish = false; //does not have to manually be done anymore.
            return Task.CompletedTask;
        }
        public async Task SaveStateAsync()
        {
            if (ThisData!.MultiPlayer == true && ThisData.Client == true)
                return; //because clients can't save.  they don't even need the thisstate object
            await PopulateSaveRootAsync();
            await ThisState!.SaveStateAsync(SaveRoot!);
        }
        //on games like chinese checkers, the board processes has to call it.
        public virtual async Task ShowWinAsync() //because board games has to also do something else.
        {
            await this.ProtectedShowWinAsync();
            await this.ProtectedGameOverNextAsync(ThisState!);
        }
        protected async Task ShowWinAsync(string customMessage) //this is needed for games like millebournes.
        {
            this.ProtectedShowCustomWin(customMessage);
            await this.ProtectedGameOverNextAsync(ThisState!);
        }
        public virtual async Task ShowTieAsync() //because board games has to also do something else.
        {
            await this.ProtectedShowTieAsync();
            await this.ProtectedGameOverNextAsync(ThisState!);
        }
        protected async Task ShowLossAsync() //for games like old maid.
        {
            await this.ProtectedShowLossAsync();
            await this.ProtectedGameOverNextAsync(ThisState!);
        }
        public async virtual Task ContinueTurnAsync() //we do open the possibility of overriding if necessary.
        {
            await SaveStateAsync();
            GetPlayerToContinueTurn(); //usually will set to current turn but can be different.
            await LoadPossibleOtherScreensAsync();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                await ShowHumanCanPlayAsync();
                return;
            }
            _thisMod.CommandContainer!.ManuelFinish = true; //has to manually be done now.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
            {
                if (ThisData!.MultiPlayer == false || ThisData.Client == false)
                {
                    if (ThisTest!.ComputerEndsTurn == true && ThisData.MultiPlayer == true)
                        throw new BasicBlankException("If the computer player is going to skip their turns and its multiplayer, then why did you add extra computer players?");
                    if (_computerEndsTurn) //needs to try to figure out another situation where the computer can't go even without testing.
                    {
                        _thisMod.CommandContainer.Processing = false; //because its testing anyways
                        if (ThisTest.NoComputerPause == false)
                            await Delay!.DelayMilli(500); //i think you should see it showed computer player though.
                        await EndTurnAsync();
                        return; //forgot the return.
                    }
                    await ComputerTurnAsync();
                    return;
                }
            }
            if (ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Stuck because not multiplayer.  Rethink");
            ThisCheck!.IsEnabled = true; //to wait for messages again.
        }
        public virtual async Task EndTurnReceivedAsync(string data)
        {
            await EndTurnAsync(); //most of the time, simple but you can override if necessary.
        }
        protected virtual void PrepStartTurn() //board games has to do something else in addition.  so upon autoresume, it can still if color is not chosen to show colors to choose.
        {
            SaveRoot!.ImmediatelyStartTurn = false;
            SingleInfo = PlayerList!.GetWhoPlayer();
            this.ShowTurn();
        }
        public abstract Task StartNewTurnAsync(); //decided to make it abstract this time.
        public virtual void Init() //instead of going into the new, has to go here.  that will stop some overflow errors.
        {
            SaveRoot = MainContainer.Resolve<S>();
            ThisState = MainContainer.Resolve<IMultiplayerSaveState>(); //i think this is needed too.
            PlayerList = SaveRoot.PlayerList;
            ThisData = MainContainer.Resolve<BasicData>();
            ThisTest = MainContainer.Resolve<TestOptions>();
            Delay = MainContainer.Resolve<IAsyncDelayer>(); //forgot this too.
        }
        public void RegisterOpening(IGamePackageDIContainer tempContainer)
        {
            tempContainer.RegisterType<MultiplayerOpeningVM<P, S>>(true);
        }
    }
}