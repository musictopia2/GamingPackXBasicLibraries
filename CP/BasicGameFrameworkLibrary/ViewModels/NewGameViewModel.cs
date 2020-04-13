using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.ViewModels
{
    //i don't think i should inherit from this class.
    public sealed class NewGameViewModel : Screen, INewGameVM, IBlankGameVM
    {
        private readonly IEventAggregator _aggregator;
        private readonly BasicData _basicData;

        public CommandContainer CommandContainer { get; set; }

        //not sure yet if we need the resolver.
        //public IGamePackageResolver? MainContainer { get; set; }

        public NewGameViewModel(CommandContainer command, IEventAggregator aggregator, BasicData basicData)
        {
            CommandContainer = command;
            _aggregator = aggregator;
            _basicData = basicData;
        }
        //try to do as method this time.  so hopefully can work properly for doing manually for xamarin forms.
        public bool CanStartNewGame() => _basicData.MultiPlayer == false || _basicData.Client == false;

        //IEventAggregator IBlankGameVM.Aggregator
        //{
        //    get => _aggregator;
        //}

        [Command(EnumCommandCategory.Old)] //try old.
        public Task StartNewGameAsync()
        {
            return _aggregator.PublishAsync(new NewGameEventModel()); //this does not care what happens with the new game.
        }
    }
}
