using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace BasicGamingUIXFLibrary.Views
{
    public abstract class BasicSubmitView : ContentView, IUIView
    {
        private readonly IEventAggregator _aggregator;

        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        protected virtual bool UseSmallerButton => false;
        private readonly Button _button;
        protected virtual string ButtonText => "";
        protected virtual string CommandText => nameof(BasicSubmitViewModel.SubmitAsync);
        public BasicSubmitView(IEventAggregator aggregator)
        {
            Button button;
            if (UseSmallerButton == false)
            {
                button = GetGamingButton("", CommandText);
            }
            else
            {
                button = GetSmallerButton("", CommandText);
            }
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            Content = button;
            _button = button;
            _aggregator = aggregator;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            if (ButtonText != "")
            {
                _button.Text = ButtonText;
            }
            else
            {
                var model = (ISubmitText)BindingContext;
                _button.Text = model.Text;
            }
            return this.RefreshBindingsAsync(_aggregator);
        }
    }
}
