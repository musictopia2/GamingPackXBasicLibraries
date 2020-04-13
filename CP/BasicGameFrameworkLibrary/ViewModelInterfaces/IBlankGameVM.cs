using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.ViewModelInterfaces
{
    public interface IBlankGameVM : IMainScreen
    {
        //maybe no need for newgamevisible since that could be a new view model itself.
        //everything needs to derive from this one.
        CommandContainer CommandContainer { get; set; }
    }
}