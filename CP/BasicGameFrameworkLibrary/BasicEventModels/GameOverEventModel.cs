using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.BasicEventModels
{
    public class GameOverEventModel
    {
        //anybody who handles the last part will do this.
        //if i decide that extra information needs to be here, can add as well.

        public string Message { get; set; } = ""; //so if there is a message, that will show up as well.

    }
}
