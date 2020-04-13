using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers
{
    public static class CheckersChessDelegates
    {
        public static Func<bool>? CanMove { get; set; }
        public static Func<int, Task>? MakeMoveAsync { get; set; }
    }
}