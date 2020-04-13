using System;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.Attributes
{
    /// <summary>
    /// This attribute if set, will be registerd as singleton.
    /// However, it must be used on the assembly that its scanning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonGameAttribute : Attribute { }
}