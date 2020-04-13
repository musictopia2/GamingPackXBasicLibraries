using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.Attributes
{
    //hopefully this simple.
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(EnumCommandCategory category)
        {
            Category = category;
        }

        public EnumCommandCategory Category { get; }
    }
}