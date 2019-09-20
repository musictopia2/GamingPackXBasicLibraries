using System;
namespace BasicGameFramework.CommonInterfaces
{
    public interface IEnumPiece<E> : ISelectableObject, IEnabledObject where E : Enum
    {
        E EnumValue { get; set; }
    }
}
