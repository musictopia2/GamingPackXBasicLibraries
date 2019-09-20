using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGameFramework.DIContainers
{
    public static class Helpers
    {
        public static void PopulateContainer(IAdvancedDIContainer thisMain) //this is probably the best thing to do.
        {
            if (thisMain.MainContainer != null)
                return;
            thisMain.MainContainer = (IGamePackageResolver)cons;
        }
    }
}