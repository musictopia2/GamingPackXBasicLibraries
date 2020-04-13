using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.EventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.Conductors;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.ViewModels
{
    public abstract class BasicMultiplayerShellViewModel<P> : ConductorCollectionAllActive<object>,
        IHandleAsync<NewGameEventModel>,
        IHandleAsync<GameOverEventModel>,
        IHandleAsync<NewRoundEventModel>,
        IHandleAsync<WaitForHostEventModel>,
        IHandleAsync<StartAutoresumeMultiplayerGameEventModel>,
        IHandleAsync<StartMultiplayerGameEventModel<P>>,
        IHandleAsync<RoundOverEventModel>,
        INewGameNM,
        ILoadGameNM,
        IMainGPXShellVM,
        IHandleAsync<LoadMainScreenEventModel>,
        IRestoreNM,
        IHandleAsync<RestoreEventModel>, 
        IBasicMultiplayerShellViewModel
        where P : class, IPlayerItem, new()
    {


        public BasicMultiplayerShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test
            )
        {
            MainContainer = mainContainer; //the subscribe part is already done for me too.
            CommandContainer = container;
            GameData = gameData;
            BasicData = basicData;
            _save = save;
            _test = test;
            DisplayName = gameData.GameName;
        }

        public string NickName { get; set; } = ""; //if you need nick name shown for test purposes that is an option.
        public IGamePackageResolver MainContainer { get; }
        protected CommandContainer CommandContainer { get; }
        protected IGameInfo GameData { get; }
        protected BasicData BasicData { get; }
        protected override async Task ActivateAsync()
        {
            GlobalHelpers.LoadGameScreenAsync = LoadGameScreenAsync;
            if (BasicData.GamePackageMode == EnumGamePackageMode.None)
            {
                throw new BasicBlankException("You need to specify whether its debug or reals");
            }
            if (BasicData.GamePackageMode == EnumGamePackageMode.Production)
            {
                if (_test.AllowAnyMove)
                {
                    throw new BasicBlankException("Can't allow any move because its production");
                }
                if (_test.AutoNearEndOfDeckBeginning)
                {
                    throw new BasicBlankException("Can't be near the end of deck at beginning because its production");
                }
                if (_test.CardsToPass != 0)
                {
                    throw new BasicBlankException("Cannot pass a special number of cards becuase its production");
                }

                if (_test.ComputerEndsTurn)
                {
                    throw new BasicBlankException("The computer cannot just end turn because its production.  Try setting another property");
                }
                if (_test.ComputerNoCards)
                {
                    throw new BasicBlankException("The computer has to have cards because its production");
                }
                if (_test.DoubleCheck)
                {
                    throw new BasicBlankException("No double checking anything because its production");
                }
                if (_test.ImmediatelyEndGame)
                {
                    throw new BasicBlankException("Cannot immediately end game because its production");

                }

                if (_test.NoAnimations)
                {
                    throw new BasicBlankException("Animations are required in production.");
                }
                if (_test.NoCommonMessages)
                {
                    throw new BasicBlankException("Must have common messages because its production");
                }
                if (_test.PlayCategory != EnumPlayCategory.Normal)
                {
                    throw new BasicBlankException("The play category must be none because its production");
                }
                if (_test.SaveOption != EnumTestSaveCategory.Normal)
                {
                    throw new BasicBlankException("The save mode must be normal because its production");
                }
                if (_test.StatePosition != 0)
                {
                    throw new BasicBlankException("Must show most recent state because its in production");
                }
                if (_test.ShowErrorMessageBoxes == false)
                {
                    throw new BasicBlankException("Must show error message boxes because its in production");
                }
                if (_test.WhoStarts != 1)
                {
                    throw new BasicBlankException("WhoStarts must start with 1 because its in production");
                }
                if (_test.ShowNickNameOnShell)
                {
                    throw new BasicBlankException("Cannot show nick name on shell because its in production");
                }
                if (_test.AlwaysNewGame)
                {
                    throw new BasicBlankException("Can't always show new game because its in production");
                }
                if (_test.EndRoundEarly)
                {
                    throw new BasicBlankException("Can't end round early because its in production");
                }
            }
            await BeforeLoadingOpeningScreenAsync();
            NickName = BasicData.NickName; //i think.
            //needs opening no matter what.
            if (CanStartWithOpenScreen)
            {
                OpeningScreen = MainContainer.Resolve<IMultiplayerOpeningViewModel>();
                await LoadScreenAsync(OpeningScreen); //so for testing purposes like three letter fun or other situations, i can more easily test.
            }
        }

        protected virtual bool CanStartWithOpenScreen => true;

        protected virtual Task BeforeLoadingOpeningScreenAsync() => Task.CompletedTask;

        public IMultiplayerOpeningViewModel? OpeningScreen { get; set; } //has to be property so it hooks up properly.

        public INewRoundVM? NewRoundScreen { get; set; }

        public INewGameVM? NewGameScreen { get; set; }
        private readonly IMultiplayerSaveState _save;
        private readonly TestOptions _test;
        private IBasicGameProcesses<P>? _mainGame;

        public IMainScreen? MainVM { get; set; } //this is another one for the ui to know.
        //IMultiplayerOpeningViewModel? IBasicMultiplayerShellViewModel.OpeningScreen
        //{
        //    get => OpeningScreen;
        //}


        /// <summary>
        /// this is used in cases where somebody actually clicks new game but sometimes extra screens has to be closed out.
        /// i could decide later to have a list it keeps track of to close out.
        /// also used if its a new round.
        /// may require rethinking.
        /// </summary>
        /// <returns></returns>
        protected virtual Task NewGameOrRoundRequestedAsync() => Task.CompletedTask;

        protected virtual Task PrepNewGameAsync() => Task.CompletedTask;
        

        /// <summary>
        /// this is when somebody chooses new game.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
        {
            await PrepNewGameAsync();
            if (_test.AlwaysNewGame)
            {
                CommandContainer.ClearLists(); //try this too.
                await _save.DeleteGameAsync(); //i think.
                ReplaceGame(); //try this too.
                //other things are needed here too.
            }

            if (NewGameScreen == null)
            {
                throw new BasicBlankException("New game was not even active.  Therefore, I should not have received message for requesting new game");
            }

            await CloseMainAsync("Should have shown main game when showing new game.");
            //i think i should load it again anyways (?) and set the old to nothing.  so a fresh one will be created.
            //maybe we don't need the update code but we still need the load processes.
            //since we have code for closing, hopefully i am not forced to figure out how to delete controls (?)
            await _save.DeleteGameAsync(); //i think.
            await CloseSpecificChildAsync(NewGameScreen);
            NewGameScreen = null;//forgot to set to null.
            await NewGameOrRoundRequestedAsync();
            if (_mainGame == null)
            {
                throw new BasicBlankException("Failed to replace game when requesting new game.  Rethink");
            }
            //can't load here because it thinks you have the old file (wrong).

            //await LoadGameScreenAsync(); //i think in this case, should load here.

            IRequestNewGameRound gameRound = MainContainer.Resolve<IRequestNewGameRound>();
            await gameRound.RequestNewGameAsync();

        }

        protected async Task CloseMainAsync(string message)
        {
            if (MainVM == null)
            {
                throw new BasicBlankException(message);
            }
            await CloseSpecificChildAsync(MainVM!);
            MainVM = null; //looks like i have to set to null manually.

        }
        private async Task LoadGameScreenAsync()
        {
            if (_mainGame == null)
            {
                _mainGame = MainContainer.Resolve<IBasicGameProcesses<P>>();
            }
            if (_mainGame.CanMakeMainOptionsVisibleAtBeginning)
            {
                ClearSubscriptions(); //try this too.
                await StartNewGameAsync();
                if (_test.AlwaysNewGame)
                {
                    NewGameScreen = MainContainer.Resolve<INewGameVM>();
                    await LoadScreenAsync(NewGameScreen);
                }
                return;
            }


            //only host or single player game gets to this part.
            await GetStartingScreenAsync();
            if (_test.AlwaysNewGame)
            {
                NewGameScreen = MainContainer.Resolve<INewGameVM>();
                await LoadScreenAsync(NewGameScreen);
            }
        }

        /// <summary>
        /// this is used for cases like when you have to choose colors.  good example are the board games.
        /// </summary>
        /// <returns></returns>
        protected async virtual Task GetStartingScreenAsync() => await Task.CompletedTask;


        /// <summary>
        /// this is the view model that represents the body.  its used when you decide on new game.
        /// </summary>
        /// <returns></returns>
        protected abstract IMainScreen GetMainViewModel();
        
        //for new version, maybe should be new method.  because for cases like uno, it may have to close out to load something else and later reload again.
        protected virtual async Task StartNewGameAsync()
        {
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
                MainVM = null;
            }
            MainVM = GetMainViewModel();
            await LoadScreenAsync(MainVM);
        }
        protected async Task ShowNewGameAsync()
        {
            NewGameScreen = MainContainer.Resolve<INewGameVM>();
            await LoadScreenAsync(NewGameScreen);
            CommandContainer.IsExecuting = true;
            CommandContainer.ManuelFinish = true;
        }

        /// <summary>
        /// this is when the game is over.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message) //done.
        {
            //i propose just having the extra button for new game that appears when the game is over.
            CommandContainer.ClearLists(); //try this too.
            ReplaceGame(); //i think here it should replace the game. not so for rounds.
            if (BasicData.MultiPlayer == true && BasicData.Client == true)
            {
                return; //because only host can choose new game unless its single player game.
            }
            await _save.DeleteGameAsync();
            
            if (MainVM == null)
            {
                throw new BasicBlankException("The main view model was not even available.  Rethink");
            }

            //get it fresh now.
            //this is one case where i have to replace the game.
            //also think about what else has to be replaced.

            //if there are other things that needs to be replaced that does not depend on the game processes, do here.

            //host has to decide if he even wants new game.
            if (_test.AlwaysNewGame)
            {
                //has to close so it can refresh agin.
                await CloseSpecificChildAsync(NewGameScreen!);
                NewGameScreen = null; //has to do again.  that is the best way to handle so i can click new game.
            }
            await ShowNewGameAsync();
        }
        protected void ClearSubscriptions()
        {
            Aggregator.ClearSubscriptions(this);
            if (GlobalDelegates.RefreshSubscriptions == null)
            {
                throw new BasicBlankException("Nobody is handling the refresh the subscriptions.  Rethink");
            }
            GlobalDelegates.RefreshSubscriptions.Invoke(Aggregator);
        }
        protected virtual void ReplaceGame()
        {
            _ = MainContainer.ReplaceObject<IViewModelData>(); //this has to be replaced before the game obviously.
            Assembly assembly = Assembly.GetAssembly(GetType())!;
            CustomBasicList<Type> thisList = assembly.GetTypes().Where(items => items.HasAttribute<AutoResetAttribute>()).ToCustomBasicList();
            thisList.AddRange(GetAdditionalObjectsToReset());
            ClearSubscriptions();
            
            Type? type = MainContainer.LookUpType<IStandardRollProcesses>();
            if (type != null)
            {
                thisList.Add(type); //hopefully this simple.  this allows more parts to be able to be added without a new class
                //useful for dice games.
            }
            //attempt to also unsubscribe to all as well.

            //could do delegates.  however, if not set, then won't do.
            if (MiscDelegates.GetMiscObjectsToReplace != null)
            {
                thisList.AddRange(MiscDelegates.GetMiscObjectsToReplace.Invoke());
            }
            MainContainer.ResetSeveralObjects(thisList);
            _mainGame = MainContainer.ReplaceObject<IBasicGameProcesses<P>>(); //hopefully this works
        }

        protected virtual CustomBasicList<Type> GetAdditionalObjectsToReset() => new CustomBasicList<Type>();


        private async Task CloseOpenScreenAsync(string message)
        {
            if (OpeningScreen == null)
            {
                throw new BasicBlankException(message);
            }
            await CloseSpecificChildAsync(OpeningScreen);
            OpeningScreen = null;
        }

        async Task IHandleAsync<WaitForHostEventModel>.HandleAsync(WaitForHostEventModel message)
        {
            await CloseOpenScreenAsync("Opening screen was nothing.  Therefore, waiting for host can't close it.  Rethink");
        }

        async Task IHandleAsync<StartAutoresumeMultiplayerGameEventModel>.HandleAsync(StartAutoresumeMultiplayerGameEventModel message)
        {
            await CloseOpenScreenAsync("Start autoresume multiplayer game");
            //await LoadGameScreenAsync(); //forgot here too.
            IStartMultiPlayerGame<P> starts = MainContainer.Resolve<IStartMultiPlayerGame<P>>();
            await starts.LoadSavedGameAsync();
        }

        async Task IHandleAsync<StartMultiplayerGameEventModel<P>>.HandleAsync(StartMultiplayerGameEventModel<P> message)
        {
            await CloseOpenScreenAsync("Start new multiplayer game");
            //await LoadGameScreenAsync(); //forgot here too.
            IStartMultiPlayerGame<P> starts = MainContainer.Resolve<IStartMultiPlayerGame<P>>();
            await starts.LoadNewGameAsync(message.PlayerList);
        }

        async Task IHandleAsync<RoundOverEventModel>.HandleAsync(RoundOverEventModel message)
        {

            NewRoundScreen = MainContainer.Resolve<INewRoundVM>();
            await LoadScreenAsync(NewRoundScreen);
            CommandContainer.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            //i think this may be it (?)
            //CommandContainer.ManuelFinish = false;
            //CommandContainer.IsExecuting = false;
        }

        private async Task CloseRoundAsync()
        {
            if (NewRoundScreen == null)
            {
                throw new BasicBlankException("Never had the round screen opened.  Rethink");
            }
            await CloseSpecificChildAsync(NewRoundScreen);
            NewRoundScreen = null;
        }

        async Task IHandleAsync<NewRoundEventModel>.HandleAsync(NewRoundEventModel message)
        {
            ClearSubscriptions();
            await CloseMainAsync("The main screen should have been not null when choosing new round.  Rethink");
            await CloseRoundAsync();
            await NewGameOrRoundRequestedAsync();
            //await LoadGameScreenAsync();
            IRequestNewGameRound gameRound = MainContainer.Resolve<IRequestNewGameRound>();
            await gameRound.RequestNewRoundAsync();
            //hint:  either another message or somebody else would send the same message for the client.
        }

        async Task INewGameNM.NewGameReceivedAsync(string data)
        {
            if (OpeningScreen != null)
            {
                throw new BasicBlankException("The opening screen should have been closed.  Rethink");
            }
            await RestoreAsync(); //take a risk here.
            IClientUpdateGame clientUpdate = MainContainer.Resolve<IClientUpdateGame>();
            await clientUpdate.UpdateGameAsync(data);
        }

        async Task ILoadGameNM.LoadGameAsync(string data) //first time only.
        {

            if (OpeningScreen != null)
            {
                throw new BasicBlankException("The opening screen should have been closed.  Rethink");
            }
            if (MainVM != null)
            {
                throw new BasicBlankException("The main screen was already opened.  Rethink");
            }
            //await LoadGameScreenAsync();
            ILoadClientGame client = MainContainer.Resolve<ILoadClientGame>();
            await client.LoadGameAsync(data);
        }

        async Task IHandleAsync<LoadMainScreenEventModel>.HandleAsync(LoadMainScreenEventModel message)
        {
            await StartNewGameAsync(); //i think this simple now.
        }

        async Task IRestoreNM.RestoreMessageAsync(string payLoad)
        {
            await RestoreAsync();
            IClientUpdateGame client = MainContainer.Resolve<IClientUpdateGame>();
            await client.UpdateGameAsync(payLoad);
        }
        //protected virtual bool NeedsReplaceUponRestore => true;
        private async Task RestoreAsync()
        {
            if (NewRoundScreen != null)
            {
                await CloseSpecificChildAsync(NewRoundScreen);
                NewRoundScreen = null;
            }
            else if (NewGameScreen != null)
            {
                await CloseSpecificChildAsync(NewGameScreen);
                NewGameScreen = null;
            }
            ReplaceGame();
            //if (NeedsReplaceUponRestore)
            //{
                
            //}
            await CloseMainAsync("Failed to restore because never had game");

            //await LoadGameScreenAsync();
        }

        async Task IHandleAsync<RestoreEventModel>.HandleAsync(RestoreEventModel message)
        {
            await RestoreAsync();
            IRestoreMultiPlayerGame restore = MainContainer.Resolve<IRestoreMultiPlayerGame>();
            await restore.RestoreGameAsync();
        }
    }
}