using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
namespace BasicGameFramework.StandardImplementations.XamarinForms.Interfaces
{
    /// <summary>
    /// This is anything that is platform specific including closing the app.
    /// or even changing orientation if its even possible.
    /// </summary>
    public interface IGamePlatform
    {
        void CloseApp(); //if i can't figure out how to close via uwp, then too bad unfortunately.
        void SupportedOrientation(IGameInfo game); //i like the idea of using the game as guideline but the platform can do something different if it desires.
        void SetUp(GamePackageDIContainer container); //this maybe even better.  so they can setup whatever they want.  something else later will decide whether to close out.
        void ResetPopups();
    }
}