using System;
using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface ISimpleBoardGameProcesses<E, M> where E : struct, Enum
    {
        bool DidChooseColors { get; set; }
        Task ChoseColorAsync(E thisColor);
        Task MakeMoveAsync(M space);
    }
}