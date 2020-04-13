using System;
namespace BasicGameFrameworkLibrary.CommonInterfaces
{
    public interface IColorObject<E> where E : Enum
    {
        E GetColor { get; }
    }
}