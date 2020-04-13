using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace BasicGamingUIXFLibrary.Views
{
    public class YahtzeeMainView<D> : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandleAsync<WarningEventModel>
        where D : SimpleDice, new()
    {
        private readonly IEventAggregator _aggregator;
        private readonly YahtzeeVMData<D> _model;
        private readonly ScoreBoardXF _score;
        private readonly DiceListControlXF<D> _diceControl; //hopefully still okay (?)



        public YahtzeeMainView(IEventAggregator aggregator, YahtzeeVMData<D> model)
        {
            _aggregator = aggregator;
            _aggregator.Publish(this);
            _model = model;
            _aggregator.Subscribe(this);
            _diceControl = new DiceListControlXF<D>();

            Grid eGrid = new Grid();
            GridHelper.AddLeftOverColumn(eGrid, 50);
            GridHelper.AddLeftOverColumn(eGrid, 50);
            ParentSingleUIContainer sheetGrid = new ParentSingleUIContainer(nameof(YahtzeeMainViewModel<D>.CurrentScoresheet));
            StackLayout stack = new StackLayout();
            stack.Children.Add(sheetGrid);

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(YahtzeeMainViewModel<D>.NormalTurn));
            firstInfo.AddRow("Roll", nameof(YahtzeeMainViewModel<D>.RollNumber)); // its bound now.
            firstInfo.AddRow("Status", nameof(YahtzeeMainViewModel<D>.Status));
            firstInfo.AddRow("Turn #", nameof(YahtzeeMainViewModel<D>.Round)); // i think
            stack.Children.Add(firstInfo.GetContent);
            _score = new ScoreBoardXF();
            _score.AddColumn("Points", false, nameof(YahtzeePlayerItem<D>.Points));
            _score.HorizontalOptions = LayoutOptions.Start;
            _score.VerticalOptions = LayoutOptions.Start;
            var thisRoll = GetGamingButton("Roll Dice", nameof(YahtzeeMainViewModel<D>.RollDiceAsync));
            stack.Children.Add(_diceControl);
            
            stack.Children.Add(eGrid);
            StackLayout finalStack = new StackLayout();
            GridHelper.AddControlToGrid(eGrid, finalStack, 0, 0); //try this too.
            finalStack.Children.Add(thisRoll);
            finalStack.Children.Add(_score);
            GridHelper.AddControlToGrid(eGrid, firstInfo.GetContent, 0, 1);

            Content = stack;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            YahtzeeSaveInfo<D> save = cons!.Resolve<YahtzeeSaveInfo<D>>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            return this.RefreshBindingsAsync(_aggregator);

        }

        async Task IHandleAsync<WarningEventModel>.HandleAsync(WarningEventModel message)
        {

            bool rets = await Application.Current.MainPage.DisplayAlert("Confirmation", message.Message, "Yes", "No");
            SelectionChosenEventModel results = new SelectionChosenEventModel();
            if (rets == false)
                results.OptionChosen = EnumOptionChosen.No;
            else
                results.OptionChosen = EnumOptionChosen.Yes;
            await _aggregator!.PublishAsync(results);
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
