using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.CommandClasses
{
    public class BasicGameCommand : IGameCommand
    {
        public event EventHandler? CanExecuteChanged;
        private CommandContainer CommandContainer { get; set; } //if you have it, use it.
        public EnumCommandBusyCategory BusyCategory { get; set; }
        private Func<object, Task>? _executeMethod;
        private Func<object, bool>? _canExecuteMethod;
        private readonly IErrorHandler _errorHandler;
        private Action<object>? _oldcommand;
        private IBasicEnableProcess? _thisProcess;
        public BasicGameCommand(IBasicEnableProcess process, Func<object, Task> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer)
        {
            StartActions(process, action, cans);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        public BasicGameCommand(IBasicEnableProcess process, Action<object> action, Func<object, bool> cans, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            StartActions(process, action, cans);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        protected void StartActions(IBasicEnableProcess process, Action<object> action, Func<object, bool> cans)
        {
            _thisProcess = process;
            _oldcommand = action;
            _canExecuteMethod = cans;
        }
        protected void StartActions(IBasicEnableProcess process, Func<object, Task> action, Func<object, bool> cans)
        {
            _thisProcess = process;
            _executeMethod = action;
            _canExecuteMethod = cans;
        }
        private async void OldExecute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    CommandContainer.IsExecuting = true;
                    CommandContainer.Processing = true;
                    _oldcommand!.Invoke(parameter);
                }
                catch (Exception ex)
                {
                    await _errorHandler.HandleErrorAsync(ex);
                }
                finally
                {
                    if (CommandContainer.ManuelFinish == false)
                        CommandContainer.IsExecuting = false;
                    CommandContainer.Processing = false;
                }
            }
        }
        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    CommandContainer.IsExecuting = true;
                    CommandContainer.Processing = true;
                    if (_executeMethod == null)
                        throw new BasicBlankException("Execute Not Set");
                    await _executeMethod(parameter);
                }
                finally
                {
                    if (CommandContainer.ManuelFinish == false)
                        CommandContainer.IsExecuting = false;
                    CommandContainer.Processing = false;
                }
            }
        }
        public bool CanExecute(object parameter)
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;
            if (_thisProcess!.CanEnableBasics() == false)
                return false;
            if (_canExecuteMethod == null)
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            return _canExecuteMethod(parameter); // i think this is how that part is done.
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
    public class BasicGameCommand<T> : IGameCommand
    {
        public event EventHandler? CanExecuteChanged;
        private CommandContainer CommandContainer { get; set; } //if you have it, use it.
        public EnumCommandBusyCategory BusyCategory { get; set; }
        private Func<T, Task>? _executeMethod;
        private Func<T, bool>? _canExecuteMethod;
        private readonly IErrorHandler _errorHandler;
        private Action<T>? _oldcommand;
        private IBasicEnableProcess? _thisProcess;
        public BasicGameCommand(IBasicEnableProcess process, Func<T, Task> action, Func<T, bool> cans, IErrorHandler errors, CommandContainer thisContainer)
        {
            StartActions(process, action, cans);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        public BasicGameCommand(IBasicEnableProcess process, Action<T> action, Func<T, bool> cans, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            StartActions(process, action, cans);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        protected void StartActions(IBasicEnableProcess process, Action<T> action, Func<T, bool> cans)
        {
            _thisProcess = process;
            _oldcommand = action;
            _canExecuteMethod = cans;
        }
        protected void StartActions(IBasicEnableProcess process, Func<T, Task> action, Func<T, bool> cans)
        {
            _thisProcess = process;
            _executeMethod = action;
            _canExecuteMethod = cans;
        }
        private async void OldExecute(T parameter)
        {
            if (CanExecute(parameter!))
            {
                try
                {
                    CommandContainer.IsExecuting = true;
                    CommandContainer.Processing = true;
                    _oldcommand!.Invoke(parameter);
                }
                catch (Exception ex)
                {
                    await _errorHandler.HandleErrorAsync(ex);
                }
                finally
                {
                    if (CommandContainer.ManuelFinish == false)
                        CommandContainer.IsExecuting = false;
                    CommandContainer.Processing = false;
                }
            }
        }
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter!))
            {
                try
                {
                    CommandContainer.IsExecuting = true;
                    CommandContainer.Processing = true;
                    await _executeMethod!(parameter);
                }
                finally
                {
                    if (CommandContainer.ManuelFinish == false)
                        CommandContainer.IsExecuting = false;
                    CommandContainer.Processing = false;
                }
            }
        }
        public bool CanExecute(object parameter)
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;
            if (_thisProcess!.CanEnableBasics() == false)
                return false;
            if (_canExecuteMethod == null)
                return true;// if nothing is sent, implies it can be executed.  otherwise, needs logic to decide whether it can be executed. never makes sense to have a command that can never be executed
            return _canExecuteMethod((T)parameter); // i think this is how that part is done.
        }
        public void Execute(object parameter)
        {
            if (_oldcommand != null)
            {
                OldExecute((T)parameter);
                return;
            }
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        public void ReportCanExecuteChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs()); // try this
        }
    }
}