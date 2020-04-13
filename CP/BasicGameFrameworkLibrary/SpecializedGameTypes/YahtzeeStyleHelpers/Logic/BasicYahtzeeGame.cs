using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.Attributes;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic
{
    public class BasicYahtzeeGame<D> : DiceGameClass<D, YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
    where D : SimpleDice, new()
    {
        private readonly YahtzeeGameContainer<D> _gameContainer;
        private readonly IScoreLogic _scoreLogic;
        private readonly ScoreContainer _scoreContainer;
        private readonly IYahtzeeEndRoundLogic _endRoundLogic;
        private readonly YahtzeeVMData<D> _model;
        public BasicYahtzeeGame(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            YahtzeeVMData<D> currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            YahtzeeGameContainer<D> gameContainer,
            IScoreLogic scoreLogic,
            ScoreContainer scoreContainer,
            IYahtzeeEndRoundLogic endRoundLogic,
            StandardRollProcesses<D, YahtzeePlayerItem<D>> roller) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _gameContainer = gameContainer;
            _scoreLogic = scoreLogic;
            _scoreContainer = scoreContainer;
            _endRoundLogic = endRoundLogic;
            _model = currentMod;
            _scoreContainer.StartTurn = (() => SingleInfo!.MissNextTurn = false);
        }

        protected override bool ShowDiceUponAutoSave => false; //try this way.
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //could be.
            _model.Cup!.CanShowDice = false;
        }
        private async Task PrepTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            _scoreContainer.RowList = SingleInfo.RowList;
            if (_gameContainer.GetNewScoreAsync != null)
            {
                await _gameContainer.GetNewScoreAsync.Invoke(); //if nobody is handling it, then skip it.
            }
            _scoreLogic.StartTurn(); //hopefully this simple.
            ProtectedStartTurn();
        }
        //maybe no need anymore for the populatesaveroot because i am using same address.
        private void LoadGame()
        {
            LoadMod();
            PlayerList.ForEach(player =>
            {
                if (player.RowList.Count == 0)
                {
                    _scoreContainer.RowList = player.RowList.ToCustomBasicList(); //try this way.  hopefully will allow cloning back later.
                    _scoreLogic.LoadBoard();
                    player.RowList = _scoreContainer.RowList.ToCustomBasicList();
                }
            });
            SaveRoot.LoadMod(_model);
        }
        
        public override async Task FinishGetSavedAsync()
        {
            LoadGame();
            AfterRestoreDice();
            SingleInfo = PlayerList.GetWhoPlayer();
            _scoreContainer.RowList = SingleInfo.RowList;
            if (SaveRoot!.RollNumber > 1)
            {
                _model.Cup!.CanShowDice = true;
                _scoreLogic.PopulatePossibleScores();
            }
            else
            {
                _model.Cup!.HideDice();
            }
            SaveRoot.LoadMod(_model);
            SingleInfo = PlayerList.GetWhoPlayer();
            if (_gameContainer.GetNewScoreAsync != null)
            {
                await _gameContainer.GetNewScoreAsync.Invoke(); //if nobody is handling it, then skip it.
            }
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            PlayerList!.ForEach(items =>
            {
                items.Points = 0;
                items.RowList.Clear();
            });

            LoadGame();
            SetUpDice();
            SaveRoot!.Round = 1;
            SaveRoot.Begins = WhoStarts;
            await PrepTurnAsync();
            await FinishUpAsync!(isBeginning);
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            _scoreLogic.PopulatePossibleScores(); //hopefully this simple now.
            await ContinueTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn();
            await PrepTurnAsync();
            if (_endRoundLogic.IsGameOver)
            {
                await EndTurnAsync();
                await _endRoundLogic.StartNewRoundAsync();
                return;
            }
            
            this.ShowTurn();
            await ContinueTurnAsync();
        }
    }
}
