using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.StandardImplementations.CrossPlatform.AutoresumeClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace GamePackageSignalRClasses
{
    public class MainStartUp : IStartUp
    {
        void IStartUp.RegisterCustomClasses(GamePackageDIContainer container, bool multiplayer, BasicData data)
        {
            if (multiplayer == false)
                throw new BasicBlankException("Only multiplayer are supported.  For single player, try different startup class that does not rely on signal r");
            container.RegisterType<NetworkStartUp>();
            if (_global == null)
                _global = GlobalDataLoaderClass.Open(data.IsXamarinForms);
            container.RegisterSingleton(_global);
            container.RegisterType<MultiplayerProductionSave>();
        }
        private GlobalDataModel? _global;
        void IStartUp.StartVariables(BasicData data) //i think this is it for this one.
        {
            //this is where we get nick name, etc.
            //i think that here is where i could host a server as well (if necessary).
            if (_global == null)
                _global = GlobalDataLoaderClass.Open(data.IsXamarinForms);
            data.NickName = GlobalDataLoaderClass.CurrentNickName(_global);
        }
    }
}