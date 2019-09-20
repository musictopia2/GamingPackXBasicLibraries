using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.YahtzeeControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace BaseGPXPagesAndControlsXF.BasePageProcesses.Pages
{
    public abstract class YahtzeePage<D> : MultiPlayerPage<YahtzeeViewModel<D>, YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>, IHandleAsync<WarningEventModel>
        where D : SimpleDice, new()
    {
        private ScoreBoardXF? _thisScore;
        private DiceListControlXF<D>? _diceControl;
        private Grid? _sheetGrid;
        private EventAggregator? _thisE;
        public YahtzeePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            YahtzeeSaveInfo<D> saveRoot = OurContainer!.Resolve<YahtzeeSaveInfo<D>>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            foreach (var thisPlayer in saveRoot.PlayerList)
            {
                ScoresheetXF<D> thisScore = new ScoresheetXF<D>();
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
                ScoresheetXF<D> thisScore = new ScoresheetXF<D>();
                thisScore.Init(thisPlayer.Scoresheet!); // i think
                _sheetGrid.Children.Add(thisScore);
            }
            return Task.CompletedTask;
        }
        public async Task HandleAsync(WarningEventModel message)
        {
            bool rets = await DisplayAlert("Confirmation", message.Message, "Yes", "No");
            SelectionChosenEventModel results = new SelectionChosenEventModel();
            if (rets == false)
                results.OptionChosen = EnumOptionChosen.No;
            else
                results.OptionChosen = EnumOptionChosen.Yes;
            await _thisE!.PublishAsync(results);
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>>();
            RegisterNonSavedClasses();
            OurContainer.RegisterType<StandardRollProcesses<D, YahtzeePlayerItem<D>>>();
            RegisterDiceProportions(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, D>();
            OurContainer.RegisterType<ScoresheetVM<D>>(false); //this time, cant' be singleton since each player will have their own.
            OurContainer.RegisterType<BasicYahtzeeGame<D>>();
            OurContainer.RegisterType<YahtzeeSaveInfo<D>>();
            _thisE = OurContainer.Resolve<EventAggregator>();
        }
        protected abstract void RegisterNonSavedClasses();
        protected abstract void RegisterDiceProportions(string tag); //if i find another way to do it, will be here.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton!.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _sheetGrid = new Grid();
            thisStack.Children.Add(_sheetGrid);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(YahtzeeViewModel<D>.NormalTurn));
            firstInfo.AddRow("Roll", nameof(YahtzeeViewModel<D>.RollNumber)); // its bound now.
            firstInfo.AddRow("Status", nameof(YahtzeeViewModel<D>.Status));
            firstInfo.AddRow("Turn #", nameof(YahtzeeViewModel<D>.Round)); // i think
            _thisScore = new ScoreBoardXF();
            _thisScore.HorizontalOptions = LayoutOptions.Start;
            _thisScore.VerticalOptions = LayoutOptions.Start;
            _thisScore.AddColumn("Points", true, nameof(YahtzeePlayerItem<D>.Points));
            var thisRoll = GetGamingButton("Roll Dice", nameof(YahtzeeViewModel<D>.RollCommand));
            _diceControl = new DiceListControlXF<D>();
            thisStack.Children.Add(_diceControl);
            Grid finalGrid = new Grid();
            thisStack.Children.Add(finalGrid);
            GridHelper.AddLeftOverColumn(finalGrid, 50);
            GridHelper.AddLeftOverColumn(finalGrid, 50);
            StackLayout finalStack = new StackLayout();
            GridHelper.AddControlToGrid(finalGrid, finalStack, 0, 0); //try this too.
            finalStack.Children.Add(thisRoll);
            finalStack.Children.Add(_thisScore);
            GridHelper.AddControlToGrid(finalGrid, firstInfo.GetContent, 0, 1);
            MainGrid!.Children.Add(thisStack); //i think this was missing too.
            await FinishUpAsync();
        }
    }
}