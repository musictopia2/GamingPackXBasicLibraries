namespace BasicGameFrameworkLibrary.BasicGameDataClasses
{
    public enum EnumPlayOptions
    {
        HumanLocal = 1,
        ComputerLocal,
        ComputerExtra,
        Solitaire
    }
    public enum EnumPlayerChoices
    {
        None, HumanOnly, ComputerOnly, Either, Solitaire
    }
    public enum EnumPlayerType
    {
        SingleOnly, NetworkOnly, SingleAndNetworked
    }
    public enum EnumGameType
    {
        None, Rounds, NewGame
    }
    public enum EnumGamePackageMode
    {
        None = 0, //to require it to be set.
        Debug = 1,
        Production = 3
    } //could have a second one but not sure.
    //public enum EnumNetworkStatus
    //{
    //    None = 0, // means beginning
    //    HostingWaitingForAtLeastOnePlayer = 1,
    //    HostingReadyToStart = 2, // its up to the host to decide to wait for more players or to start
    //    ConnectingWaitingToConnect = 3,
    //    ConnectedToHost = 4,
    //    SinglePlayer = 5,
    //    InGame = 6, // this means its in the actual game.
    //    GameOrRoundOver = 7 // this is needed so it will know that some things can't be done because the game or round is over.
    //}

    //hopefully with the new, some things are not required since its splitted out now.
    public enum EnumOpeningStatus
    {
        None,
        HostingWaitingForAtLeastOnePlayer,
        HostingReadyToStart,
        ConnectingWaitingToConnect,
        ConnectedToHost
    }
    public enum EnumSmallestSuggested //decided to do some renaming.  we don't support small tablets anymore but could have a way to support betweens again though.
    {
        AnyDevice = 1, //this means any should be able to play the game.
        AnyTablet = 2,
        LargeDevices = 3,
        DesktopOnly = 4
    } //decided to do as suggestions.  however, its more flexible in deciding.
    public enum EnumSuggestedOrientation
    {
        Landscape, Portrait
    }
}