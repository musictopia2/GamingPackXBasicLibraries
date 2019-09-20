using BasicGameFramework.BasicDrawables.Interfaces;
namespace BasicGameFramework.BasicDrawables.Dictionary
{
    public interface IDeckLookUp<D> where D : IDeckObject
    {
        D GetSpecificItem(int deck);
        bool ObjectExist(int deck); //decided to have that here too.
    }
}