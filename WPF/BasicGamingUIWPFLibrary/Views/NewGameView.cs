using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
//i think this is the most common things i like to do


//was forced to use views.  otherwise, would not pick it up.
namespace BasicGamingUIWPFLibrary.Views
{
    public class NewGameView : UserControl, IUIView
    {
        public NewGameView()
        {
            Button button = GetGamingButton("Start New Game", nameof(NewGameViewModel.StartNewGameAsync));
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
