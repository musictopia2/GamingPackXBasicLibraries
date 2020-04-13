using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.MiscProcesses
{
    /// <summary>
    /// the purpose of this class is to allow cases where a gameboard can have several actions.  this handles processing those actions.
    /// </summary>
    public abstract class BasicBoardActionProcesses : ISeveralCommands
    {
        private CustomBasicList<BoardCommand> _boardList;
        public BoardCommand GetCommand(string method)
        {
            var output = _boardList.SingleOrDefault(x => x.Name == method);
            if (output == null)
            {
                throw new BasicBlankException($"No method was found with the name of {method}.  Rethink");
            }
            return output;
        }

        public BasicBoardActionProcesses(CommandContainer command)
        {
            Command = command;
            _boardList = this.GetBoardCommandList();
        }

        public CommandContainer Command { get; set; }
    }
}
