using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BasicGameFrameworkLibrary.NetworkingClasses.Data
{
    public class FirstGameData : ObservableObject// this is  json now.
    {
        private bool _Client;
        public bool Client
        {
            get
            {
                return _Client;
            }
            set
            {
                if (SetProperty(ref _Client, value) == true) { }
            }
        }
        public CustomBasicCollection<PlayerInfo> PlayerList { get; set; } = new CustomBasicCollection<PlayerInfo>(); //i think
        private string _NickName = "";
        public string NickName
        {
            get
            {
                return _NickName;
            }
            set
            {
                if (SetProperty(ref _NickName, value) == true) { }
            }
        }
    }
}