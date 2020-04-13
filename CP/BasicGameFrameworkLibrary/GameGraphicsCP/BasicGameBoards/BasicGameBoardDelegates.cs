using System;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards
{
    public static class BasicGameBoardDelegates
    {
        public static Func<Task>? AfterPaintAsync { get; set; }
    }
}
