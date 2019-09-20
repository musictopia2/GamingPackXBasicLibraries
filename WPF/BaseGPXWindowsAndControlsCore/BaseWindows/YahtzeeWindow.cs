using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.YahtzeeControls;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.YahtzeeStyleHelpers;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace BaseGPXWindowsAndControlsCore.BaseWindows
{
    public abstract class YahtzeeWindow<D> : MultiPlayerWindow<YahtzeeViewModel<D>, YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
        , IHandleAsync<WarningEventModel>
        where D : SimpleDice, new()
    {
        public override Task HandleAsync(LoadEventModel message)
        {
            YahtzeeSaveInfo<D> saveRoot = OurContainer!.Resolve<YahtzeeSaveInfo<D>>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            foreach (var thisPlayer in saveRoot.PlayerList)
            {
                ScoresheetWPF<D> thisScore = new ScoresheetWPF<D>();
                thisScore.Init(thisPlayer.Scoresheet!); // i think
                _sheetGrid!.Children.Add(thisScore);
            }
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            YahtzeeSaveInfo<D> saveRoot = OurContainer!.Resolve<YahtzeeSaveInfo<D>>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            _sheetGrid!.Children.Clear(); //there is no other way around it this time.
            foreach (var thisPlayer in saveRoot.PlayerList)
            {
                ScoresheetWPF<D> thisScore = new ScoresheetWPF<D>();
                thisScore.Init(thisPlayer.Scoresheet!); // i think
                _sheetGrid.Children.Add(thisScore);
            }
            return Task.CompletedTask;
        }
        public async Task HandleAsync(WarningEventModel message)
        {
            var exps = MessageBox.Show(this, message.Message, "", MessageBoxButton.YesNo);
            SelectionChosenEventModel results = new SelectionChosenEventModel();
            if (exps == MessageBoxResult.No)
                results.OptionChosen = EnumOptionChosen.No;
            else
                results.OptionChosen = EnumOptionChosen.Yes;
            await _thisE!.PublishAsync(results);
        }
        protected override void AfterCreateViewModel() { }
        private ScoreBoardWPF? _thisScore;
        private DiceListControlWPF<D>? _diceControl;
        private Grid? _sheetGrid;
        protected override async void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            Grid eGrid = new Grid();
            GridHelper.AddAutoColumns(eGrid, 2);
            GridHelper.AddAutoRows(eGrid, 2);
            _sheetGrid = new Grid();
            GridHelper.AddControlToGrid(eGrid, _sheetGrid, 0, 0);
            GridHelper.AddControlToGrid(eGrid, thisStack, 0, 1);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(YahtzeeViewModel<D>.NormalTurn));
            firstInfo.AddRow("Roll", nameof(YahtzeeViewModel<D>.RollNumber)); // its bound now.
            firstInfo.AddRow("Status", nameof(YahtzeeViewModel<D>.Status));
            firstInfo.AddRow("Turn #", nameof(YahtzeeViewModel<D>.Round)); // i think
            thisStack.Children.Add(firstInfo.GetContent);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Points", false, nameof(YahtzeePlayerItem<D>.Points));
            thisStack.Children.Add(_thisScore);
            var thisRoll = GetGamingButton("Roll Dice", nameof(YahtzeeViewModel<D>.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<D>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            GridHelper.AddControlToGrid(eGrid, otherStack, 1, 0);
            Grid.SetColumnSpan(otherStack, 2);
            AddRestoreCommand(thisStack);
            MainGrid!.Children.Add(eGrid); // i think will be this instead.
            await FinishUpAsync();
        }
        private EventAggregator? _thisE;
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>>();
            RegisterNonSavedClasses();
            OurContainer.RegisterType<StandardRollProcesses<D, YahtzeePlayerItem<D>>>();
            RegisterDiceProportions();
            OurContainer.RegisterSingleton<IGenerateDice<int>, D>();
            OurContainer.RegisterType<ScoresheetVM<D>>(false); //this time, cant' be singleton since each player will have their own.
            OurContainer.RegisterType<BasicYahtzeeGame<D>>();
            OurContainer.RegisterType<YahtzeeSaveInfo<D>>();
            _thisE = OurContainer.Resolve<EventAggregator>();
        }
        protected abstract void RegisterNonSavedClasses();
        protected abstract void RegisterDiceProportions();
    }
}