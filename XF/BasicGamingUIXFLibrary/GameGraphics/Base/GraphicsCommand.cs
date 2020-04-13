using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.Helpers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.GameGraphics.Base
{
    public abstract class GraphicsCommand : ContentView
    {
        //has to be here instead of the other part.  because its needed for the clicks.
        protected SKCanvasView ThisDraw;
        //public TestOptions TestInfo;
        public GraphicsCommand()
        {
            ThisDraw = new SKCanvasView();
            //TestInfo = Resolve<TestOptions>();
            //since you do this before the overrided version, should be fine.
            ThisDraw.EnableTouchEvents = true;
            ThisDraw.Touch += ThisDraw_Touch;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            SetUpContent();
        }

        protected virtual void SetUpContent()
        {
            Content = ThisDraw;
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: "Command", returnType: typeof(ICommand), declaringType: typeof(GraphicsCommand), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandPropertyChanged);
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: "CommandParameter", returnType: typeof(object), declaringType: typeof(GraphicsCommand), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandParameterPropertyChanged);
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        private async void ThisDraw_Touch(object sender, SKTouchEventArgs e)
        {
            FirstTouch();
            if (Command == null)
                return;
            //maybe no need for the checking for canexecute because something else checks it anyways.
            await Task.Delay(5);


            //do
            //{
            //    if (SharedUIFunctions.BoardProcessing == false)
            //    {
            //        break;
            //    }
            //    await Task.Delay(5);
            //} while (true);
            if (Command.CanExecute(CommandParameter) == true)
            {
                await TouchProcessAsync(e.Location);
            }
            //TouchProcess(e.Location);
            //if (Command.CanExecute(CommandParameter) == true)
            //    TouchProcess(e.Location);
        }
        protected virtual void FirstTouch() { } //used so i can see if the overrided version handled it for help in testing.
        private async Task TouchProcessAsync(SKPoint thisPoint)
        {
            await Task.Delay(20);



            //BeforeProcessCommand(thisPoint); //looks like needs both for now.  could decide later to do just beforeprocessclick.
            //not sure yet.
            BeforeProcessClick(Command, CommandParameter, thisPoint);

            if (Command is ControlCommand control)
            {
                await control.ExecuteAsync(CommandParameter);
                return;
            }

            Command.Execute(CommandParameter);
            
            
        }
        //protected virtual void BeforeProcessCommand(SKPoint point) { }

        //hopefully has enough information.  if something else is needed, rethinking will be required.
        protected virtual void BeforeProcessClick(ICommand thisCommand, object thisParameter, SKPoint clickPoint) { } //most of the time, do nothing.



    }
}