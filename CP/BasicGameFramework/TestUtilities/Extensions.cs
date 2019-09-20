using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace BasicGameFramework.TestUtilities
{
    public static class Extensions
    {
        public static void RegisterTestHands<D, P, T>(this IGamePackageDIContainer thisContainer)
            where D : IDeckObject, new()
        where P : class, IPlayerObject<D>, new()
            where T : ITestCardSetUp<D, P>
        {
            thisContainer.RegisterType<T>(true);
        }
    }
}