using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace BasicGamingUIWPFLibrary.Views
{
    public abstract class BasicSubmitView : UserControl, IUIView
    {
        //protected virtual string ButtonText => nameof(BasicSubmitViewModel.Text);

        readonly Button _button;
        protected virtual string CommandText => nameof(BasicSubmitViewModel.SubmitAsync); //this part is okay.
        public BasicSubmitView()
        {
            Button button = GetGamingButton("", CommandText);
            _button = button;
            Content = button;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            ISubmitText model = (ISubmitText)DataContext;
            _button.Content = model.Text;
            return Task.CompletedTask;
        }
    }
}
