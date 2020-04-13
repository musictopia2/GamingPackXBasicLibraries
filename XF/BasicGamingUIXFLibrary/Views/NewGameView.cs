using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace BasicGamingUIXFLibrary.Views
{
    public class NewGameView : ContentView, IUIView
    {
        private readonly Button _button;
        public NewGameView()
        {
            Button button = GetGamingButton("Start New Game", nameof(NewGameViewModel.StartNewGameAsync));
            _button = button;
            Content = button;
        }

        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        //doing manually fixed it this time.  since xamarin forms is more picky.
        Task IUIView.TryActivateAsync()
        {
            NewGameViewModel model = (NewGameViewModel)BindingContext;
            MethodInfo method = model.GetPrivateMethod(nameof(NewGameViewModel.StartNewGameAsync));
            MethodInfo function = model.GetPrivateMethod(nameof(NewGameViewModel.CanStartNewGame));
            ReflectiveCommand command = new ReflectiveCommand(model, method, function);
            _button.Command = command; //do manually this time.
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
