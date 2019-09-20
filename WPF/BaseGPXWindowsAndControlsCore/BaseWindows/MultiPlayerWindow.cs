using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace BaseGPXWindowsAndControlsCore.BaseWindows
{
    public enum EnumPlayOptions
    {
        HumanLocal = 1,
        ComputerLocal = 2,
        ComputerExtra = 3,
        Solitaire = 4 //needs to add a new option.
    }
    public abstract class MultiPlayerWindow<VM, P, S> : BasicGameWindow<VM>
        where VM : BaseViewModel, IBasicGameVM, ISimpleMultiPlayerVM, IRoundCommand
        where P : class, IPlayerItem, new()
        where S: BasicSavedGameClass<P>, new()
    {
        protected Grid? MainGrid;
        protected MultiplayerOpeningVM<P, S>? OpenMod;
        protected void BasicSetUp(UIElement? extraControl = null)
        {
            Grid thisGrid = new Grid();
            MainGrid = new Grid();
            var thisBind = GetVisibleBinding(nameof(ISimpleMultiPlayerVM.MainOptionsVisible));
            MainGrid.SetBinding(VisibilityProperty, thisBind);
            thisGrid.Children.Add(MainGrid);
            Grid firstGrid = new Grid();
            OpenMod = OurContainer!.Resolve<MultiplayerOpeningVM<P, S>>();
            ThisRestore = OurContainer.Resolve<RestoreVM>();
            firstGrid.DataContext = OpenMod;
            thisBind = GetVisibleBinding(nameof(MultiplayerOpeningVM<P, S>.Visible));
            thisGrid.Children.Add(firstGrid);
            firstGrid.SetBinding(VisibilityProperty, thisBind);
            StackPanel newStack = new StackPanel();
            newStack.Margin = new Thickness(5, 5, 5, 5);
            Button thisBut;
            StackPanel tempStack;
            if (ThisGame!.CanHaveExtraComputerPlayers)
            {
                tempStack = new StackPanel();
                thisBind = GetVisibleBinding(nameof(MultiplayerOpeningVM<P, S>.ExtraOptionsVisible));
                tempStack.SetBinding(VisibilityProperty, thisBind);
                LoadPlayerOptions(EnumPlayOptions.ComputerExtra, tempStack);
                newStack.Children.Add(tempStack);
            }
            var hostBind = GetVisibleBinding(nameof(MultiplayerOpeningVM<P, S>.HostCanStart));
            var clientBind = GetVisibleBinding(nameof(MultiplayerOpeningVM<P, S>.CanShowSingleOptions));
            if (ThisGame!.MinPlayers == 2)
            {
                thisBut = GetGamingButton("Start Game With No Extra Players.", nameof(MultiplayerOpeningVM<P, S>.StartCommand));
                thisBut.CommandParameter = 0; // to show no extra players in this case
                thisBut.SetBinding(Button.VisibilityProperty, hostBind);
                newStack.Children.Add(thisBut); // try this
            }
            thisBut = GetGamingButton("Auto Resume Networked Game", nameof(MultiplayerOpeningVM<P, S>.ResumeMultiplayerGameCommand));
            thisBut.SetBinding(VisibilityProperty, hostBind);
            newStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Auto Resume Local Game", nameof(MultiplayerOpeningVM<P, S>.ResumeSinglePlayerCommand));
            newStack.Children.Add(thisBut);
            if (OpenMod.CanComputer == true && OpenMod.CanHuman == true)
            {
                Grid tempGrid = new Grid();
                GridHelper.AddLeftOverColumn(tempGrid, 50);
                GridHelper.AddLeftOverColumn(tempGrid, 50); // half for one and half for another
                tempStack = new StackPanel();
                tempStack.SetBinding(VisibilityProperty, clientBind);
                tempStack.Margin = new Thickness(10, 10, 10, 10);
                GridHelper.AddControlToGrid(tempGrid, tempStack, 0, 0);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                tempStack = new StackPanel();
                tempStack.SetBinding(VisibilityProperty, clientBind);
                tempStack.Margin = new Thickness(10, 10, 10, 10);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                GridHelper.AddControlToGrid(tempGrid, tempStack, 0, 1);
                newStack.Children.Add(tempGrid); // i think
            }
            else if (OpenMod.CanHuman == true)
            {
                // single column of human players
                tempStack = new StackPanel();
                tempStack.SetBinding(VisibilityProperty, clientBind);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                newStack.Children.Add(tempStack);
            }
            else if (OpenMod.CanComputer == true)
            {
                // single column of computer players
                tempStack = new StackPanel();
                tempStack.SetBinding(VisibilityProperty, clientBind);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                newStack.Children.Add(tempStack);
            }
            else if (ThisGame!.SinglePlayerChoice == EnumPlayerChoices.Solitaire)
            {
                tempStack = new StackPanel();
                tempStack.SetBinding(VisibilityProperty, clientBind);
                LoadPlayerOptions(EnumPlayOptions.Solitaire, tempStack);
                newStack.Children.Add(tempStack);
            }
            thisBut = GetGamingButton("Start Network Game (Host)", nameof(MultiplayerOpeningVM<P, S>.HostCommand)); // i like the idea of start and join.
            newStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Join Network Game", nameof(MultiplayerOpeningVM<P, S>.ConnectCommand));
            newStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Cancel Selection", nameof(MultiplayerOpeningVM<P, S>.CancelCommand));
            newStack.Children.Add(thisBut);
            tempStack = new StackPanel();
            tempStack.SetBinding(VisibilityProperty, hostBind);
            SimpleLabelGrid PlayerInfo = new SimpleLabelGrid();
            PlayerInfo.AddRow("Players Connected", nameof(MultiplayerOpeningVM<P, S>.ClientsConnected));
            PlayerInfo.AddRow("Previous Players", nameof(MultiplayerOpeningVM<P, S>.PreviousNonComputerNetworkedPlayers));
            tempStack.Children.Add(PlayerInfo.GetContent);
            newStack.Children.Add(tempStack);
            firstGrid.Children.Add(newStack);
            if (extraControl != null)
                thisGrid.Children.Add(extraControl);
            ComplexStartControls(thisGrid);
            Content = thisGrid;
        }
        protected override bool UseMultiplayerProcesses => true;
        protected virtual void ComplexStartControls(Grid thisGrid) { }

        protected Button? RoundButton;
        protected override void OtherCommonButtons()
        {
            RoundButton = GetGamingButton("New Round", nameof(IRoundCommand.NewRoundCommand));
            RoundButton.SetBinding(VisibilityProperty, GetVisibleBinding(nameof(ISimpleMultiPlayerVM.NewRoundVisible)));
        }
        protected async Task FinishUpAsync()
        {
            if (OpenMod == null)
                throw new BasicBlankException("The opening view model was never created");
            await OpenMod.FinishStartAsync();
        }
        private void LoadPlayerOptions(EnumPlayOptions whichOption, StackPanel thisStack)
        {
            Button thisBut;
            string path;
            string header;
            if (whichOption == EnumPlayOptions.Solitaire)
            {
                header = "New Single Player Game";
                thisBut = SharedWindowFunctions.GetGamingButton(header, nameof(MultiplayerOpeningVM<P, S>.SolitaireCommand));
                thisStack.Children.Add(thisBut);
                return;
            }
            var tempList = OpenMod!.GetPossiblePlayers();
            if (whichOption == EnumPlayOptions.ComputerLocal && ThisGame!.MinPlayers == 3)
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
                    path = nameof(MultiplayerOpeningVM<P, S>.StartCommand);
                    header = display + " Extra Computer Players";
                }
                else if (whichOption == EnumPlayOptions.ComputerLocal)
                {
                    path = nameof(MultiplayerOpeningVM<P, S>.ComputerCommand);
                    header = display + " Local Computer Players";
                }
                else
                {
                    path = nameof(MultiplayerOpeningVM<P, S>.HumanCommand);
                    header = display + " Pass And Play Human Players";
                }
                thisBut = GetGamingButton(header, path);
                thisBut.CommandParameter = display;
                thisStack.Children.Add(thisBut);
            }
        }
    }
}