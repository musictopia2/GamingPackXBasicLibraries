using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.BasicDrawables.Interfaces
{
    public interface IListShuffler<D> : IDeckShuffler<D>, ISimpleList<D>, IEnumerableDeck<D> where D : IDeckObject, new()
    {
        void RelinkObject(int oldDeck, D newObject);
    }
}