using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses
{
    internal static class GlobalHelpers
    {
        public static Func<Task>? LoadGameScreenAsync { get; set; }
    }
}
