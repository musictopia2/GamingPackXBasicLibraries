using System.Reflection;
namespace BasicGameFrameworkLibrary.CommandClasses
{
    public class LimitedGameCommand : BasicGameCommand
    {
        public LimitedGameCommand(IBasicEnableProcess model, MethodInfo execute, MethodInfo canExecuteM, CommandContainer container) : base(model, execute, canExecuteM, container)
        {
            BusyCategory = EnumCommandBusyCategory.Limited;
        }

        public LimitedGameCommand(IBasicEnableProcess model, MethodInfo execute, PropertyInfo? canExecute, CommandContainer container) : base(model, execute, canExecute, container)
        {
            BusyCategory = EnumCommandBusyCategory.Limited;
        }
    }
}
