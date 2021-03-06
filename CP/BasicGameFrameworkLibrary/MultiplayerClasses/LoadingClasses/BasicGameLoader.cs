﻿using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses
{
    public sealed class BasicGameLoader<P, S> : IStartMultiPlayerGame<P>, IClientUpdateGame, ILoadClientGame,
        IRequestNewGameRound, IRestoreMultiPlayerGame

        where P : class, IPlayerItem, new()
        where S : BasicSavedGameClass<P>, new() //i think the new one was needed after all.
    {
        private readonly BasicData _basic;
        private readonly IGameInfo _gameInfo;
        private readonly IMultiplayerSaveState _state;
        private readonly TestOptions _test;
        private readonly IEventAggregator _aggregator;
        private IGameSetUp<P, S>? _gameSetUp;
        private readonly IGamePackageResolver _resolver;
        private readonly CommandContainer _command;

        //has to see what we need for this to work properly.

        //i think the basic game container has to be also reset each time as well.  otherwise, won't be in sync anymore.

        //can't ask for it because this can be replaced.  that would cause serious problems.


        //the new part happens once.  however, the game does change.
        public BasicGameLoader(BasicData basic,
            IGameInfo gameInfo,
            IMultiplayerSaveState state,
            TestOptions test,
            IEventAggregator aggregator,
            IGamePackageResolver resolver,
            CommandContainer command
            )
        {
            _basic = basic;
            _gameInfo = gameInfo;
            _state = state;
            _test = test;
            _aggregator = aggregator;
            _resolver = resolver;
            _command = command;
        }

        private void SetGame()
        {
            _gameSetUp = _resolver.Resolve<IGameSetUp<P, S>>();
            _gameSetUp.ComputerEndsTurn = _test.ComputerEndsTurn || _gameInfo.PlayerType == EnumPlayerType.NetworkOnly;
            _gameSetUp.FinishUpAsync = FinishUpAsync;
        }

        private async Task FinishUpAsync(bool isBeginning)
        {
            if (GlobalHelpers.LoadGameScreenAsync == null)
            {
                throw new BasicBlankException("Did not set the load game function when finishing up.  Rethink");
            }
            await GlobalHelpers.LoadGameScreenAsync.Invoke(); //hopefully this simple.  risk it here
            await Step1FinishAsync(isBeginning);



            //because it creates each time, attempt to do load each time now.
            await _aggregator.SendLoadAsync();


            //if (isBeginning == true)
            //    await _thisE.SendLoadAsync();
            await FinishStartAsync();
        }

        private async Task Step1FinishAsync(bool isBeginning)
        {
            if (_basic.MultiPlayer == true && _basic.Client == false) //i think this has to be here.
            {
                await _gameSetUp!.PopulateSaveRootAsync(); //has to be right before sending it.
                if (isBeginning == true)
                    await _gameSetUp.Network!.SendLoadGameMessageAsync(_gameSetUp.SaveRoot!);
                else
                    await _gameSetUp.Network!.SendNewGameAsync(_gameSetUp.SaveRoot!); //maybe okay if its new game.
            }
        }

        async Task ILoadClientGame.LoadGameAsync(string payLoad)
        {
            SetGame(); //maybe here i need this as well.
            _basic.MultiPlayer = true;
            _basic.Client = true;
            _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(payLoad);
            _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
            _gameSetUp.PlayerList.FixNetworkedPlayers(_basic.NickName);

            await FinishGetSavedAsync();
            await _aggregator.SendLoadAsync();

            await FinishStartAsync();
            _command.Processing = false;
        }
        private async Task FinishGetSavedAsync()
        {
            _resolver.ReplaceObject(_gameSetUp!.SaveRoot);
            if (_gameSetUp.SaveRoot!.PlayerList != null)
            {
                _gameSetUp.SaveRoot.PlayerList.MainContainer = _resolver; //has to redo that part.
                _gameSetUp.SaveRoot.PlayerList.AutoSaved(_gameSetUp.SaveRoot.PlayOrder);
                _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList;
            }

            await _gameSetUp.FinishGetSavedAsync();
            //hopefully this is the correct place for the other part (?)
            if (GlobalHelpers.LoadGameScreenAsync == null)
            {
                throw new BasicBlankException("Did not set the load game function when getting saved or restored.  Rethink");
            }
            await GlobalHelpers.LoadGameScreenAsync.Invoke(); //hopefully this simple.  risk it here
        }

        private async Task FinishStartAsync()
        {
            bool rets;
            rets = _resolver.RegistrationExist<IFinishStart>();
            if (rets == true)
            {
                IFinishStart thisFinish = _resolver.Resolve<IFinishStart>();
                await thisFinish.FinishStartAsync();
            }
            _gameSetUp!.StartingStatus();
            if (_gameSetUp!.PlayerList?.Count > 0)
                _gameSetUp.SingleInfo = _gameSetUp.PlayerList.GetWhoPlayer();

            //hopefully the other part is not needed anymore.

            //if (_thisUp.CanMakeMainOptionsVisibleAtBeginning == true)
            //    _thisUp.CurrentMod.MainOptionsVisible = true; //if we don't do main options visible to begin with, rethink.
            _gameSetUp.ShowTurn();
            if (_gameSetUp.SaveRoot!.ImmediatelyStartTurn)
                await _gameSetUp.StartNewTurnAsync(); //this is the best way to handle this.
            else
                await _gameSetUp.ContinueTurnAsync();
        }



        async Task IStartMultiPlayerGame<P>.LoadNewGameAsync(PlayerCollection<P> startList)
        {
            SetGame();
            //if (GlobalHelpers.LoadGameScreenAsync == null)
            //{
            //    throw new BasicBlankException("Loading game screen was not set when starting a brand new game for host or for single player.  Rethink");
            //}
            //await GlobalHelpers.LoadGameScreenAsync.Invoke();
            _gameSetUp!.SaveRoot!.PlayOrder = (PlayOrderClass)_resolver.Resolve<IPlayOrder>();
            if (_resolver.RegistrationExist<ISetObjects>())
            {
                ISetObjects up = _resolver.Resolve<ISetObjects>();
                await up.SetSaveRootObjectsAsync(); //this is something a person has to do.
            }
            if (startList.GetTemporaryCount == 0)
            {
                await FinishLoadingAsync(true);
                _command.Processing = false;
                return;
            }
            bool rets;
            if (_test.PlayCategory == EnumPlayCategory.Normal)
                rets = true;
            else
                rets = false;
            startList.FinishLoading(rets);
            if (_basic.MultiPlayer == true)
                startList.FixNetworkedPlayers(_basic.NickName); //i think.
            _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _test.WhoStarts;
            _gameSetUp.SaveRoot.PlayerList = startList;
            _gameSetUp.PlayerList = startList;
            await FinishLoadingAsync(true);
            _command.Processing = false;
        }

        async Task IStartMultiPlayerGame<P>.LoadSavedGameAsync()
        {
            SetGame();
            bool rets;
            rets = await GetSavedRootAsync();
            if (rets == false)
                throw new BasicBlankException("You should have loaded new game now");
            await FinishHostSavedAsync();
        }
        private async Task<bool> GetSavedRootAsync()
        {
            string payLoad = await _state.SavedDataAsync<S>();
            if (payLoad == "")
            {
                _gameSetUp!.SaveRoot = _resolver.Resolve<S>(); //if you do right away, no need to communicate with anybody.
                return false;
            }
            _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(payLoad);
            return true;
        }

        private async Task FinishHostSavedAsync()
        {
            await FinishGetSavedAsync();
            if (_basic.MultiPlayer == true)
                await _gameSetUp!.Network!.SendLoadGameMessageAsync(_gameSetUp.SaveRoot!);
            await _aggregator.SendLoadAsync();
            await FinishStartAsync();
            _command.Processing = false;
        }

        async Task IRequestNewGameRound.RequestNewGameAsync()
        {

            SetGame();
            _command.ManuelFinish = true; //has to manually be done i think (?)
            _command.Processing = true; //i think this too.
            //we still need this after all.  because saveroot can't be deleted.
            if (_gameInfo.GameType == EnumGameType.Rounds)
            {
                IStartNewGame temps = _resolver.Resolve<IStartNewGame>();
                await temps.ResetAsync();
            }
            await FinishLoadingAsync(false);
            _command.Processing = false;
        }

        async Task IRequestNewGameRound.RequestNewRoundAsync()
        {
            if (_gameInfo.GameType == EnumGameType.NewGame)
                throw new BasicBlankException("Rounds was never supported for this game.  Therefore, should have never been allowed");
            //_gameSetUp.CurrentMod.NewRoundVisible = false; //we know this part for sure.
            //maybe its okay for the normal shell to load it in this case because its brand new.
            _command.ManuelFinish = true; //has to manually be done i think (?)



            //if (GlobalHelpers.LoadGameScreenAsync == null)
            //{
            //    throw new BasicBlankException("No loading screen function was set when requesting new round");
            //}
            //await GlobalHelpers.LoadGameScreenAsync.Invoke();
            await FinishLoadingAsync(false);
            _command.Processing = false;
        }

        private async Task UpdateGameAsync(string data)
        {
            SetGame();
            _gameSetUp!.SaveRoot = await js.DeserializeObjectAsync<S>(data);
            _gameSetUp.PlayerList = _gameSetUp.SaveRoot.PlayerList; //do this first.
            _gameSetUp.PlayerList.FixNetworkedPlayers(_basic.NickName);
            await FinishGetSavedAsync();
            await _aggregator.SendLoadAsync(); //try to do load again (?)
            //await _thisE.SendUpdateAsync();
            await FinishStartAsync();
            _command.Processing = false;
        }

        async Task IRestoreMultiPlayerGame.RestoreGameAsync()
        {
            SetGame(); //just in case.
            await GetSavedRootAsync();
            await FinishGetSavedAsync();
            if (_basic.MultiPlayer == true)
                await _gameSetUp!.Network!.SendRestoreGameAsync(_gameSetUp.SaveRoot!);
            //await _thisE.SendUpdateAsync();
            await _aggregator.SendLoadAsync(); //hopefully still okay since it would most likely erase the old view model.
            await FinishStartAsync();
        }

        async Task IClientUpdateGame.UpdateGameAsync(string payload)
        {
            SetGame();
            await UpdateGameAsync(payload);
        }

        private async Task FinishLoadingAsync(bool isBeginning)
        {
            if (_gameSetUp!.SaveRoot!.PlayerList == null || _gameSetUp.SaveRoot.PlayerList.Count == 0)
            { //hopefully i won't regret this decision that if the player count is 0, still do this part.
                await _gameSetUp.SetUpGameAsync(isBeginning);
                return; //because there was no players.  games like three letter fun has that possibility.
            }
            if (_gameSetUp.SaveRoot.PlayerList.Count() == 0 && _gameInfo.SinglePlayerChoice != EnumPlayerChoices.Solitaire)
                throw new BasicBlankException("There was no players.  Rethink");
            if (isBeginning == false)
            {
                _gameSetUp.SaveRoot.PlayerList.ForEach(items => items.InGame = items.CanStartInGame);
                _gameSetUp.SaveRoot.PlayOrder.WhoTurn = _gameSetUp.SaveRoot.PlayOrder.WhoStarts; //this was wrong too.
                _gameSetUp.SaveRoot.PlayOrder.WhoTurn = await _gameSetUp.SaveRoot.PlayerList.CalculateWhoTurnAsync();
            }
            _gameSetUp.SaveRoot.PlayOrder.WhoStarts = _gameSetUp.SaveRoot.PlayOrder.WhoTurn;
            await _gameSetUp.SetUpGameAsync(isBeginning);
        }

    }
}