using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.BasicDrawables.Interfaces;
//i think this is the most common things i like to do
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public class PlayerTrick<S, T> : PlayerSingleHand<T>, IPlayerTrick<S, T>
        where S : Enum
        where T : ITrickCard<S>, new()
    {
        private int _TricksWon;

        public int TricksWon
        {
            get { return _TricksWon; }
            set
            {
                if (SetProperty(ref _TricksWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}
