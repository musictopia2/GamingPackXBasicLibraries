using CommonBasicStandardLibraries.CollectionClasses;
using System;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    /// <summary>
    /// This generates a list of enums that are possible items to choose from
    /// </summary>
    /// <typeparam name="E">The choices.  For example, if suits, then will be Clubs, Diamonds, Spades, And Hearts</typeparam>
    public interface IEnumListClass<E> where E : Enum
    {
        CustomBasicList<E> GetEnumList(); //needs to be a function.
    }
}