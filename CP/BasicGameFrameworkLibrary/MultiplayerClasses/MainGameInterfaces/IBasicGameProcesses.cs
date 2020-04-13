using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces
{
    public interface IBasicGameProcesses<P> : IAggregatorContainer
        where P : class, IPlayerItem, new()
    {
        PlayerCollection<P> PlayerList { get; set; } //found more than one use for it now.

        //we really have to do with no more view model dependency.

        P? SingleInfo { get; set; }
        BasicData BasicData { get; }
        INetworkMessages? Network { get; set; }
        IMessageChecker? Check { get; set; }
        IViewModelData CurrentMod { get; }
        bool CanMakeMainOptionsVisibleAtBeginning { get; } //for card games, default to true.  but can make it overridable.

        //hopefully no need for init.  hope i can find other ways to stop the overflows.

        //void Init(); //this is needed so for some things you have to do here to stop the overflow issues.
        //void RegisterOpening(IGamePackageDIContainer tempContainer);
    }
}