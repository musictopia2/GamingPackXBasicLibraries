using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using CommonBasicStandardLibraries.MVVMFramework.EventArgClasses;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Input;
using uu = CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.CommandClasses
{
    public class BasicGameCommand : IGameCommand
    {
        protected readonly PropertyInfo? _canExecutep;
        private readonly MethodInfo _execute;
        protected readonly IBasicEnableProcess _model;
        protected readonly MethodInfo? _canExecuteM;

        private bool _isAsync;
        private readonly string _functionName = "";

        private bool _hasParameters;

        public EnumCommandBusyCategory BusyCategory { get; set; }
        protected CommandContainer CommandContainer { get; set; }
        object ICustomCommand.Context => _model;

        protected virtual void AddCommand()
        {
            CommandContainer.AddCommand(this);
        }

        public BasicGameCommand(IBasicEnableProcess model, MethodInfo execute, MethodInfo canExecuteM, CommandContainer container)
        {
            CommandContainer = container;
            _model = model;
            _execute = execute;
            _canExecuteM = canExecuteM;
            if (_canExecuteM != null)
            {
                _functionName = canExecuteM.Name;
            }
            HookUpNotifiers();
        }
        public BasicGameCommand(IBasicEnableProcess model, MethodInfo execute, PropertyInfo? canExecute, CommandContainer container)
        {
            CommandContainer = container;
            _model = model;
            _execute = execute;
            _canExecutep = canExecute;
            if (_canExecutep != null)
            {
                _functionName = _canExecutep.Name;
            }
            HookUpNotifiers();
        }

        

        private void HookUpNotifiers()
        {
            _isAsync = _execute.ReturnType.Name == "Task";
            var count = _execute.GetParameters().Count();
            if (count > 1)
            {
                throw new BasicBlankException($"Method {_execute.Name} cannot have more than one parameter.  If more than one is needed, lots of rethinking is required");
            }
            AddCommand();
            _hasParameters = count == 1;
            if (_canExecuteM == null && _canExecutep == null)
                return; //no need to notify because the resulting part is not even there.
            if (_model is INotifyCanExecuteChanged notifier)
                notifier.CanExecuteChanged += Notifier_CanExecuteChanged;
        }

        private void Notifier_CanExecuteChanged(object sender, CanExecuteChangedEventArgs e)
        {
            if (_functionName == "")
                throw new BasicBlankException("No canexecute function was found.  Should not have raised this.  Rethink");

            if (e.Name == _functionName)
                ReportCanExecuteChange();
        }

        public void ReportCanExecuteChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        protected virtual void StartExecuting()
        {
            CommandContainer.StartExecuting();
        }
        protected virtual void StopExecuting()
        {
            CommandContainer.StopExecuting();
        }

        //if everything works, then no generics are needed anymore.


        public event EventHandler CanExecuteChanged = delegate { };
        public virtual bool CanExecute(object parameter)
        {
            //can have extra things that runj before even running this (but not yet).
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
                return false;
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
                return false;
            if (_model!.CanEnableBasics() == false)
                return false;
            if (_canExecutep != null)
            {
                return (bool)_canExecutep.GetValue(_model); //properties cannot have parameters obviously.
            }

            if (_canExecuteM != null)
            {
                if (parameter == null || _canExecuteM.GetParameters().Count() == 0)
                    return (bool)_canExecuteM.Invoke(_model, null);
                return (bool)_canExecuteM.Invoke(_model, new object[] { parameter });
            }

            return true;
        }


        public async void Execute(object parameter)
        {


            if (CanExecute(parameter) == false)
            {
                return;
            }
            StartExecuting();
            if (_isAsync == false)

                if (_hasParameters)
                {
                    _execute.Invoke(_model, new object[] { parameter });
                }
                else
                {
                    _execute.Invoke(_model, null);
                }


            else

                await uu.Execute.OnUIThreadAsync(async () =>
                {
                    Task task;
                    if (_hasParameters)
                    {
                        task = (Task)_execute.Invoke(_model, new object[] { parameter });
                    }
                    else
                    {
                        task = (Task)_execute.Invoke(_model, null);
                    }

                    await task;
                });
            StopExecuting();
        }

        
    }
}
