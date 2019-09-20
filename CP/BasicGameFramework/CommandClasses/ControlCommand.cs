using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.CommandClasses
{
    public class ControlCommand : IGameCommand
    {
        public EnumCommandBusyCategory BusyCategory { get; set; }
        public event EventHandler? CanExecuteChanged;
        private CommandContainer CommandContainer { get; set; } //if you have it, use it.
        private Func<object, Task>? _executeMethod;
        private readonly IErrorHandler _errorHandler;
        private Action<object>? _oldcommand;
        private IControlVM? _thisProcess;
        public ControlCommand(IControlVM process, Func<object, Task> action, IErrorHandler errors, CommandContainer thisContainer)
        {
            StartActions(process, action);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        public ControlCommand(IControlVM process, Action<object> action, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            StartActions(process, action);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        protected void StartActions(IControlVM process, Action<object> action)
        {
            _thisProcess = process;
            _oldcommand = action;
        }
        protected void StartActions(IControlVM process, Func<object, Task> action)
        {
            _thisProcess = process;
            _executeMethod = action;
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
                    CommandContainer.Processing = true;
                    CommandContainer.IsExecuting = true;
                    await _executeMethod!(parameter);
                }
                finally
                {
                    CommandContainer.Processing = false;
                    if (CommandContainer.ManuelFinish == false)
                        CommandContainer.IsExecuting = false;
                }
            }
        }
        public bool CanExecute(object parameter)
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;
            return _thisProcess!.CanExecute(); //for control, it somehow ignores the parameter
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
    public class ControlCommand<T> : IGameCommand
    {
        public EnumCommandBusyCategory BusyCategory { get; set; }
        public event EventHandler? CanExecuteChanged;
        private CommandContainer CommandContainer { get; set; } //if you have it, use it.
        private Func<T, Task>? _executeMethod;
        private readonly IErrorHandler _errorHandler;
        private Action<T>? _oldcommand;
        private IControlVM? _thisProcess;
        public ControlCommand(IControlVM process, Func<T, Task> action, IErrorHandler errors, CommandContainer thisContainer)
        {
            StartActions(process, action);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            if (_errorHandler == null)
                throw new BasicBlankException("No error handler");
            if (CommandContainer == null)
                throw new BasicBlankException("No Container");
            CommandContainer.AddCommand(this);
        }
        public ControlCommand(IControlVM process, Action<T> action, IErrorHandler errors, CommandContainer thisContainer) //i think its best to do it this way because i really don't want to have to do several waitblanks when its not really needed
        {
            StartActions(process, action);
            _errorHandler = errors;
            CommandContainer = thisContainer;
            CommandContainer.AddCommand(this);
        }
        protected void StartActions(IControlVM process, Action<T> action)
        {
            _thisProcess = process;
            _oldcommand = action;
        }
        protected void StartActions(IControlVM process, Func<T, Task> action)
        {
            _thisProcess = process;
            _executeMethod = action;
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
            return _thisProcess!.CanExecute(); //for control, it somehow ignores the parameter
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