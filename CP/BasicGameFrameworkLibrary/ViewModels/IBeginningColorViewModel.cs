using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.ViewModels
{
    //the purpose of this is so something else can ask for it and not deal with generics.
    public interface IBeginningColorViewModel : IScreen
    {
        string Turn { get; set; }
        string Instructions { get; set; }
    }
}