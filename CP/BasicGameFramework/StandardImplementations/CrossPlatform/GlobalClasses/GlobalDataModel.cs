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
using static BasicGameFramework.NetworkingClasses.Misc.GlobalStaticClasses;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses
{
    public class GlobalDataModel
    {
        public EnumServerMode ServerMode { get; set; } = EnumServerMode.AzureHosting; //default to azure hosting.
        //its possible that if server mode is mobileserver, then this will need to start listening too (only if choosing).
        //i am guessing that whoever uses mobile server is hosting (routes to them).
        //does not matter if you join or not.
        //local means you can connecting to host ip address.
        //since there are methods being used, then it can do extra things (hopefully everything will somehow work).
        //can test on desktop.

        //if a separate app is created, then no problem.  both will use local and choose proper ip address to connect to.
        public string AzureEndPointAddress { get; set; } = MainAzureHostAddress; //this is default if nothing else is entered.
        public string HostIPAddress { get; set; } = ""; //if entered, then you can connect to another computer directly.
        public string MainNickName { get; set; } = "";
        public string SecondaryNickName { get; set; } = "";
        //getting current nick name is the view models responsibility.

    }
}
