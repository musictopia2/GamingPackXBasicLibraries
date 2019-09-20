using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BaseGPXWindowsAndControlsCore.BaseWindows
{
    public abstract class SinglePlayerWindow<VM> : BasicGameWindow<VM>
        where VM : BaseViewModel, IBasicGameVM
    {
        protected override bool UseMultiplayerProcesses => false;
    }
}