using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.StandardImplementations.CrossPlatform.AutoresumeClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.MiscClasses
{
    public class SinglePlayerStartUpClass : IStartUp
    {
        void IStartUp.RegisterCustomClasses(GamePackageDIContainer container, bool multiplayer, BasicData data)
        {
            if (multiplayer == true)
                throw new BasicBlankException("This can only be registered on single player classes");
            //i think the only thing that needs to be registered is the autoresume classes alone.
            container.RegisterType<SinglePlayerProductionSave>();
        }
        void IStartUp.StartVariables(BasicData data) { }
    }
}