using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    public interface IGameSetUp<P, S> : IBasicGameProcesses<P>
        where P : class, IPlayerItem, new()
        where S : BasicSavedGameClass<P>
    {
        Task SetUpGameAsync(bool IsBeginning);
        Task PopulateSaveRootAsync();
        S? SaveRoot { get; set; }
        BasicGameLoader<P, S>? ThisLoader { get; set; }
        Task StartNewTurnAsync(); //sometimes, it has to startnewturn if its beginning.
        bool ComputerEndsTurn { set; }
        Task ContinueTurnAsync();
        Task FinishGetSavedAsync();
        bool CanMakeMainOptionsVisibleAtBeginning { get; } //for card games, default to true.  but can make it overridable.
    }
}