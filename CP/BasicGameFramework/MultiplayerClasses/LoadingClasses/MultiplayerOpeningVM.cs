using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    public class MultiplayerOpeningVM<P, S> : ObservableObject, IOpeningMessenger, IReadyNM
        where P : class, IPlayerItem, new()
        where S : BasicSavedGameClass<P>, new()
    {
        private readonly ISimpleUI _thisUI;
        private INetworkMessages? _thisNet;
        private readonly BasicData _thisData;
        private readonly IBasicGameVM _thisMod;
        private readonly IGameInfo _thisGame;
        private readonly IMultiplayerSaveState _thisSave;
        private PlayerCollection<P> _playerList = new PlayerCollection<P>();
        public bool HasNetwork { get; private set; }

        private bool _ExtraOptionsVisible;
        public bool ExtraOptionsVisible
        {
            get { return _ExtraOptionsVisible; }
            set
            {
                if (SetProperty(ref _ExtraOptionsVisible, value)) { }
            }
        }
        public int ClientsConnected
        {
            get
            {
                if (_playerList.GetTemporaryCount == 0)
                    return 0;// if no players, then its obviously 0 still.
                return _playerList.GetTemporaryCount - 1; // i think
            }
        }
        public bool HostCanStart => _thisData.NetworkStatus == EnumNetworkStatus.HostingReadyToStart;
        public bool CanShowSingleOptions => _thisData.NetworkStatus == EnumNetworkStatus.None;

        private int _PreviousNonComputerNetworkedPlayers;
        public int PreviousNonComputerNetworkedPlayers
        {
            get { return _PreviousNonComputerNetworkedPlayers; }
            set
            {
                if (SetProperty(ref _PreviousNonComputerNetworkedPlayers, value)) { }
            }
        }
        private void ShowOtherChangesBecauseOfNetworkChange()
        {
            if (_thisData.NetworkStatus == EnumNetworkStatus.HostingReadyToStart)
                ExtraOptionsVisible = _thisGame.CanHaveExtraComputerPlayers;
            else
                ExtraOptionsVisible = false;
            OnPropertyChanged(nameof(HostCanStart));
            OnPropertyChanged(nameof(ClientsConnected)); // needs to be readonly so nobody can update.
            OnPropertyChanged(nameof(CanShowSingleOptions));
        }
        public OpenCommand ResumeSinglePlayerCommand { get; set; }
        public OpenCommand ResumeMultiplayerGameCommand { get; set; }
        public OpenCommand<int> ComputerCommand { get; set; } //how many computer players
        public OpenCommand<int> HumanCommand { get; set; } //how many human players.
        public OpenCommand<int> StartCommand { get; set; } //the host chooses this to actually start the game.
        public OpenCommand HostCommand { get; set; } //this means you are hosting a game.
        public OpenCommand ConnectCommand { get; set; } //this means you are connecting to host.
        public OpenCommand SolitaireCommand { get; set; } //this is when its solitaire type like three letter fun game.
        public OpenCommand CancelCommand { get; set; } //only the host can cancel.
        private EnumRestoreCategory _singleRestore;
        private EnumRestoreCategory _multiRestore;
        private readonly TestOptions _thisTest;
        private PlayerCollection<P>? _saveList;
        private bool _Visible;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value)) { }
            }
        }
        public async Task FinishStartAsync()
        {
            _singleRestore = await _thisSave.SinglePlayerRestoreCategoryAsync();
            _multiRestore = await _thisSave.MultiplayerRestoreCategoryAsync();
            if (_thisMod.MainContainer!.ObjectExist<INetworkMessages>() == true)
            {
                _thisNet = _thisMod.MainContainer.Resolve<INetworkMessages>(); //the problem for overflow is in the network part.
                HasNetwork = true;
            }
            else
                HasNetwork = false;

            if (_multiRestore != EnumRestoreCategory.NoRestore)
            {
                string thisStr = await _thisSave.TempMultiSavedAsync();
                if (thisStr == "")
                    throw new BasicBlankException("You should have not called temporary multisaved because there was none");
                S SaveRoot = await js.DeserializeObjectAsync<S>(thisStr); //hopefully this still works.
                _saveList = SaveRoot.PlayerList;
                _saveList.RemoveNonHumanPlayers();
            }
            Reset();
            Visible = true;
        }
        private bool SaveReconciles()
        {
            CustomBasicList<P> temporaryList = _playerList.GetTemporaryList();
            return temporaryList.DoesReconcile(_saveList, Items => Items.NickName);
        }
        private void Reset()
        {
            _thisMod.CommandContainer!.OpenBusy = false;
            _thisData.NetworkStatus = EnumNetworkStatus.None;
            ShowOtherChangesBecauseOfNetworkChange(); //i think here too.
        }
        public MultiplayerOpeningVM(IBasicGameVM thisMod, ISimpleUI thisUI, BasicData thisData,
            IGameInfo thisGame, IMultiplayerSaveState thisSave, TestOptions thisTest)
        {
            thisMod.CommandContainer!.OpenBusy = true;
            _thisMod = thisMod; //looks like i did need the view model because it needs the new game if single player game.
            _thisUI = thisUI;
            _thisData = thisData;
            _thisGame = thisGame;
            _thisSave = thisSave;
            _thisTest = thisTest;
            ComputerCommand = new OpenCommand<int>(async items =>
            {
                await StartComputerSinglePlayerGameAsync(items);
            }, items =>
            {
                if (thisData.NetworkStatus != EnumNetworkStatus.None)
                    return false;
                if (_singleRestore == EnumRestoreCategory.MustRestore)
                    return false; //because you have to restore.
                return CanComputer;
            }, thisMod, thisMod.CommandContainer);
            HumanCommand = new OpenCommand<int>(async items =>
            {
                await StartPassAndPlayGameAsync(items);
            }, items =>
            {
                if (thisData.NetworkStatus != EnumNetworkStatus.None)
                    return false;
                if (_singleRestore == EnumRestoreCategory.MustRestore)
                    return false; //because you have to restore.
                return CanHuman;
            }, thisMod, thisMod.CommandContainer);
            ResumeSinglePlayerCommand = new OpenCommand(async items =>
            {
                await StartSavedGameAsync();
            }, items =>
            {
                if (thisData.NetworkStatus != EnumNetworkStatus.None)
                    return false;
                return _singleRestore != EnumRestoreCategory.NoRestore;
            }, thisMod, thisMod.CommandContainer);
            SolitaireCommand = new OpenCommand(async items =>
            {
                await StartSolitaireGameAsync();
            }, items =>
            {
                if (thisData.NetworkStatus != EnumNetworkStatus.None)
                    return false;
                if (_singleRestore == EnumRestoreCategory.MustRestore)
                    return false; //because you have to restore.
                return thisGame.SinglePlayerChoice == EnumPlayerChoices.Solitaire;
            }, thisMod, thisMod.CommandContainer);
            ResumeMultiplayerGameCommand = new OpenCommand(async items =>
            {
                thisData.MultiPlayer = true; //to make sure its multiplayer now.
                await StartSavedGameAsync();
            }, items =>
            {
                if (thisData.NetworkStatus != EnumNetworkStatus.HostingReadyToStart)
                    return false;
                if (_multiRestore == EnumRestoreCategory.NoRestore)
                    return false;
                return SaveReconciles();

            }, thisMod, thisMod.CommandContainer);
            CancelCommand = new OpenCommand(async items =>
            {
                thisData.NetworkStatus = EnumNetworkStatus.None;
                await _thisNet!.CloseConnectionAsync(); //i think this will close out obviously.
                thisMod.CommandContainer.OpenBusy = false; //no longer busy.  had to set manually.
            }, items =>
            {
                return thisData.NetworkStatus == EnumNetworkStatus.HostingWaitingForAtLeastOnePlayer;
            }, thisMod, thisMod.CommandContainer);
            HostCommand = new OpenCommand(async items =>
            {
                bool rets = await _thisNet!.InitNetworkMessagesAsync(thisData.NickName, false); //because you are hosting.
                if (rets == false)
                {
                    await thisMod.ShowGameMessageAsync("Failed To Connect To Server");
                    Reset();
                    return;
                }
            }, Items => thisData.NetworkStatus == EnumNetworkStatus.None, thisMod, thisMod.CommandContainer);
            ConnectCommand = new OpenCommand(async items =>
            {
                bool rets = await _thisNet!.InitNetworkMessagesAsync(thisData.NickName, true);
                if (rets == false)
                {
                    await thisMod.ShowGameMessageAsync("Failed To Connect To Server");
                    Reset();
                    return;
                }
                await _thisNet.ConnectToHostAsync(); //this will connect to host.
                Visible = false; //this will now show not visible because too late to do anything.
            }, items => thisData.NetworkStatus == EnumNetworkStatus.None, thisMod, thisMod.CommandContainer);
            StartCommand = new OpenCommand<int>(async items =>
            {
                if (items > 0)
                    _playerList.LoadPlayers(0, items);
                thisData.MultiPlayer = true;
                await StartNewGameAsync();
            }, howManyExtra =>
            {
                if (_multiRestore == EnumRestoreCategory.MustRestore)
                    return false; //in this case, you can't no matter what because you must restore.
                int TempCount = _playerList.GetTemporaryCount;
                if (TempCount == 0)
                    return false;
                if (howManyExtra > 0 && thisGame.CanHaveExtraComputerPlayers == false)
                    return false;// because can't have extra computer players.
                if (howManyExtra > 0)
                {
                    if (howManyExtra + TempCount > thisGame.MaxPlayers)
                        return false;
                    if (howManyExtra + TempCount < thisGame.MinPlayers)
                        return false;
                    if (howManyExtra + TempCount == thisGame.NoPlayers)
                        return false;
                }
                return thisData.NetworkStatus == EnumNetworkStatus.HostingReadyToStart; //i think
            }, thisMod, thisMod.CommandContainer);
        }
        private void StartSingle()
        {
            _thisData.NetworkStatus = EnumNetworkStatus.SinglePlayer;
            _thisData.MultiPlayer = false;
            _playerList = new PlayerCollection<P>();
        }
        private async Task StartComputerSinglePlayerGameAsync(int howManyComputerPlayers)
        {
            StartSingle();
            bool rets;
            if (_thisTest.PlayCategory == EnumPlayCategory.Reverse)
                rets = true;
            else
                rets = false;
            _playerList.LoadPlayers(1, howManyComputerPlayers, rets); //i think
            await StartNewGameAsync();
        }
        private async Task StartSolitaireGameAsync()
        {
            StartSingle(); //i think this too.
            await StartNewGameAsync();
        }
        private async Task StartPassAndPlayGameAsync(int howManyHumanPlayers)
        {
            StartSingle();
            _playerList.LoadPlayers(howManyHumanPlayers); //i think
            await StartNewGameAsync();
        }
        private async Task StartSavedGameAsync()
        {
            IStartMultiPlayerGame<P> ThisPlay = _thisMod.MainContainer!.Resolve<IStartMultiPlayerGame<P>>();
            Visible = false;
            await ThisPlay.LoadSavedGameAsync();
        }
        private async Task StartNewGameAsync()
        {
            IStartMultiPlayerGame<P> ThisPlay = _thisMod.MainContainer!.Resolve<IStartMultiPlayerGame<P>>();
            await _thisSave.DeleteGameAsync(); //will delete any autosaved game at this point.
            Visible = false;
            await ThisPlay.LoadNewGameAsync(_playerList);
        }
        public bool CanComputer
        {
            get
            {
                if (_thisGame.SinglePlayerChoice == EnumPlayerChoices.ComputerOnly)
                    return true;
                if (_thisGame.SinglePlayerChoice == EnumPlayerChoices.Either)
                    return true;
                return false; //if i am wrong, rethink.
            }
        }
        public bool CanHuman
        {
            get
            {
                if (_thisGame.SinglePlayerChoice == EnumPlayerChoices.HumanOnly)
                    return true;
                if (_thisGame.SinglePlayerChoice == EnumPlayerChoices.Either)
                    return true;
                return false; //could require rethinking (?)
            }
        }
        public CustomBasicList<int> GetPossiblePlayers()
        {
            int x;
            x = 0;
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            if (_thisGame.MinPlayers == 3)
                tempList.Add(1); //because you would already have at least 2 players.
            do
            {
                x += 1;
                if (x > _thisGame.MaxPlayers)
                    break;
                if (x + 1 >= _thisGame.MinPlayers && x + 1 != _thisGame.NoPlayers)
                    tempList.Add(x);
            }
            while (true);
            tempList.RemoveLastItem(); //i think
            return tempList;
        }
        public async Task ConnectedToHostAsync(IMessageChecker thisCheck, string hostName)
        {
            await _thisNet!.SendReadyMessageAsync(thisCheck.NickName, hostName);
            _thisData.NetworkStatus = EnumNetworkStatus.ConnectedToHost;
            _thisData.Client = true;
            _thisData.MultiPlayer = true; //we do know its multiplayer for sure.
            _thisData.NickName = thisCheck.NickName;
            thisCheck.IsEnabled = true; //because you will receive more information from host.
            ShowOtherChangesBecauseOfNetworkChange();
        }
        public Task HostConnectedAsync(IMessageChecker thisCheck)
        {
            _thisData.Client = false;
            thisCheck.IsEnabled = true; //so it can process the message from client;
            _thisData.NetworkStatus = EnumNetworkStatus.HostingWaitingForAtLeastOnePlayer;
            _playerList = new PlayerCollection<P>(); //i think here too.
            P thisPlayer = new P();
            thisPlayer.NickName = _thisData.NickName;
            thisPlayer.IsHost = true;
            thisPlayer.Id = 1;
            thisPlayer.InGame = true; //you have to show you are in game obviously to start with.
            thisPlayer.PlayerCategory = EnumPlayerCategory.OtherHuman;
            _playerList.AddPlayer(thisPlayer);
            _thisMod.CommandContainer!.OpenBusy = false; //because you may decide to cancel.
            if (_saveList != null)
                PreviousNonComputerNetworkedPlayers = _saveList.Count() - 1; //i think
            return Task.CompletedTask;
        }
        public async Task HostNotFoundAsync(IMessageChecker thisCheck)
        {
            await _thisUI.ShowMessageBox("Unable to connect to host");
            _thisData.NetworkStatus = EnumNetworkStatus.None; //i think.
            thisCheck.IsEnabled = true; //so can try again later.
            Visible = true; //because its not visible.
            ShowOtherChangesBecauseOfNetworkChange();
            _thisMod.CommandContainer!.OpenBusy = false;
        }
        public Task ProcessReadyAsync(string nickName)
        {
            P thisPlayer = new P();
            thisPlayer.NickName = nickName;
            thisPlayer.IsHost = false;
            thisPlayer.Id = _playerList.GetTemporaryCount + 1;
            thisPlayer.InGame = true; //you have to be in game obviously.
            thisPlayer.PlayerCategory = EnumPlayerCategory.OtherHuman; //for now, just set to other.
            _playerList.AddPlayer(thisPlayer);
            _thisData.NetworkStatus = EnumNetworkStatus.HostingReadyToStart; //now he is ready to start.
            ShowOtherChangesBecauseOfNetworkChange();
            StartCommand.ReportCanExecuteChange(); //has to manually be done this time.
            ResumeMultiplayerGameCommand.ReportCanExecuteChange(); //this had to also be manually done.
            return Task.CompletedTask;
        }
    }
}