using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.Attributes
{
    /// <summary>
    /// this is used for cases where you want to be able to do a list of properties and being able to hook the events for property notify change.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class VMAttribute : Attribute
    {
    }
}