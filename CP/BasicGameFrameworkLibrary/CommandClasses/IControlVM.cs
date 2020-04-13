using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.CommandClasses
{
    //this could be iffy on how important this part is (?)
    //looks like it may still be important.  we may risk making major changes
    //while i am working on minesweeper.
    //since its the beginning, won't be afraid of making more breaking changes.
    public interface IControlObservable
    {
        bool CanExecute();
        void ReportCanExecuteChange();
        EnumCommandBusyCategory BusyCategory { get; set; }
    }
}