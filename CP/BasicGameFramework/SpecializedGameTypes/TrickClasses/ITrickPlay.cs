using System.Threading.Tasks;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public interface ITrickPlay
    {
        Task PlayCardAsync(int deck);
    }
}