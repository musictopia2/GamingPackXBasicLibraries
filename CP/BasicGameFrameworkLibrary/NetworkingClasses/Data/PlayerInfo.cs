using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BasicGameFrameworkLibrary.NetworkingClasses.Data
{
    public class PlayerInfo : ObservableObject
    {
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
        private bool _IsHost;
        public bool IsHost
        {
            get
            {
                return _IsHost;
            }

            set
            {
                if (SetProperty(ref _IsHost, value) == true) { }
            }
        }
    }
}