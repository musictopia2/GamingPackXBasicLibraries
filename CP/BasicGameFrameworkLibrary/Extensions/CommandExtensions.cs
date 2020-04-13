using BasicGameFrameworkLibrary.CommandClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace BasicGameFrameworkLibrary.Extensions
{
    public static class CommandExtensions
    {
        //this will do without the canexceute version.
        public static CustomBasicList<BoardCommand> GetBoardCommandList(this ISeveralCommands vm)
        {
            CustomBasicList<BoardCommand> output = new CustomBasicList<BoardCommand>();

            Type type = vm.GetType();
            CustomBasicList<MethodInfo> methods = type.GetMethods().ToCustomBasicList(); //decided to just show all methods period.
            //must have no 
            methods.ForEach(x =>
            {
                output.Add(new BoardCommand(vm, x, vm.Command, x.Name));
            });
            BoardCommand board = output.First();
            return output;
        }

        public static bool CanExecuteBasics(this CommandContainer command)
        {
            if (command.IsExecuting == true || command.Processing)
            {
                return false;
            }
            return true;
        }

        private static MethodInfo? GetPrivateMethod(this object payLoad, string name)
        {
            Type type = payLoad.GetType();
            MethodInfo output = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (output != null)
            {
                return output;
            }
            output = type.GetMethod(name);
            if (output != null)
            {
                return output;
            }
            type = type.BaseType;
            output = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            return output;
        }

        private static PropertyInfo? GetPrivateProperty(this object payLoad, string name)
        {
            Type type = payLoad.GetType();
            PropertyInfo output = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (output != null)
            {
                return output;
            }
            output = type.GetProperty(name);
            if (output != null)
            {
                return output;
            }
            type = type.BaseType;
            output = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
            return output;
        }
        public static PlainCommand GetPlainCommand(this ISeveralCommands payLoad, string name)
        {
            MethodInfo? method = payLoad.GetPrivateMethod(name);
            if (method == null)
            {
                Type type = payLoad.GetType();
                throw new BasicBlankException($"Method with the name of {name} was not found  Type was {type.Name}");
            }
            PlainCommand output = new PlainCommand(payLoad, method, canExecuteM: null!, payLoad.Command);
            return output;
        }
        public static PlainCommand GetPlainCommand(this ObservableObject payLoad, CommandContainer commandContainer, string commandName)
        {
            string functionName = $"Can{commandName}";
            functionName = functionName.Replace("Async", "");
            MethodInfo? method = payLoad.GetPrivateMethod(commandName);
            if (method == null)
            {
                Type type = payLoad.GetType();
                throw new BasicBlankException($"Method with the name of {commandName} was not found  Type was {type.Name}");
            }
            MethodInfo? fun = payLoad.GetPrivateMethod(functionName);
            PlainCommand output;
            if (fun != null)
            {
                output = new PlainCommand(payLoad, method, canExecuteM: fun, commandContainer);

            }
            else
            {
                PropertyInfo? pp = payLoad.GetPrivateProperty(functionName);
                output = new PlainCommand(payLoad, method, canExecute: pp, commandContainer);
            }
            return output;
        }
        public static BasicGameCommand GetBasicGameCommand(this ISimpleGame payLoad, string commandName)
        {
            string functionName = $"Can{commandName}";
            functionName = functionName.Replace("Async", "");
            MethodInfo? method = payLoad.GetPrivateMethod(commandName);
            if (method == null)
            {
                Type type = payLoad.GetType();
                throw new BasicBlankException($"Method with the name of {commandName} was not found  Type was {type.Name}");
            }
            MethodInfo? fun = payLoad.GetPrivateMethod(functionName);
            BasicGameCommand output;
            if (fun != null)
            {
                output = new BasicGameCommand(payLoad, method, canExecuteM: fun, payLoad.CommandContainer);

            }
            else
            {
                PropertyInfo? pp = payLoad.GetPrivateProperty(functionName);
                output = new BasicGameCommand(payLoad, method, canExecute: pp, payLoad.CommandContainer);
            }
            return output;
        }
    }
}
