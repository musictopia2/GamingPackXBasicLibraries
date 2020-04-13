using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.ViewModelInterfaces
{
    public interface INewRoundVM : IScreen
    {
        bool CanStartNewRound { get; }
        Task StartNewRoundAsync();

    }
}