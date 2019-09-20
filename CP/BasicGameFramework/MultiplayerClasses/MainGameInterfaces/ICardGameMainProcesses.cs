using BasicGameFramework.BasicDrawables.Interfaces;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface ICardGameMainProcesses<D> where D : IDeckObject, new()
    {
        Task ContinueTurnAsync(); //do need to keep the option open to be async.
        Task EndTurnAsync();
        Task DrawAsync(); //this for sure is needed.
        Task PickupFromDiscardAsync(); //we no longer have single.
        Task DiscardAsync(int deck);
        Task DiscardAsync(D thisCard);
        int PlayerDraws { get; set; }
        int LeftToDraw { get; set; }
    }
}