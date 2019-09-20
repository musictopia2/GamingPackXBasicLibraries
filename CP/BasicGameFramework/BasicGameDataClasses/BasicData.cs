namespace BasicGameFramework.BasicGameDataClasses
{
    public class BasicData
    {
        public bool MultiPlayer { get; set; }
        public string NickName { get; set; } = "";
        public bool IsXamarinForms { get; set; } //i think this is fine too.
        public EnumNetworkStatus NetworkStatus { get; set; } //decided to have it here now.
        public bool Client { get; set; }
        public EnumGamePackageMode GamePackageMode { get; set; } = EnumGamePackageMode.None; //default to none.  will require showing what it is.
    }
}