using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses
{
    public interface ILoadClientGame
    {
        Task LoadGameAsync(string payLoad);
    }
}
