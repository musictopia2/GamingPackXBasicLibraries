using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BaseGPXPagesAndControlsXF.BasePageProcesses.Pages
{
    public abstract class SinglePlayerGamePage<VM> : BasicGamePage<VM>
        where VM : BaseViewModel, IBasicGameVM
    {
        public SinglePlayerGamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        protected override bool UseMultiplayerProcesses => false;
    }
}