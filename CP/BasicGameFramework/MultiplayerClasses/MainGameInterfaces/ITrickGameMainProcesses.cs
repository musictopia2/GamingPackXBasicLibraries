using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface ITrickGameMainProcesses<SU, T, P, SA> : ICardGameMainProcesses<T>
        where SU : struct, Enum
        where T : ITrickCard<SU>, new()
        where P : class, IPlayerTrick<SU, T>, new()
        where SA : BasicSavedTrickGamesClass<SU, T, P>
    {
        PlayerCollection<P>? PlayerList { get; set; }
        T GetSpecificCardFromDeck(int deck);
        T GetBrandNewCard(int deck);
        SA? SaveRoot { get; set; } //well see how this works.
        Task CardClickedAsync();
        Task ContinueTrickAsync();
        Task EndTrickAsync();
        P? SingleInfo { get; set; }
        int WhoTurn { get; set; }
        int SelfPlayer { get; } //i like this idea.
    }
}