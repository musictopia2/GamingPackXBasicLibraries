using System.Reflection;
namespace BasicGameFrameworkLibrary.CommandClasses
{
    /// <summary>
    /// this one should do without canexecute because something else can handle that part easier.
    /// if that is needed, then use something else.
    /// </summary>
    public class BoardCommand : PlainCommand
    {
        
        
        

        public string Name { get; private set; } = ""; //the purpose of this is so it can search and get the proper command.
        public BoardCommand(object model, MethodInfo execute, CommandContainer container, string name) : base(model, execute, canExecuteM: null!, container)
        {
            Name = name;
        }
    }
}
