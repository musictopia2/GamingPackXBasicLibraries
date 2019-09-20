using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IMissTurnClass<P> where P : IPlayerItem
    {
        Task PlayerMissTurnAsync(P player);
    }
}