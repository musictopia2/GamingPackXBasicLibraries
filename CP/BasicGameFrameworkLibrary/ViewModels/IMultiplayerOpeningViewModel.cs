using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.ViewModels
{
    public interface IMultiplayerOpeningViewModel : IScreen
    {
        int ClientsConnected { get; }
        bool ExtraOptionsVisible { get; set; }
        bool HostCanStart { get; }
        EnumOpeningStatus OpeningStatus { get; set; }
        int PreviousNonComputerNetworkedPlayers { get; set; }
        bool CanShowSingleOptions { get; }
        Task ConnectAsync();
        Task HostAsync();
        Task ResumeMultiplayerGameAsync();
        Task ResumeSinglePlayerAsync();
        Task SolitaireAsync();
        Task StartAsync(int howManyExtra);
        Task StartComputerSinglePlayerGameAsync(int howManyComputerPlayers);
        Task StartPassAndPlayGameAsync(int howManyHumanPlayers);
        Task CancelConnectionAsync();
    }
}