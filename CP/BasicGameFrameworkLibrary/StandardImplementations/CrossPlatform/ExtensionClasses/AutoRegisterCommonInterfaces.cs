using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
//using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
//using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Linq;
using System.Reflection;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses
{
    public static class AutoRegisterCommonInterfaces
    {




        //something else has to register network classes because its unknown at this point.


        public static void RegisterNonSavedClasses<V>(this IGamePackageRegister thisContainer)
        {
            //thisContainer.RegisterSingleton<ListContainer, ListContainer>(); //maybe try it here now.
            thisContainer.RegisterSingleton<IPlayOrder, PlayOrderClass>();
            Assembly thisAssembly = Assembly.GetAssembly(typeof(V))!;
            CustomBasicList<Type> thisList = thisAssembly.GetTypes().Where(items => items.HasAttribute<SingletonGameAttribute>()).ToCustomBasicList();
            thisList.ForEach(items =>
            {
                thisContainer.RegisterSingleton(items);
            });

            thisList = thisAssembly.GetTypes().Where(items => items.HasAttribute<InstanceGameAttribute>()).ToCustomBasicList();
            thisList.ForEach(items =>
            {
                thisContainer.RegisterInstanceType(items);
            });

        }



        public static void RegisterCommonMiscCards<V, D>(this IGamePackageDIContainer thisContainer, string tagUsed, bool registerCommonProportions = true)
             where D : IDeckObject, new()
        {
            thisContainer.RegisterNonSavedClasses<V>();
            thisContainer.RegisterType<DeckObservablePile<D>>(true);
            if (registerCommonProportions)
                thisContainer.RegisterSingleton<IProportionImage, StandardProportion>(tagUsed);
        }
        public static void RegisterCommonRegularCards<V, R>(this IGamePackageDIContainer thisContainer, bool aceLow = true, bool registerCommonProportions = true, bool customDeck = false)
            where R : IRegularCard, new()
        {
            thisContainer.RegisterNonSavedClasses<V>();
            
            thisContainer.RegisterType<DeckObservablePile<R>>(true); //i think
            thisContainer.RegisterType<RegularCardsBasicShuffler<R>>(true); //if i needed a custom shuffler, rethink.
            if (registerCommonProportions == true)
                thisContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed); //so if not using common ones, then has to register that yourself.
            bool rets = thisContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory thisCat = thisContainer.Resolve<ISortCategory>();
                SortSimpleCards<R> thisSort = new SortSimpleCards<R>();
                thisSort.SuitForSorting = thisCat.SortCategory;
                thisContainer.RegisterSingleton(thisSort); //if we have a custom one, will already be picked up.
            }
            if (customDeck == false)
            {
                thisContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
                if (aceLow == true)
                    thisContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //i think for now, if custom deck, then you have to manually register both.
            }
        }
    }
}