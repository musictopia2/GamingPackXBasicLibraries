using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.Dice
{
    public interface IStandardRollProcesses
    {
        //this was created so view models don't need interfaces.
        Task RollDiceAsync();
        Task SelectUnSelectDiceAsync(int id);
    }
}