using static BasicGameFrameworkLibrary.NetworkingClasses.Misc.GlobalStaticClasses;

namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses
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

        //will have azure when i am ready to start multiplayer processes

        public string AzureEndPointAddress { get; set; } = MainAzureHostAddress; //this is default if nothing else is entered.
        public string HostIPAddress { get; set; } = ""; //if entered, then you can connect to another computer directly.
        public string MainNickName { get; set; } = "";
        public string SecondaryNickName { get; set; } = "";
        //getting current nick name is the view models responsibility.

    }
}
