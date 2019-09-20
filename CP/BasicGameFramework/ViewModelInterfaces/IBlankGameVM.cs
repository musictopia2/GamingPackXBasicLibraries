using BasicGameFramework.CommandClasses;
using System.Threading.Tasks;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface IBlankGameVM
    {
        bool NewGameVisible { get; set; }
        Task ShowGameMessageAsync(string message); //i think its so common it needs to be here
        CommandContainer? CommandContainer { get; set; }
    }
}