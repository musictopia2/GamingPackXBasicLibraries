using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.CommandClasses
{
    public class PlainCommand : IGameCommand
    {
        public event EventHandler? CanExecuteChanged;
        public EnumCommandBusyCategory BusyCategory { get; set; }
        protected CommandContainer CommandContainer { get; set; } //if you have it, use it.
        private readonly Func<object, Task>? _executeMethod;
        protected readonly Func<object, bool> CanExecuteMethod;
        private readonly IErrorHandler _errorHandler;
        private readonly Action<object>? _oldcommand;
        protected virtual void AddCommand()
        {
            CommandContainer.AddCommand(this);
        }
        public PlainCommand(Func<object, Task> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer)
        {
            _executeMethod = action;
            CanExecuteMethod = cans;
            _errorHandler = errors;
            CommandContainer = thisContainer;
            AddCommand();
        }
        public PlainCommand(Action<object> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            _oldcommand = action;
            CanExecuteMethod = cans;
            _errorHandler = errors;
            CommandContainer = thisContainer;
            AddCommand();
        }
        protected virtual void StartExecuting()
        {
            CommandContainer.IsExecuting = true;
            CommandContainer.Processing = true;
        }
        protected virtual void StopExecuting()
        {
            if (CommandContainer.ManuelFinish == false)
                CommandContainer.IsExecuting = false;
            CommandContainer.Processing = false;
        }
        private async void OldExecute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    StartExecuting();

                    _oldcommand!.Invoke(parameter);
                }
                catch (Exception ex)
                {
                    await _errorHandler.HandleErrorAsync(ex);
                }
                finally
                {
                    StopExecuting();

                }
            }
        }
        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    StartExecuting();
                    await _executeMethod!(parameter);
                }
                finally
                {
                    StopExecuting();
                }
            }
        }
        public virtual bool CanExecute(object parameter)
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;

            if (CanExecuteMethod == null == true)
            {
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            }
            return CanExecuteMethod!(parameter); // i think this is how that part is done.
        }
        public void Execute(object parameter)
        {
            if (_oldcommand != null)
            {
                OldExecute(parameter);
                return;
            }
            ExecuteAsync(parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        public void ReportCanExecuteChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs()); // try this
        }
    }
    public class PlainCommand<T> : IGameCommand
    {
        public event EventHandler? CanExecuteChanged;
        public EnumCommandBusyCategory BusyCategory { get; set; }
        protected CommandContainer CommandContainer { get; set; } //if you have it, use it.
        private readonly Func<T, Task>? _executeMethod;
        protected readonly Func<T, bool> CanExecuteMethod;
        private readonly IErrorHandler _errorHandler;
        private readonly Action<T>? _oldcommand;
        protected virtual void AddCommand()
        {
            CommandContainer.AddCommand(this);
        }
        public PlainCommand(Func<T, Task> action, Func<T, bool> cans, IErrorHandler errors, CommandContainer thisContainer) //decided to make it required for all. i think this is best to force good practices
        {
            _executeMethod = action;
            CanExecuteMethod = cans;
            _errorHandler = errors;
            CommandContainer = thisContainer;
            AddCommand();
        }
        public PlainCommand(Action<T> action, Func<T, bool> cans, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            _oldcommand = action;
            CanExecuteMethod = cans;
            _errorHandler = errors;
            CommandContainer = thisContainer;
            AddCommand();
        }
        protected virtual void StartExecuting()
        {
            CommandContainer.IsExecuting = true;
            CommandContainer.Processing = true;
        }
        protected virtual void StopExecuting()
        {
            if (CommandContainer.ManuelFinish == false)
                CommandContainer.IsExecuting = false;
            CommandContainer.Processing = false;
        }
        public virtual bool CanExecute(object parameter)
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;
            if (IsValidParameter(parameter) == false)
                return false;
            T newObj = (T)parameter;
            if (CanExecuteMethod == null == true)
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            return CanExecuteMethod!(newObj); // i think this is how that part is done.
        }
        static bool IsValidParameter(object o)
        {
            if (o != null)
                return o is T;
            var t = typeof(T);
            if (Nullable.GetUnderlyingType(t) != null)
                return true;
            return !t.GetTypeInfo().IsValueType;
        }
        private async void OldExecute(T parameter)
        {
            if (CanExecute(parameter!))
            {
                try
                {
                    StartExecuting();
                    _oldcommand!.Invoke(parameter);
                }
                catch (Exception ex)
                {
                    await _errorHandler.HandleErrorAsync(ex);
                }
                finally
                {
                    StopExecuting();
                }
            }
        }
        public void Execute(object parameter) //we decided to do it this way so for data entry, the same command can be used regardless of generics (?)
        {
            if (IsValidParameter(parameter) == false)
            {
                return; //can't execute because the parameter is not valid this time
            }
            if (_oldcommand != null)
            {
                {
                    OldExecute((T)parameter);
                    return;
                }
            }
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter!))
            {
                try
                {
                    StartExecuting();
                    await _executeMethod!(parameter);
                }
                finally
                {
                    StopExecuting();
                }
            }
        }
        public void ReportCanExecuteChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs()); // try this
        }
    }
}