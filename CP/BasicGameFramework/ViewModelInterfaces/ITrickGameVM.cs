using System;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface ITrickGameVM<SU>
        where SU : Enum
    {
        SU TrumpSuit { get; set; }
    }
}