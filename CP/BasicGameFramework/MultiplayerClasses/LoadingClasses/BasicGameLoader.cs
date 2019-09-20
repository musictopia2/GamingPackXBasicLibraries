using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    public class BasicGameLoader<P, S> : INewGameNM, ILoadGameNM,
        IStartMultiPlayerGame<P>, IRestoreMultiPlayerGame, IRequestNewGameRound
        where P : class, IPlayerItem, new()
        where S : BasicSavedGameClass<P> //not sure if new argument is needed (?)
    {
        private readonly BasicData _thisData; //not sure if i need overriding or not.
        private readonly IGameInfo _thisGame;
        private readonly IMultiplayerSaveState _thisState; //we for sure need it for restoring saved games.
        private readonly IGamePackageResolver _mainContainer;
        private readonly TestOptions _thisTest;
        private readonly IGameSetUp<P, S> _thisUp;
        private readonly EventAggregator _thisE;
        public BasicGameLoader(BasicData thisData, IGameInfo thisGame,
            IMultiplayerSaveState thisState, IGamePackageResolver mainContainer,
            TestOptions thisTest, IGameSetUp<P, S> thisUp, EventAggregator thisE)
        {
            _thisData = thisData;
            _thisGame = thisGame;
            _thisState = thisState;
            _mainContainer = mainContainer;
            _thisTest = thisTest;
            _thisUp = thisUp;
            thisUp.ThisLoader = this;
            _thisE = thisE;
            _thisUp.ComputerEndsTurn = _thisTest.ComputerEndsTurn || _thisGame.PlayerType == EnumPlayerType.NetworkOnly;
        }
        async Task ILoadGameNM.LoadGameAsync(string data)
        {
            _thisData.MultiPlayer = true;
            _thisData.Client = true; //you are a client because you received data to load a game.
            SetUpNetworking();
            _thisUp.SaveRoot = await js.DeserializeObjectAsync<S>(data);
            _thisUp.PlayerList = _thisUp.SaveRoot.PlayerList; //do this first.
            _thisUp.PlayerList.FixNetworkedPlayers(_thisData.NickName);
            await FinishGetSavedAsync();
            await _thisE.SendLoadAsync();
            await FinishStartAsync();
            _thisUp.CurrentMod.CommandContainer!.Processing = false;
        }
        private void SetUpNetworking()
        {
            if (_thisData.MultiPlayer == true && _thisUp.ThisCheck == null)
            {
                _thisUp.ThisNet = _mainContainer.Resolve<INetworkMessages>();
                _thisUp.ThisCheck = _mainContainer.Resolve<IMessageChecker>();
            }
        }
        private async Task PrivateLoadPlayers(PlayerCollection<P> startList)
        {
            SetUpNetworking();
            _thisUp.SaveRoot!.PlayOrder = (PlayOrderClass)_mainContainer.Resolve<IPlayOrder>();
            if (_mainContainer.ObjectExist<ISetObjects>())
            {
                ISetObjects ThisUp = _mainContainer.Resolve<ISetObjects>();
                await ThisUp.SetSaveRootObjectsAsync(); //this is something a person has to do.
            }
            if (startList.GetTemporaryCount == 0)
            {
                await FinishLoadingAsync(true);
                _thisUp.CurrentMod.CommandContainer!.Processing = false;
                return;
            }
            bool rets;
            if (_thisTest.PlayCategory == EnumPlayCategory.Normal)
                rets = true;
            else
                rets = false;
            startList.FinishLoading(rets);
            if (_thisData.MultiPlayer == true)
                startList.FixNetworkedPlayers(_thisData.NickName); //i think.
            _thisUp.SaveRoot.PlayOrder.WhoTurn = _thisTest.WhoStarts;
            _thisUp.SaveRoot.PlayerList = startList;
            _thisUp.PlayerList = startList;
            await FinishLoadingAsync(true);
            _thisUp.CurrentMod.CommandContainer!.Processing = false;
        }
        async Task IStartMultiPlayerGame<P>.LoadNewGameAsync(PlayerCollection<P> startList)
        {
            await PrivateLoadPlayers(startList);
        }
        private async Task FinishLoadingAsync(bool isBeginning)
        {
            if (_thisUp.SaveRoot!.PlayerList == null)
            {
                await _thisUp.SetUpGameAsync(isBeginning);
                return; //because there was no players.  games like three letter fun has that possibility.
            }
            if (_thisUp.SaveRoot.PlayerList.Count() == 0 && _thisGame.SinglePlayerChoice != EnumPlayerChoices.Solitaire)
                throw new BasicBlankException("There was no players.  Rethink");
            if (isBeginning == false)
            {
                _thisUp.SaveRoot.PlayerList.ForEach(items => items.InGame = items.CanStartInGame);
                _thisUp.SaveRoot.PlayOrder.WhoTurn = _thisUp.SaveRoot.PlayOrder.WhoStarts; //this was wrong too.
                _thisUp.SaveRoot.PlayOrder.WhoTurn = await _thisUp.SaveRoot.PlayerList.CalculateWhoTurnAsync();
            }
            _thisUp.SaveRoot.PlayOrder.WhoStarts = _thisUp.SaveRoot.PlayOrder.WhoTurn;
            await _thisUp.SetUpGameAsync(isBeginning);
        }
        public async Task FinishUpAsync(bool isBeginning)
        {
            await Step1FinishAsync(isBeginning);
            if (isBeginning == true)
                await _thisE.SendLoadAsync();
            await FinishStartAsync();
        }
        private async Task Step1FinishAsync(bool isBeginning)
        {
            if (_thisData.MultiPlayer == true && _thisData.Client == false) //i think this has to be here.
            {
                await _thisUp.PopulateSaveRootAsync(); //has to be right before sending it.
                if (isBeginning == true)
                    await _thisUp.ThisNet!.SendLoadGameMessageAsync(_thisUp.SaveRoot!);
                else
                    await _thisUp.ThisNet!.SendNewGameAsync(_thisUp.SaveRoot!); //maybe okay if its new game.
            }
        }
        private async Task FinishStartAsync()
        {
            bool rets;
            rets = _mainContainer.ObjectExist<IFinishStart>();
            if (rets == true)
            {
                IFinishStart ThisFinish = _mainContainer.Resolve<IFinishStart>();
                await ThisFinish.FinishStartAsync();
            }
            _thisUp.StartingStatus();
            if (_thisUp.PlayerList?.Count > 0)
                _thisUp.SingleInfo = _thisUp.PlayerList.GetWhoPlayer();
            if (_thisUp.CanMakeMainOptionsVisibleAtBeginning == true)
                _thisUp.CurrentMod.MainOptionsVisible = true; //if we don't do main options visible to begin with, rethink.
            _thisUp.ShowTurn();
            if (_thisUp.SaveRoot!.ImmediatelyStartTurn)
                await _thisUp.StartNewTurnAsync(); //this is the best way to handle this.
            else
                await _thisUp.ContinueTurnAsync();
        }
        private async Task<bool> GetSavedRootAsync()
        {
            string ThisStr = await _thisState.SavedDataAsync();
            if (ThisStr == "")
            {
                _thisUp.SaveRoot = _mainContainer.Resolve<S>(); //if you do right away, no need to communicate with anybody.
                return false;
            }
            _thisUp.SaveRoot = await js.DeserializeObjectAsync<S>(ThisStr);
            return true;
        }
        async Task IStartMultiPlayerGame<P>.LoadSavedGameAsync()
        {
            await PrivateLoadSavedGameAsync();
        }
        private async Task PrivateLoadSavedGameAsync()
        {
            SetUpNetworking();
            bool rets;
            rets = await GetSavedRootAsync();
            if (rets == false)
                throw new BasicBlankException("You should have loaded new game now");
            await FinishHostSavedfAsync();
        }
        private async Task FinishHostSavedfAsync()
        {
            await FinishGetSavedAsync();
            if (_thisData.MultiPlayer == true)
                await _thisUp.ThisNet!.SendLoadGameMessageAsync(_thisUp.SaveRoot!);
            await _thisE.SendLoadAsync();
            await FinishStartAsync();
            _thisUp.CurrentMod.CommandContainer!.Processing = false;
        }
        private async Task FinishGetSavedAsync()
        {
            _mainContainer.ReplaceObject(_thisUp.SaveRoot);
            if (_thisUp.SaveRoot!.PlayerList != null)
            {
                _thisUp.SaveRoot.PlayerList.MainContainer = _mainContainer; //has to redo that part.
                _thisUp.SaveRoot.PlayerList.AutoSaved(_thisUp.SaveRoot.PlayOrder);
                _thisUp.PlayerList = _thisUp.SaveRoot.PlayerList;
            }
            await _thisUp.FinishGetSavedAsync();
        }
        async Task IRequestNewGameRound.NewGameFromCommandAsync() //looks like this was easy after all.
        {
            _thisUp.CurrentMod.NewGameVisible = false; //because its not new game anymore.
            _thisUp.CurrentMod.CommandContainer!.ManuelFinish = true; //has to manually be done i think (?)
            if (_thisGame.GameType == EnumGameType.Rounds)
            {
                IStartNewGame Temps = _mainContainer.Resolve<IStartNewGame>();
                await Temps.ResetAsync();
            }
            await FinishLoadingAsync(false);
            _thisUp.CurrentMod.CommandContainer.Processing = false;
        }
        async Task INewGameNM.NewGameReceivedAsync(string data) //can't put here anymore for setup networking.  causes a different problem later.
        {
            await UpdateGameAsync(data);
        }
        private async Task UpdateGameAsync(string data)
        {
            _thisUp.SaveRoot = await js.DeserializeObjectAsync<S>(data);
            _thisUp.PlayerList = _thisUp.SaveRoot.PlayerList; //do this first.
            _thisUp.PlayerList.FixNetworkedPlayers(_thisData.NickName);
            await FinishGetSavedAsync();
            await _thisE.SendUpdateAsync();
            await FinishStartAsync();
            _thisUp.CurrentMod.CommandContainer!.Processing = false;
        }
        async Task IRequestNewGameRound.NewRoundFromCommandAsync()
        {
            if (_thisGame.GameType == EnumGameType.NewGame)
                throw new BasicBlankException("Rounds was never supported for this game.  Therefore, should have never been allowed");
            _thisUp.CurrentMod.NewRoundVisible = false; //we know this part for sure.
            _thisUp.CurrentMod.CommandContainer!.ManuelFinish = true; //has to manually be done i think (?)
            await FinishLoadingAsync(false);
            _thisUp.CurrentMod.CommandContainer.Processing = false;
        }
        async Task IRestoreMultiPlayerGame.RestoreGameAsync()
        {
            await GetSavedRootAsync();
            await FinishGetSavedAsync();
            if (_thisData.MultiPlayer == true)
                await _thisUp.ThisNet!.SendRestoreGameAsync(_thisUp.SaveRoot!);
            await _thisE.SendUpdateAsync();
            await FinishStartAsync();
        }
        async Task IRestoreMultiPlayerGame.RestoreMessageAsync(string Data)
        {
            await UpdateGameAsync(Data);
        }
    }
}