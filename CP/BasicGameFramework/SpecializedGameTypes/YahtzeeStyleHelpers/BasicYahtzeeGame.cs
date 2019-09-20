using BasicGameFramework.BasicEventModels;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    /// <summary>
    /// this is everything for a basic yahtzee game.
    /// however, can have things to override just in case its needed.
    /// i'll have to see if i need a player here or not.
    /// </summary>
    public class BasicYahtzeeGame<D> : DiceGameClass<D, YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
        , IMoveNM
        where D : SimpleDice, new()
    {
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Points).Take(1).Single();
            SaveRoot!.Round = 0;
            SaveRoot.RollNumber = 0; //i think
            await ShowWinAsync();
        }
        protected override bool ShowDiceUponAutoSave => false; //try this way.
        public override async Task EndTurnAsync()
        {
            SingleInfo!.Scoresheet!.Visible = false;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //could be.
            _thisMod.ThisCup!.CanShowDice = false;
        }
        private async Task StartNewRoundAsync()
        {
            if (WhoTurn == SaveRoot!.Begins)
            {
                if (SingleInfo!.Scoresheet!.IsGameOver == true)
                {
                    await GameOverAsync();
                    return;
                }
                SaveRoot.Round++;
            }
            await StartNewTurnAsync();
        }
        private void PrepTurn()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.Scoresheet!.Visible = true;
            SingleInfo.Scoresheet.StartTurn();
            ProtectedStartTurn();
        }
        public async Task MakeMoveAsync(RowInfo thisRow)
        {
            SingleInfo!.Scoresheet!.MarkScore(thisRow);
            SingleInfo.Points = SingleInfo.Scoresheet.TotalScore;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            _thisMod.ThisCup!.UnholdDice();
            _thisMod.ThisCup.HideDice();
            if (PlayerList.Any(Items => Items.MissNextTurn == true))
                await _thisMod.ShowGameMessageAsync($"Everyone gets their turns skipped except for {SingleInfo.NickName}.  Also, everyone will get a 0 for the category closest to the top because {SingleInfo.NickName} got a Kismet even though it was already marked"); //kismet is the only game that does this
            await EndTurnAsync();
            await StartNewRoundAsync();
        }
        private void LoadGame()
        {
            LoadUpDice();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Scoresheet = MainContainer.Resolve<ScoresheetVM<D>>(); //needs to be instance, not singleton.
                thisPlayer.Scoresheet.LoadBoard();
                thisPlayer.Scoresheet.Visible = false;
                LoadEnables(thisPlayer);
            });
            SaveRoot!.LoadMod(_thisMod);
        }
        private void LoadEnables(YahtzeePlayerItem<D> thisPlayer)
        {
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                thisPlayer.Scoresheet!.SendEnableProcesses(_thisMod, () => true);
        }
        public override Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.Scoresheet!.RowList.Count == 0)
                    throw new BasicBlankException("RowList Not Populated.  Rethink");
                thisPlayer.RowList = thisPlayer.Scoresheet.RowList;
            });
            return Task.CompletedTask;
        }
        private readonly YahtzeeViewModel<D> _thisMod;
        public BasicYahtzeeGame(IGamePackageResolver container) : base(container)
        {
            _thisMod = MainContainer.Resolve<YahtzeeViewModel<D>>();
        }
        public override Task FinishGetSavedAsync()
        {
            if (IsLoaded == false)
                LoadGame();
            AfterRestoreDice(); //i think
            PlayerList!.ForEach(items =>
            {
                items.Scoresheet!.RowList = items.RowList;
            });
            if (SaveRoot!.RollNumber > 1)
            {
                _thisMod.ThisCup!.CanShowDice = true;
                SingleInfo = PlayerList.GetWhoPlayer();
                SingleInfo.Scoresheet!.PopulatePossibleScores();
            }
            else
                _thisMod.ThisCup!.HideDice();
            SaveRoot.LoadMod(_thisMod);
            SingleInfo = PlayerList.GetWhoPlayer();
            SingleInfo.Scoresheet!.Visible = true; //try this.
            return Task.CompletedTask;
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (IsLoaded == false)
                LoadGame();
            SetUpDice(); //i think
            PlayerList!.ForEach(items =>
            {
                items.Points = 0;
                items.Scoresheet!.ClearBoard();
            });
            SaveRoot!.Round = 1;
            SaveRoot.Begins = WhoStarts;
            PrepTurn();
            if (isBeginning == false)
                await ThisE.SendUpdateAsync();//try this.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            SingleInfo!.Scoresheet!.PopulatePossibleScores();
            SingleInfo.Scoresheet.Visible = true;
            await ContinueTurnAsync();
        }
        async Task IMoveNM.MoveReceivedAsync(string data)
        {
            int id = int.Parse(data);
            RowInfo ThisRow = SingleInfo!.Scoresheet!.GetRowSent(id);
            await MakeMoveAsync(ThisRow);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn();
            PrepTurn();
            if (SingleInfo!.Scoresheet!.IsGameOver == true)
            {
                await EndTurnAsync();
                await StartNewRoundAsync();
                return;
            }
            this.ShowTurn();
            await ContinueTurnAsync();
        }
    }
}