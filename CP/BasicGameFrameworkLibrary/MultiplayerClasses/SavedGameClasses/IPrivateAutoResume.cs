using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses
{
    /// <summary>
    /// this will do the processes for doing private autoresumes.
    /// </summary>
    public interface IPrivateAutoResume
    {
        /// <summary>
        /// if this returns blank, then it means no private save.  decided to do it this way.
        /// that worked well for autoresume
        /// </summary>
        /// <param name="save"></param>
        /// <returns></returns>
        string GetPrivateSaveAsync(SimpleSave save);

        Task DeletePrivateAsync(SimpleSave save);

        Task SavePrivateAsync(SimpleSave save, string data);


        //for now, this is possible future.  we need to really move on now.



        

    }
}
