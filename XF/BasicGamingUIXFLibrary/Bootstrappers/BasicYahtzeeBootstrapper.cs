using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Shells;
using BasicGamingUIXFLibrary.Views;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using System.Threading.Tasks;

namespace BasicGamingUIXFLibrary.Bootstrappers
{
    public abstract class BasicYahtzeeBootstrapper<D> : MultiplayerBasicBootstrapper<YahtzeeShellViewModel<D>, YahtzeeShellView>

        where D : SimpleDice, new()
    {
        public BasicYahtzeeBootstrapper(
            IGamePlatform customPlatform,
            IStartUp starts,
            EnumGamePackageMode mode) : base(customPlatform, starts, mode)
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
