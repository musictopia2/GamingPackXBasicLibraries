using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses
{
    //may require rethinking to stop the overflows.
    public interface ITrickPlay
    {
        Task PlayCardAsync(int deck);
    }
}