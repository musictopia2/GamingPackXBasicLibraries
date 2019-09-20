using BasicGameFramework.CommandClasses;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface IRoundCommand
    {
        PlainCommand NewRoundCommand { get; set; }
    }
}