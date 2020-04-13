using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;

namespace BasicGamingUIXFLibrary.Bootstrappers
{
    public abstract class SinglePlayerBootstrapper<TViewModel, TView> : BasicGameBootstrapper<TViewModel, TView>
        where TViewModel : IMainGPXShellVM
        where TView : IUIView
    {
        public SinglePlayerBootstrapper(
            IGamePlatform customPlatform,
            IStartUp starts,
            EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override bool UseMultiplayerProcesses => false;

    }
}