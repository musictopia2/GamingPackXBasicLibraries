using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BasicGameFrameworkLibrary.CommandClasses
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
            if (_canExecutep != null)
            {
                return (bool)_canExecutep.GetValue(_model); //properties cannot have parameters obviously.
            }

            if (_canExecuteM != null)
            {
                if (parameter == null)
                    return (bool)_canExecuteM.Invoke(_model, null);
                return (bool)_canExecuteM.Invoke(_model, new object[] { parameter });
            }
            return true;
        }

        public OpenCommand(object model, MethodInfo execute, MethodInfo canExecuteM, CommandContainer container) : base(model, execute, canExecuteM, container)
        {
        }
        public OpenCommand(object model, MethodInfo execute, PropertyInfo? canExecute, CommandContainer container) : base(model, execute, canExecute, container)
        {
        }
    }
}
