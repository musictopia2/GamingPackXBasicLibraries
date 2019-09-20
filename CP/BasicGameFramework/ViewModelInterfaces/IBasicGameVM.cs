using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface IBasicGameVM : IAdvancedDIContainer, IErrorHandler, IBlankGameVM
    {
        PlainCommand? NewGameCommand { get; set; } //has to break some things now.
        void Init(); //sometimes you need to do this.
    }
}