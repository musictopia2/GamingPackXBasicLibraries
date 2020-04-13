using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace BasicGamingUIWPFLibrary.Views
{
    //attempt to do this without generics via interfaces.
    public class MultiplayerOpeningView : UserControl, IUIView
    {
        private readonly IGameInfo _game;

        public MultiplayerOpeningView(IGameInfo game)
        {
            StackPanel tempStack;
            StackPanel mainStack = new StackPanel();
            mainStack.Margin = new Thickness(5);
            _game = game;

            if (_game.CanHaveExtraComputerPlayers)
            {
                tempStack = new StackPanel()
                {
                    Name = nameof(IMultiplayerOpeningViewModel.ExtraOptionsVisible)
                };
                LoadPlayerOptions(EnumPlayOptions.ComputerExtra, tempStack);
                mainStack.Children.Add(tempStack);
            }

            Binding hostBind = SharedUIFunctions.GetVisibleBinding(nameof(IMultiplayerOpeningViewModel.HostCanStart));
            string singleText;
            singleText = nameof(IMultiplayerOpeningViewModel.CanShowSingleOptions);
            Binding clientBind = SharedUIFunctions.GetVisibleBinding(singleText);
            Button button;
            if (_game.MinPlayers == 2)
            {
                button = SharedUIFunctions.GetGamingButton("Start Game With No Extra Players.", nameof(IMultiplayerOpeningViewModel.StartAsync));
                button.CommandParameter = 0;
                button.SetBinding(VisibilityProperty, hostBind);
                mainStack.Children.Add(button);
            }
            button = SharedUIFunctions.GetGamingButton("Auto Resume Networked Game", nameof(IMultiplayerOpeningViewModel.ResumeMultiplayerGameAsync));
            button.SetBinding(VisibilityProperty, hostBind);
            mainStack.Children.Add(button);
            button = SharedUIFunctions.GetGamingButton("Auto Resume Local Game", nameof(IMultiplayerOpeningViewModel.ResumeSinglePlayerAsync));
            mainStack.Children.Add(button);
            //next issue.  needs functions for cancomputer, canhuman
            bool canHuman;
            bool canComputer;
            canHuman = OpenPlayersHelper.CanHuman(_game);
            canComputer = OpenPlayersHelper.CanComputer(_game);
            if (canHuman && canComputer)
            {
                Grid tempgrid = new Grid();
                AddLeftOverColumn(tempgrid, 1);
                AddLeftOverColumn(tempgrid, 1);
                tempStack = new StackPanel()
                {
                    Name = singleText,
                    //Margin = new Thickness(10)
                };
                AddControlToGrid(tempgrid, tempStack, 0, 0);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                tempStack = new StackPanel()
                {
                    Name = singleText
                };
                AddControlToGrid(tempgrid, tempStack, 0, 1);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                mainStack.Children.Add(tempgrid);
            }
            else if (canHuman)
            {
                tempStack = new StackPanel()
                {
                    Name = singleText
                };
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                mainStack.Children.Add(tempStack);
            }
            else if (canComputer)
            {
                tempStack = new StackPanel()
                {
                    Name = singleText
                };
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                mainStack.Children.Add(tempStack);
            }
            else if (_game.SinglePlayerChoice == EnumPlayerChoices.Solitaire)
            {
                tempStack = new StackPanel()
                {
                    Name = singleText
                };
                LoadPlayerOptions(EnumPlayOptions.Solitaire, tempStack);
                mainStack.Children.Add(tempStack);
            }
            button = SharedUIFunctions.GetGamingButton("Start Network Game (Host)", nameof(IMultiplayerOpeningViewModel.HostAsync));
            mainStack.Children.Add(button);
            button = SharedUIFunctions.GetGamingButton("Join Network Game", nameof(IMultiplayerOpeningViewModel.ConnectAsync));
            mainStack.Children.Add(button);
            button = SharedUIFunctions.GetGamingButton("Cancel Selection", nameof(IMultiplayerOpeningViewModel.CancelConnectionAsync));
            mainStack.Children.Add(button);
            tempStack = new StackPanel()
            {
                Name = nameof(IMultiplayerOpeningViewModel.HostCanStart)
            };
            SimpleLabelGrid playerInfo = new SimpleLabelGrid();
            playerInfo.AddRow("Players Connected", nameof(IMultiplayerOpeningViewModel.ClientsConnected));
            playerInfo.AddRow("Previous Players", nameof(IMultiplayerOpeningViewModel.PreviousNonComputerNetworkedPlayers));
            tempStack.Children.Add(playerInfo.GetContent);
            mainStack.Children.Add(tempStack);
            Content = mainStack;
        }

        private void LoadPlayerOptions(EnumPlayOptions whichOption, StackPanel stack)
        {
            Button thisBut;
            string path;
            string header;
            if (whichOption == EnumPlayOptions.Solitaire)
            {
                header = "New Single Player Game";
                thisBut = SharedUIFunctions.GetGamingButton(header, nameof(IMultiplayerOpeningViewModel.SolitaireAsync));
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
                thisBut = SharedUIFunctions.GetGamingButton(header, path);
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