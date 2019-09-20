using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.NetworkingClasses.Interfaces;
namespace BasicGameFramework.MultiplayerClasses.InterfacesForHelpers
{
    public interface IBasicGameProcesses<P>
        where P : class, IPlayerItem, new()
    {
        PlayerCollection<P>? PlayerList { get; set; } //found more than one use for it now.
        ISimpleMultiPlayerVM CurrentMod { get; } //hopefully okay.  in this case, no generics required.
        P? SingleInfo { get; set; }
        BasicData? ThisData { get; }
        INetworkMessages? ThisNet { get; set; }
        IMessageChecker? ThisCheck { get; set; }
        void Init(); //this is needed so for some things you have to do here to stop the overflow issues.
        void RegisterOpening(IGamePackageDIContainer tempContainer);
    }
}