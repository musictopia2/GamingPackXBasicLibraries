using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using uu = CommonBasicStandardLibraries.MVVMFramework.UIHelpers;

//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.CommandClasses
{
    public class OutOfTurnCommand : IGameCommand
    {
        //private readonly PropertyInfo? _canExecutep;
        private readonly MethodInfo _execute;
        private readonly IEnableAlways _model; //this must implement ienablealways.  otherwise, can't use.
        //private readonly MethodInfo? _canExecuteM;

        private bool _isAsync;
        object ICustomCommand.Context => _model;
        //private readonly string _functionName = "";

        //not this time.

        private bool _hasParameters;

        public EnumCommandBusyCategory BusyCategory { get; set; }
        private CommandContainer CommandContainer { get; set; }

        //this is slightly different.

        public OutOfTurnCommand(IEnableAlways model, MethodInfo execute, CommandContainer container)
        {
            CommandContainer = container;
            _model = model;
            _execute = execute;

            HookUpNotifiers();
        }


        private void HookUpNotifiers()
        {
            BusyCategory = EnumCommandBusyCategory.Limited;
            CommandContainer.AddCommand(this);
            _isAsync = _execute.ReturnType.Name == "Task";
            var count = _execute.GetParameters().Count();
            if (count > 1)
            {
                throw new BasicBlankException($"Method {_execute.Name} cannot have more than one parameter.  If more than one is needed, lots of rethinking is required");
            }
            _hasParameters = count == 1;

            //iffy.

            //if (_canExecuteM == null && _canExecutep == null)
            //    return; //no need to notify because the resulting part is not even there.
            //if (_model is INotifyCanExecuteChanged notifier)
            //    notifier.CanExecuteChanged += Notifier_CanExecuteChanged;
        }

        //private void Notifier_CanExecuteChanged(object sender, CanExecuteChangedEventArgs e)
        //{
        //    if (_functionName == "")
        //        throw new BasicBlankException("No canexecute function was found.  Should not have raised this.  Rethink");

        //    if (e.Name == _functionName)
        //        ReportCanExecuteChange();
        //}

        public void ReportCanExecuteChange()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        protected virtual void StartExecuting()
        {
            CommandContainer.IsExecuting = true;
            CommandContainer.Processing = true;
        }
        protected virtual void StopExecuting()
        {
            if (CommandContainer.ManuelFinish == false)
            {
                CommandContainer.IsExecuting = false;
            }
            CommandContainer.Processing = false;
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
            return _model.CanEnableAlways();
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
