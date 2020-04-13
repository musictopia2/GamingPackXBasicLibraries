using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.ViewModels
{
    [InstanceGame] //hopefully this pays off.
    public abstract class BasicSubmitViewModel : Screen, IBlankGameVM, IMainScreen, ISubmitText
    {
        public abstract bool CanSubmit { get; } //i think this is the best way to go.

        [Command(EnumCommandCategory.Plain)]
        public abstract Task SubmitAsync();

        public virtual string Text => "Submit"; //since this is default, will use this to start with.

        public BasicSubmitViewModel(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
        }

        public CommandContainer CommandContainer { get; set; }
        //this is intended to be used in cases where you have a simple picker.  you have to submit but its up to overrided versions to decide how to do this.
    }
}