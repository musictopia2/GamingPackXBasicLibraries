using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.ViewModelInterfaces;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace BaseGPXPagesAndControlsXF.BasePageProcesses.Pages
{
    public enum EnumPlayOptions
    {
        HumanLocal = 1,
        ComputerLocal = 2,
        ComputerExtra = 3,
        Solitaire = 4 //needs to add a new option.
    }
    public abstract class MultiPlayerPage<VM, P, S> : BasicGamePage<VM>
        where VM : BaseViewModel, IBasicGameVM, ISimpleMultiPlayerVM, IRoundCommand
        where S : BasicSavedGameClass<P>, new()
        where P : class, IPlayerItem, new()
    {
        protected Grid? MainGrid;
        protected MultiplayerOpeningVM<P, S>? OpenMod;
        protected override bool UseMultiplayerProcesses => true;
        private Button PrivateGetButton(string text, string path)
        {
            if (ThisGame!.SuggestedOrientation == EnumSuggestedOrientation.Landscape)
                return GetSmallerButton(text, path);
            Button output = GetGamingButton(text, path);
            output.FontSize = 14; //decided a little smaller for the main page.
            return output;
        }
        private Binding GetHostBinding => new Binding(nameof(MultiplayerOpeningVM<P, S>.HostCanStart));
        private Binding GetClientBinding => new Binding(nameof(MultiplayerOpeningVM<P, S>.CanShowSingleOptions));
        protected void BasicSetUp(View? extraControl = null)
        {
            Grid thisGrid = new Grid();
            MainGrid = new Grid();
            var thisBind = new Binding(nameof(ISimpleMultiPlayerVM.MainOptionsVisible));
            MainGrid.SetBinding(IsVisibleProperty, thisBind);
            thisGrid.Children.Add(MainGrid);
            Grid firstGrid = new Grid();
            OpenMod = OurContainer.Resolve<MultiplayerOpeningVM<P, S>>();
            ThisRestore = OurContainer.Resolve<RestoreVM>();
            firstGrid.BindingContext = OpenMod;
            thisBind = new Binding(nameof(MultiplayerOpeningVM<P, S>.Visible));
            thisGrid.Children.Add(firstGrid);
            firstGrid.SetBinding(IsVisibleProperty, thisBind);
            StackLayout newStack = new StackLayout();
            newStack.Margin = new Thickness(5, 5, 5, 5);
            Button thisBut;
            StackLayout tempStack;
            if (ThisGame!.CanHaveExtraComputerPlayers)
            {
                tempStack = new StackLayout();
                thisBind = new Binding(nameof(MultiplayerOpeningVM<P, S>.ExtraOptionsVisible));
                tempStack.SetBinding(IsVisibleProperty, thisBind);
                LoadPlayerOptions(EnumPlayOptions.ComputerExtra, tempStack);
                newStack.Children.Add(tempStack);
            }
            if (ThisGame!.MinPlayers == 2)
            {
                thisBut = PrivateGetButton("Start Game With No Extra Players.", nameof(MultiplayerOpeningVM<P, S>.StartCommand));
                thisBut.CommandParameter = 0; // to show no extra players in this case
                thisBut.SetBinding(IsVisibleProperty, GetHostBinding);
                newStack.Children.Add(thisBut); // try this
            }
            thisBut = PrivateGetButton("Auto Resume Networked Game", nameof(MultiplayerOpeningVM<P, S>.ResumeMultiplayerGameCommand));
            thisBut.SetBinding(IsVisibleProperty, GetHostBinding);
            newStack.Children.Add(thisBut);
            thisBut = PrivateGetButton("Auto Resume Local Game", nameof(MultiplayerOpeningVM<P, S>.ResumeSinglePlayerCommand));
            newStack.Children.Add(thisBut);
            if (OpenMod.CanComputer == true && OpenMod.CanHuman == true)
            {
                Grid tempGrid = new Grid();
                GridHelper.AddLeftOverColumn(tempGrid, 50);
                GridHelper.AddLeftOverColumn(tempGrid, 50); // half for one and half for another
                tempStack = new StackLayout();
                tempStack.SetBinding(IsVisibleProperty, GetClientBinding); //well see
                tempStack.Margin = new Thickness(10, 10, 10, 10);
                GridHelper.AddControlToGrid(tempGrid, tempStack, 0, 0);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                tempStack = new StackLayout();
                tempStack.SetBinding(IsVisibleProperty, GetClientBinding);
                tempStack.Margin = new Thickness(10, 10, 10, 10);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                GridHelper.AddControlToGrid(tempGrid, tempStack, 0, 1);
                newStack.Children.Add(tempGrid); // i think
            }
            else if (OpenMod.CanHuman == true)
            {
                // single column of human players
                tempStack = new StackLayout();
                tempStack.SetBinding(IsVisibleProperty, GetClientBinding);
                LoadPlayerOptions(EnumPlayOptions.HumanLocal, tempStack);
                newStack.Children.Add(tempStack);
            }
            else if (OpenMod.CanComputer == true)
            {
                // single column of computer players
                tempStack = new StackLayout();
                tempStack.SetBinding(IsVisibleProperty, GetClientBinding);
                LoadPlayerOptions(EnumPlayOptions.ComputerLocal, tempStack);
                newStack.Children.Add(tempStack);
            }
            else if (ThisGame!.SinglePlayerChoice == EnumPlayerChoices.Solitaire)
            {
                tempStack = new StackLayout();
                tempStack.SetBinding(IsVisibleProperty, GetClientBinding);
                LoadPlayerOptions(EnumPlayOptions.Solitaire, tempStack);
                newStack.Children.Add(tempStack);
            }
            thisBut = PrivateGetButton("Start Network Game (Host)", nameof(MultiplayerOpeningVM<P, S>.HostCommand)); // i like the idea of start and join.
            newStack.Children.Add(thisBut);
            thisBut = PrivateGetButton("Join Network Game", nameof(MultiplayerOpeningVM<P, S>.ConnectCommand));
            newStack.Children.Add(thisBut);
            thisBut = PrivateGetButton("Cancel Selection", nameof(MultiplayerOpeningVM<P, S>.CancelCommand));
            newStack.Children.Add(thisBut);
            tempStack = new StackLayout();
            tempStack.SetBinding(IsVisibleProperty, GetHostBinding);
            SimpleLabelGridXF PlayerInfo = new SimpleLabelGridXF();
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
        protected virtual void ComplexStartControls(Grid thisGrid) { }
        protected Button? RoundButton;
        public MultiPlayerPage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        protected override void OtherCommonButtons()
        {
            RoundButton = GetGamingButton("New Round", nameof(IRoundCommand.NewRoundCommand));
            RoundButton.SetBinding(IsVisibleProperty, new Binding(nameof(ISimpleMultiPlayerVM.NewRoundVisible)));
        }
        protected async Task FinishUpAsync()
        {
            if (OpenMod == null)
                throw new BasicBlankException("The opening view model was never created");
            await OpenMod.FinishStartAsync();
        }
        private void LoadPlayerOptions(EnumPlayOptions whichOption, StackLayout thisStack)
        {
            Button thisBut;
            string path;
            string header;
            if (whichOption == EnumPlayOptions.Solitaire)
            {
                header = "New Single Player Game";
                thisBut = PrivateGetButton(header, nameof(MultiplayerOpeningVM<P, S>.SolitaireCommand));
                thisStack.Children.Add(thisBut);
                return;
            }
            var tempList = OpenMod!.GetPossiblePlayers();
            if (whichOption == EnumPlayOptions.ComputerLocal && ThisGame!.MinPlayers == 3)
                tempList.RemoveFirstItem();
            if (Screen!.IsSmallest)
            {
                if (whichOption == EnumPlayOptions.HumanLocal)
                    tempList.RemoveAllAndObtain(items => items > 5);
                else
                    tempList.RemoveAllAndObtain(items => items > 6);
            }
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
                thisBut = PrivateGetButton(header, path);
                thisBut.CommandParameter = display;
                thisStack.Children.Add(thisBut);
            }
        }
    }
}