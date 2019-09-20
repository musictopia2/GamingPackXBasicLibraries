using System;
namespace BasicGameFramework.CommonInterfaces
{
    public interface IPopulateObject<T> where T : IConvertible
    {
        void Populate(T chosen); //so more options.
    }
}