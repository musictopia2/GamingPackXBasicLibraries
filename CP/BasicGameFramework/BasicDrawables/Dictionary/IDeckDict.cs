using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.BasicDrawables.Dictionary
{
    public interface IDeckDict<D> : ICustomBasicList<D>, IDeckLookUp<D> where D : IDeckObject
    {
        D RemoveObjectByDeck(int deck);
    }
}
