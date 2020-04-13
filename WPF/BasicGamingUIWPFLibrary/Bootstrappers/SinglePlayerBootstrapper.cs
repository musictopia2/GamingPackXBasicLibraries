using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.Messenging;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Bootstrappers
{
    public abstract class SinglePlayerBootstrapper<TViewModel, TView> : BasicGameBootstrapper<TViewModel, TView>
        where TViewModel : IMainGPXShellVM
        where TView : IUIView
    {
        protected override bool UseMultiplayerProcesses => false;
        public SinglePlayerBootstrapper(IStartUp starts, EnumGamePackageMode mode ) : base(starts, mode)
        {
            
        }
    }
}
