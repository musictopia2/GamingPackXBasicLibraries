using System.Threading.Tasks;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public interface IAdvancedTrickProcesses
    {
        Task AnimateWinAsync(int wins);
        void ClearBoard();
        void FirstLoad();
        void LoadGame(); //just call it load game.  this is used for autoresume.
    }
}