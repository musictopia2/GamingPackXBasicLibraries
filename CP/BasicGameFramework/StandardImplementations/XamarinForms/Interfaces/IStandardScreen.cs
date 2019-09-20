using BasicGameFramework.BasicGameDataClasses;
namespace BasicGameFramework.StandardImplementations.XamarinForms.Interfaces
{
    /// <summary>
    /// This is intended to be a standard process.  Something else can be more specific (probably a personal library).
    /// For now, needs to know if its the smallest device.  Also needs to know for a given game whether it can even play it or not.  Takes in IGameInfo object.
    /// </summary>
    public interface IStandardScreen
    {
        bool IsSmallest { get; }
        bool CanPlay(IGameInfo game);
    }
}