using BasicGameFramework.BasicDrawables.Interfaces;
using System.Collections.Generic;
namespace BasicGameFramework.BasicDrawables.Dictionary
{
    public interface IEnumerableDeck<D> : IEnumerable<D> where D : IDeckObject { }
}