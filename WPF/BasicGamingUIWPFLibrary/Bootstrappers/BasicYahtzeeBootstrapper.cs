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
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGamingUIWPFLibrary.Shells;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using BasicGamingUIWPFLibrary.Views;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Bootstrappers
{
    public abstract class BasicYahtzeeBootstrapper<D> : MultiplayerBasicBootstrapper<YahtzeeShellViewModel<D>, YahtzeeShellView>
      
        where D : SimpleDice, new()
    {
        public BasicYahtzeeBootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
            
        }
        protected override bool NeedExtraLocations => false;
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterType<BasicGameLoader<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>>();
            
            OurContainer.RegisterType<StandardRollProcesses<D, YahtzeePlayerItem<D>>>();
            RegisterDiceProportions();
            OurContainer.RegisterSingleton<IGenerateDice<int>, D>();
            OurContainer.RegisterType<YahtzeeScoresheetViewModel<D>>(false);
            OurContainer.RegisterType<YahtzeeMainViewModel<D>>(false);
            OurContainer.RegisterType<YahtzeeVMData<D>>();
            OurContainer.RegisterType<BasicYahtzeeGame<D>>();
            OurContainer.RegisterType<YahtzeeSaveInfo<D>>();
            OurContainer.RegisterType<ScoreLogic>();
            OurContainer.RegisterType<YahtzeeGameContainer<D>>();
            OurContainer.RegisterType<ScoreContainer>();
            OurContainer.RegisterType<YahtzeeMove<D>>();
            OurContainer.RegisterType<YahtzeeEndRoundLogic<D>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<YahtzeePlayerItem<D>>>(true);

            ViewModelViewLinker link = new ViewModelViewLinker()
            {
                ViewModelType = typeof(YahtzeeMainViewModel<D>),
                ViewType = typeof(YahtzeeMainView<D>)
                //ViewModelType = typeof(BeginningChooseColorViewModel<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>),
                //ViewType = typeof(BeginningChooseColorView<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>, EnumColorChoice>)
            };
            ViewLocator.ManuelVMList.Add(link);

            RegisterNonSavedClasses();
            //hopefully this works.
            //i do have hints from board games if needed.
            //i have hints but don't remember from where (?)


            return Task.CompletedTask;
        }

        protected abstract void RegisterNonSavedClasses();
        protected abstract void RegisterDiceProportions();

    }
}
