using System;
namespace BasicGameFramework.CommonInterfaces
{
    public interface IColorObject<E> where E : Enum
    {
        E GetColor { get; }
    }
}