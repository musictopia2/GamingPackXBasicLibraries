using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace BasicGameFramework.CommandClasses
{
    public interface IGameCommand : ICustomCommand
    {
        EnumCommandBusyCategory BusyCategory { get; set; }
    }
}