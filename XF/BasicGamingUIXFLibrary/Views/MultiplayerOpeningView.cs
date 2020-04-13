using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace BasicGamingUIXFLibrary.Views
{
    public class MultiplayerOpeningView : ContentView, IUIView
    {
        private readonly IGameInfo _game;

        private Binding GetHostBinding => new Binding(nameof(IMultiplayerOpeningViewModel.HostCanStart));


        private Button PrivateGetButton(string text, string path)
        {
            if (_game!.SuggestedOrientation == EnumSuggestedOrientation.Landscape)
                return SharedUIFunctions.GetSmallerButton(text, path);
            Button output = SharedUIFunctions.GetGamingButton(text, path);
            output.FontSize = 14; //decided a little smaller for the main page.
            return output;
        }

        public MultiplayerOpeningView(IGameInfo game)
        {
            StackLayout tempStack;
            StackLayout mainStack = new StackLayout();
            mainStack.Margin = new Thickness(5);
            _game = game;

            if (_game.CanHaveExtraComputerPlayers)
            {
                tempStack = new StackLayout();
                tempStack.SetName(nameof(IMultiplayerOpeningViewModel.ExtraOptionsVisible));
                LoadPlayerOptions(EnumPlayOptions.ComputerExtra, tempStack);
                mainStack.Children.Add(tempStack);
            }
            //if bindings cannot be reused, then requires rethinking (?)
            Button button;
            string singleText;
            singleText = nameof(IMultiplayerOpeningViewModel.CanShowSingleOptions);
            if (_game.MinPlayers == 2)
            {
                button = PrivateGetButton("Start Game With No Extra Players.", nameof(IMultiplayerOpeningViewModel.StartAsync));
                button.CommandParameter = 0;
                button.SetBinding(IsVisibleProperty, GetHostBinding);
                mainStack.Children.Add(button);
            }
            button = PrivateGetButton("Auto Resume Networked Game", nameof(IMultiplayerOpeningViewModel.ResumeMultiplayerGameAsync));
            button.SetBinding(IsVisibleProperty, GetHostBinding);
            mainStack.Children.Add(button);
            button = PrivateGetButton("Auto Resume Local Game", nameof(IMultiplayerOpeningViewModel.ResumeSinglePlayerAsync));
            mainStack.Children.Add(button);
            bool canHuman;
            bool canComputer;
            canHuman = OpenPlayersHelper.CanHuman(_game);
            canComputer = OpenPlayersHelper.CanComputer(_game);
            if (canHuman && canComputer)
            {
                Grid tempgrid = new Grid();
                AddLeftOverColumn(tempgrid, 1);
                AddLeftOverColumn(tempgrid, 1);
                tempStack = new StackLayout();
                tempStack.SetName(singleText);
                AddControlToGrid(tempgrid, tempStack, 0, 0);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                tempStack = new StackLayout();
                tempStack.SetName(singleText);
                AddControlToGrid(tempgrid, tempStack, 0, 1);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                mainStack.Children.Add(tempgrid);
            }
            else if (canHuman)
            {
                tempStack = new StackLayout();
                tempStack.SetName(singleText);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                mainStack.Children.Add(tempStack);
            }
            else if (canComputer)
            {
                tempStack = new StackLayout();
                tempStack.SetName(singleText);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                mainStack.Children.Add(tempStack);
            }
            else if (_game.SinglePlayerChoice == EnumPlayerChoices.Solitaire)
            {
                tempStack = new StackLayout();
                tempStack.SetName(singleText);
                LoadPlayerOptions(EnumPlayOptions.Solitaire, tempStack);
                mainStack.Children.Add(tempStack);
            }
            button = PrivateGetButton("Start Network Game (Host)", nameof(IMultiplayerOpeningViewModel.HostAsync));
            mainStack.Children.Add(button);
            button = PrivateGetButton("Join Network Game", nameof(IMultiplayerOpeningViewModel.ConnectAsync));
            mainStack.Children.Add(button);
            button = PrivateGetButton("Cancel Selection", nameof(IMultiplayerOpeningViewModel.CancelConnectionAsync));
            mainStack.Children.Add(button);
            tempStack = new StackLayout();
            tempStack.SetName(nameof(IMultiplayerOpeningViewModel.HostCanStart));
            SimpleLabelGridXF playerInfo = new SimpleLabelGridXF();
            playerInfo.AddRow("Players Connected", nameof(IMultiplayerOpeningViewModel.ClientsConnected));
            playerInfo.AddRow("Previous Players", nameof(IMultiplayerOpeningViewModel.PreviousNonComputerNetworkedPlayers));
            tempStack.Children.Add(playerInfo.GetContent);
            mainStack.Children.Add(tempStack);
            Content = mainStack;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        private void LoadPlayerOptions(EnumPlayOptions whichOption, StackLayout stack)
        {
            Button thisBut;
            string path;
            string header;
            if (whichOption == EnumPlayOptions.Solitaire)
            {
                header = "New Single Player Game";
                thisBut = PrivateGetButton(header, nameof(IMultiplayerOpeningViewModel.SolitaireAsync));
                stack.Children.Add(thisBut);
                return;
            }
            var tempList = OpenPlayersHelper.GetPossiblePlayers(_game);
            if (whichOption == EnumPlayOptions.ComputerLocal && _game.MinPlayers == 3)
                tempList.RemoveFirstItem();
            foreach (var thisTemp in tempList)
            {
                int display;
                if (whichOption == EnumPlayOptions.HumanLocal)
                    display = thisTemp + 1;
                else
                    display = thisTemp;
                if (whichOption == EnumPlayOptions.ComputerExtra)
                {
                    path = nameof(IMultiplayerOpeningViewModel.StartAsync);
                    header = display + " Extra Computer Players";
                }
                else if (whichOption == EnumPlayOptions.ComputerLocal)
                {
                    path = nameof(IMultiplayerOpeningViewModel.StartComputerSinglePlayerGameAsync);
                    header = display + " Local Computer Players";
                }
                else
                {
                    path = nameof(IMultiplayerOpeningViewModel.StartPassAndPlayGameAsync);
                    header = display + " Pass And Play Human Players";
                }
                thisBut = PrivateGetButton(header, path);
                thisBut.CommandParameter = display;
                stack.Children.Add(thisBut);
            }
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
