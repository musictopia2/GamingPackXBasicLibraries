using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses
{
    public interface IClientUpdateGame
    {
        //looks like does not matter whether its from restore or not (?)
        Task UpdateGameAsync(string payload);
    }
}