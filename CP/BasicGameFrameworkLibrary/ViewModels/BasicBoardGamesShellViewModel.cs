﻿using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.ViewModels
{
    public abstract class BasicBoardGamesShellViewModel<P> : BasicMultiplayerShellViewModel<P>, IBasicBoardGamesShellViewModel where P : class, IPlayerColors, new()
    {
        public BasicBoardGamesShellViewModel(
            IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test) : base(mainContainer,
                container,
                gameData,
                basicData,
                save,
                test)
        {
            MiscDelegates.ColorsFinishedAsync = CloseColorsAsync; //hopefully this simple this time.
        }
        protected override Task PrepNewGameAsync()
        {
            IEraseColors erase = MainContainer.Resolve<IEraseColors>();
            erase.EraseColors();
            return Task.CompletedTask;
        }
        public IBeginningColorViewModel? ColorScreen { get; set; }
        //still needs to be able to override it.
        //so for game of life, has the option for another screen.
        protected override async Task GetStartingScreenAsync()
        {
            if (ColorScreen != null)
            {
                await CloseSpecificChildAsync(ColorScreen);
            }
            
            ColorScreen = MainContainer.Resolve<IBeginningColorViewModel>();
            await LoadScreenAsync(ColorScreen);

        }
        protected virtual bool CanOpenMainAfterColors => true; //so overrided versions can do other things.
        private async Task CloseColorsAsync()
        {
            if (ColorScreen == null)
            {
                throw new BasicBlankException("The color screen was not even active.  Rethink");
            }
            await CloseSpecificChildAsync(ColorScreen);
            ColorScreen = null;
            if (CanOpenMainAfterColors)
            {
                await StartNewGameAsync();
            }
            //if something else is needed, then whoever handles this has to decide what to do and anything that inherits from this can handle it.
            IAfterColorProcesses processes = MainContainer.Resolve<IAfterColorProcesses>();
            await processes.AfterChoosingColorsAsync(); //hopefully this simple.
        }
    }
}