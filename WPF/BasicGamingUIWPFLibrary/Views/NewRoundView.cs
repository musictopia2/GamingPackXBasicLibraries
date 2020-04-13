using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace BasicGamingUIWPFLibrary.Views
{
    public class NewRoundView : UserControl, IUIView
    {
        public NewRoundView()
        {
            Button button = GetGamingButton("Start New Round", nameof(NewRoundViewModel.StartNewRoundAsync));
            Content = button;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
