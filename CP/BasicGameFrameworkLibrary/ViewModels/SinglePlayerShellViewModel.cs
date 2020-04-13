using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.Conductors;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.ViewModels
{
    public abstract class SinglePlayerShellViewModel : ConductorCollectionAllActive<object> , 
        IHandleAsync<NewGameEventModel>,
        IHandleAsync<GameOverEventModel>,
        IMainGPXShellVM
    {
        private readonly ISaveSinglePlayerClass _saves;

        //i may decide to make this abstract.
        //so when its a new game, then the overrided version has to handle deciding on the main view model.

        //this is single player game so its separate from multiplayer anyways.

        //once i get the simple parts to work, then can expand.
        //i don't think single player even have rounds.


        public SinglePlayerShellViewModel(IGamePackageResolver mainContainer, 
            CommandContainer container,
            IGameInfo gameData,
            ISaveSinglePlayerClass saves
            )
        {
            MainContainer = mainContainer; //the subscribe part is already done for me too.
            CommandContainer = container;
            GameData = gameData;
            _saves = saves;
            DisplayName = gameData.GameName;
            //for other shells, since we know whether its new game or new round, it can load the proper ui.
            //may need another interface that knows whether its game over or not for cases where its rounds.

        }

        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            //go ahead and load the body anyways.  later can rethink.
            //MainVM = GetMainViewModel();
            //await LoadScreenAsync(MainVM);

            //for a game like bunco, can't show new game to start because its not alwaysnewgame.
            //if wrong, make notes and fix.

            if (AlwaysNewGame)
            {
                await ShowNewGameAsync();
            }
            if (AutoStartNewGame)
            {
                await StartNewGameAsync();
            }
            else
            {
                await OpenStartingScreensAsync();
            }
        }
        /// <summary>
        /// this should be responsible for any opening screens that is not new game.  new game is automatic.
        /// if we find a case where it can't be automatic, rethink.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OpenStartingScreensAsync() => Task.CompletedTask;
        private async Task StartNewGameAsync()
        {
            CommandContainer.ClearLists(); //try this too.  hopefully this simple.
            ClearSubscriptions();
            MainVM = GetMainViewModel();
            await LoadScreenAsync(MainVM);
            FinishInit();
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
        protected void FinishInit()
        {
            CommandContainer.Processing = false;
            CommandContainer.IsExecuting = false;
        }


        //private static async Task<bool> TryActivateItemAsync(object parentViewModel, IUIView? parentViewScreen, object childViewModel)
        //{
        //    if (UIPlatform.ViewLocator is null)
        //        return false;
        //    if (UIPlatform.ScreenLoader is null)
        //        return false;


        //    IUIView childview = await UIPlatform.ViewLocator.LocateViewAsync(childViewModel)!;
        //    if (childview == null)
        //        throw new BasicBlankException("No view was found when trying to active an item.  Rethink");
        //    if (parentViewScreen == null)
        //        throw new BasicBlankException("Has to have an active view in order to activate another screen");
        //    //we need a method that will do the rest of what is needed.  probably another interface.
        //    await UIPlatform.ScreenLoader.LoadScreenAsync(parentViewModel, parentViewScreen, childViewModel, childview);
        //    if (childViewModel is IScreen screens)
        //    {
        //        if (parentViewModel is IHaveActiveViewModel active)
        //            active.ActiveViewModel = screens;
        //        await screens.ActivateAsync(childview);
        //    }
        //    else
        //    {
        //        await childview.TryActivateAsync(); //to do extra things whatever is needed that is actually async.
        //        //should not activate twice.
        //    }

        //    return true;
        //}


        //private async Task ExperimentAsync(object vm)
        //{
        //    //this time, has to do lots of copy/paste to see what the problem is this time.
        //    //return ConductorBehavior.TryActivateItemAsync(this, MainScreen, viewModel!);
        //    await TryActivateItemAsync(this, MainScreen, vm);
        //}

        protected async Task ShowNewGameAsync()
        {
            NewGameVM = MainContainer.Resolve<INewGameVM>();

            //await ExperimentAsync(NewGameVM);

            await LoadScreenAsync(NewGameVM);
        }
        /// <summary>
        /// this is needed because it may need to resolve other things to load other things but not at the beginning.
        /// </summary>
        public IGamePackageResolver MainContainer { get; }
        protected CommandContainer CommandContainer { get; }
        protected IGameInfo GameData { get; }
        public INewGameVM? NewGameVM { get; set; } //this is one for the ui to know
        //this is the view model that represents the majority of it.
        //can even be a gameboard or gameboard plus extras (?)
        public IMainScreen? MainVM { get; set; } //this is another one for the ui to know.

        /// <summary>
        /// this is the view model that represents the body.  its used when you decide on new game.
        /// </summary>
        /// <returns></returns>
        protected abstract IMainScreen GetMainViewModel();

        
        protected abstract bool AlwaysNewGame { get; }
        /// <summary>
        /// usually can automatically start a new game upon loading.
        /// however some games requires settings to be chosen first.
        /// 
        /// </summary>
        protected virtual bool AutoStartNewGame => true;

        async Task IHandleAsync<NewGameEventModel>.HandleAsync(NewGameEventModel message)
        {
            if (NewGameVM == null)
            {
                throw new BasicBlankException("New game was not even active.  Therefore, I should not have received message for requesting new game");
            }
            if (AlwaysNewGame == false)
            {
                await CloseSpecificChildAsync(NewGameVM);
                NewGameVM = null;//forgot to set to null.
            }
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
            }
            MainVM = null; //looks like i have to set to null manually.
            //i think i should load it again anyways (?) and set the old to nothing.  so a fresh one will be created.
            //maybe we don't need the update code but we still need the load processes.
            //since we have code for closing, hopefully i am not forced to figure out how to delete controls (?)
            await _saves.DeleteSinglePlayerGameAsync(); //i think.
            await NewGameRequestedAsync();
            await StartNewGameAsync();
        }
        /// <summary>
        /// this is used in cases where somebody actually clicks new game but sometimes extra screens has to be closed out.
        /// i could decide later to have a list it keeps track of to close out.
        /// </summary>
        /// <returns></returns>
        protected virtual Task NewGameRequestedAsync() => Task.CompletedTask;
        /// <summary>
        /// this is used in cases like mastermind where you need to show a solution when its game over.
        /// </summary>
        /// <returns></returns>
        protected virtual Task GameOverScreenAsync() => Task.CompletedTask;

        async Task IHandleAsync<GameOverEventModel>.HandleAsync(GameOverEventModel message)
        {
            CommandContainer.ClearLists(); //try this too.
            if (MainVM == null)
            {
                throw new BasicBlankException("The main view model was not even available.  Rethink");
            }
            await _saves.DeleteSinglePlayerGameAsync(); //just in case its forgotten, it will be deleted.
            await CloseSpecificChildAsync(MainVM);
            MainVM = null;
            if (NewGameVM != null)
            {
                return;
            }
            await GameOverScreenAsync();
            if (AutoStartNewGame == false)
            {
                await OpenStartingScreensAsync();
            }
            else
            {
                await ShowNewGameAsync(); //try this way (?)
            }
        }
    }
}