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
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using System.Windows;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Shells
{
    public class YahtzeeShellView : MultiplayerBasicShellView, IHandleAsync<WarningEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public YahtzeeShellView(IGameInfo gameData, BasicData basicData, IStartUp start, IEventAggregator aggregator) : base(gameData, basicData, start)
        {
            aggregator.Subscribe(this);
            _aggregator = aggregator;
        }

        protected override Task PopulateUIAsync()
        {
            return Task.CompletedTask;
        }

        async Task IHandleAsync<WarningEventModel>.HandleAsync(WarningEventModel message)
        {
            var exps = MessageBox.Show(this, message.Message, "", MessageBoxButton.YesNo);
            SelectionChosenEventModel results = new SelectionChosenEventModel();
            if (exps == MessageBoxResult.No)
                results.OptionChosen = EnumOptionChosen.No;
            else
                results.OptionChosen = EnumOptionChosen.Yes;
            await _aggregator!.PublishAsync(results);
        }
    }
}
