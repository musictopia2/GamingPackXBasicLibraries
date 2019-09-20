using BasicGameFramework.MiscProcesses;
using System.Collections.Generic;
namespace BasicGameFramework.GameBoardCollections
{
    public interface IBoardCollection<C> : IEnumerable<C> where C : class, IBasicSpace, new()
    {
        C this[Vector thisV] { get; }
        C this[int row, int column] { get; }
        int GetTotalColumns();
        int GetTotalRows();
    }
}