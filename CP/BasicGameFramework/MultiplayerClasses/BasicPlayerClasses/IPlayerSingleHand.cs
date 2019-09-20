using BasicGameFramework.BasicDrawables.Interfaces;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerSingleHand<D> : IPlayerItem, IPlayerObject<D> where D : IDeckObject, new()
    {
        int ObjectCount { get; set; }
        void HookUpHand();
    }
}