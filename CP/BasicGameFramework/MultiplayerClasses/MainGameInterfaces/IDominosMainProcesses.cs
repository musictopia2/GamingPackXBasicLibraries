using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;

namespace BasicGameFramework.MultiplayerClasses.MainGameInterfaces
{
    public interface IDominosMainProcesses<D, P> : IBasicGameProcesses<P>
        where D: IDominoInfo, new()
        where P: class, IPlayerSingleHand<D>, new()
    {
        Task DrawDominoAsync(D thisDomino);
    }
}