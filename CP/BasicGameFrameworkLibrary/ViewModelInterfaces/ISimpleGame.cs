using BasicGameFrameworkLibrary.CommandClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.ViewModelInterfaces
{
    public interface ISimpleGame : IBlankGameVM, IEnableAlways, IBasicEnableProcess
    {
    }
}
