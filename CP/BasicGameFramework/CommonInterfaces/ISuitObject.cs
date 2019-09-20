using System;
namespace BasicGameFramework.CommonInterfaces
{
    public interface ISuitObject<E> where E : Enum
    {
        E GetSuit { get; }
    }
}