using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace BasicGamingUIXFLibrary.Views
{
    public class NewRoundView : ContentView, IUIView
    {
        private readonly IEventAggregator _aggregator;

        public NewRoundView(IEventAggregator aggregator)
        {
            Button button = GetGamingButton("Start New Round", nameof(NewRoundViewModel.StartNewRoundAsync));
            Content = button;
            _aggregator = aggregator;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
