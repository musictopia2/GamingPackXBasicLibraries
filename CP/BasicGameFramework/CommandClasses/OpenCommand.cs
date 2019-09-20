using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.CommandClasses
{
    public class OpenCommand : PlainCommand
    {
        protected override void AddCommand()
        {
            CommandContainer.AddOpen(this);
        }
        protected override void StartExecuting()
        {
            CommandContainer.OpenBusy = true;
        }
        protected override void StopExecuting() { }
        public override bool CanExecute(object parameter)
        {
            if (CommandContainer.OpenBusy == true)
                return false;
            if (CanExecuteMethod == null == true)
            {
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            }
            return CanExecuteMethod!(parameter);
        }
        public OpenCommand(Func<object, Task> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer) : base(action, cans, errors, thisContainer) { }
        public OpenCommand(Action<object> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer) : base(action, cans, errors, thisContainer) { }
    }
    public class OpenCommand<T> : PlainCommand<T>
    {
        protected override void AddCommand()
        {
            CommandContainer.AddOpen(this);
        }
        protected override void StartExecuting()
        {
            CommandContainer.OpenBusy = true;
        }
        protected override void StopExecuting() { }
        public override bool CanExecute(object parameter)
        {
            if (CommandContainer.OpenBusy == true)
                return false;
            if (CanExecuteMethod == null == true)
            {
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            }
            return CanExecuteMethod!((T)parameter);
        }
        public OpenCommand(Func<T, Task> action, Func<T, bool> cans, IErrorHandler errors, CommandContainer thisContainer) : base(action, cans, errors, thisContainer) { }
        public OpenCommand(Action<T> action, Func<T, bool> _Cans, IErrorHandler errors, CommandContainer thisContainer) : base(action, _Cans, errors, thisContainer) { }
    }
}