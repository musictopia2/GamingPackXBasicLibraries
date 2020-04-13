﻿using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
namespace BasicGamingUIWPFLibrary.Bootstrappers
{
    public abstract class MultiplayerBasicBootstrapper<TViewModel, TView> : BasicGameBootstrapper<TViewModel, TView>
     where TViewModel : IMainGPXShellVM
        where TView : IUIView
    {
        public MultiplayerBasicBootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        protected override void MiscRegisterFirst()
        {
            OurContainer!.RegisterType<NewRoundViewModel>(false);
            OurContainer.RegisterType<RestoreViewModel>(false);
        }
        protected override bool UseMultiplayerProcesses => true;

    }
}
