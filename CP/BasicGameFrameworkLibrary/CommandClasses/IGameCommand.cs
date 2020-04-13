using CommonBasicStandardLibraries.MVVMFramework.Commands;

namespace BasicGameFrameworkLibrary.CommandClasses
{
    public interface IGameCommand : ICustomCommand
    {
        EnumCommandBusyCategory BusyCategory { get; set; }
    }
}