using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.AutoresumeClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.MiscClasses
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