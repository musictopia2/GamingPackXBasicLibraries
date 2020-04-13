using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIWPFLibrary.Views
{
    public class YahtzeeMainView<D> : UserControl, IUIView, IHandleAsync<LoadEventModel>
        where D : SimpleDice, new()
    {
        private readonly IEventAggregator _aggregator;
        private readonly YahtzeeVMData<D> _model;
        private readonly ScoreBoardWPF _score;
        private readonly DiceListControlWPF<D> _diceControl; //hopefully still okay (?)

        public YahtzeeMainView(IEventAggregator aggregator, YahtzeeVMData<D> model)
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Publish(this);
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _diceControl = new DiceListControlWPF<D>();

            Grid eGrid = new Grid();
            GridHelper.AddAutoColumns(eGrid, 2);
            GridHelper.AddAutoRows(eGrid, 2);
            ParentSingleUIContainer sheetGrid = new ParentSingleUIContainer()
            {
                Name = nameof(YahtzeeMainViewModel<D>.CurrentScoresheet)
            };
            StackPanel stack = new StackPanel();

            GridHelper.AddControlToGrid(eGrid, sheetGrid, 0, 0);
            GridHelper.AddControlToGrid(eGrid, stack, 0, 1);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(YahtzeeMainViewModel<D>.NormalTurn));
            firstInfo.AddRow("Roll", nameof(YahtzeeMainViewModel<D>.RollNumber)); // its bound now.
            firstInfo.AddRow("Status", nameof(YahtzeeMainViewModel<D>.Status));
            firstInfo.AddRow("Turn #", nameof(YahtzeeMainViewModel<D>.Round)); // i think
            stack.Children.Add(firstInfo.GetContent);
            _score = new ScoreBoardWPF();
            _score.AddColumn("Points", false, nameof(YahtzeePlayerItem<D>.Points));
            stack.Children.Add(_score);
            var thisRoll = GetGamingButton("Roll Dice", nameof(YahtzeeMainViewModel<D>.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<D>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            GridHelper.AddControlToGrid(eGrid, otherStack, 1, 0);
            Grid.SetColumnSpan(otherStack, 2);
            Content = eGrid;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            //GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            YahtzeeSaveInfo<D> save = cons!.Resolve<YahtzeeSaveInfo<D>>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            return this.RefreshBindingsAsync(_aggregator);

        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
