using BasicGameFramework.CommandClasses;
namespace BasicGameFramework.ViewModelInterfaces
{
    public static class BasicViewModelHelpers
    {
        public static void SetUpBasicViewModel(ISinglePlayerVM thisMod)
        {
            thisMod.CommandContainer = thisMod.MainContainer!.Resolve<CommandContainer>();
            thisMod.NewGameCommand = new PlainCommand(async Items =>
            {
                await thisMod.StartNewGameAsync();
            }, Items => thisMod.NewGameVisible, thisMod, thisMod.CommandContainer);
        }
    }
}