using CommonBasicStandardLibraries.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static BasicGameFrameworkLibrary.NetworkingClasses.Misc.GlobalStaticClasses;
namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses
{


    public class GlobalDataViewModel : Screen
    {
        public static GlobalDataViewModel? GlobalData { get; set; }
        public GlobalDataViewModel(GlobalDataLoaderClass procs)
        {
            _procs = procs;
            LocalIPAddress = GetIPAddress();
            DisplayName = "Game Package Global Settings";
            GlobalData = this;
        }

        private string GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new BasicBlankException("Unable to find ip address.  Rethink");
        }

        protected override async Task ActivateAsync(IUIView view)
        {
            _data = await _procs.OpenAsync();
            MainNickName = _data.MainNickName;
            SecondaryNickName = _data.SecondaryNickName;
            ServerMode = _data.ServerMode;
            HostIPAddress = _data.HostIPAddress;
            AzureEndPointAddress = _data.AzureEndPointAddress;
            await base.ActivateAsync(view);
        }
        
        public bool CanSave()
        {
            return IsValid;
        }
        public async Task SaveAsync()
        {
            if (_data == null)
            {
                throw new BasicBlankException("Should had loaded data.  Rethink");
            }
            await _procs.SaveAsync(_data); //i think
            UIPlatform.ExitApp();
        }

        private readonly GlobalDataLoaderClass _procs;
        private GlobalDataModel? _data;
        private EnumServerMode _serverMode = EnumServerMode.AzureHosting; //default with azure.  easiest anyways.
        public EnumServerMode ServerMode
        {
            get { return _serverMode; }
            set
            {
                if (SetProperty(ref _serverMode, value))
                {
                    _data!.ServerMode = value;
                }
            }
        }
        private string _mainNickName = "";
        [Required]
        public string MainNickName
        {
            get { return _mainNickName; }
            set
            {
                if (SetProperty(ref _mainNickName, value))
                {
                    _data!.MainNickName = value;
                    OnPropertyChanged(nameof(CurrentNickName));
                }

            }
        }
        private string _secondaryNickName = "";
        public string SecondaryNickName
        {
            get { return _secondaryNickName; }
            set
            {
                if (SetProperty(ref _secondaryNickName, value))
                {
                    _data!.SecondaryNickName = value;
                    OnPropertyChanged(nameof(CurrentNickName));
                }
            }
        }
        private string _azureEndPointAddress = MainAzureHostAddress;
        [Required]
        public string AzureEndPointAddress
        {
            get { return _azureEndPointAddress; }
            set
            {
                if (SetProperty(ref _azureEndPointAddress, value))
                {
                    _data!.AzureEndPointAddress = value;
                }
            }
        }
        private string _hostIPAddress = "";
        public string HostIPAddress
        {
            get { return _hostIPAddress; }
            set
            {
                if (SetProperty(ref _hostIPAddress, value))
                {
                    _data!.HostIPAddress = value;
                }

            }
        }
        public string CurrentNickName => GlobalDataLoaderClass.CurrentNickName(_data!);

        private bool CanProcess(EnumServerMode mode)
        {
            //this shows whether this mode can be processed or not.
            if (mode == EnumServerMode.HomeHosting)
            {
                return CanConnectToHomeGameServer;
            }
            if (mode == EnumServerMode.LocalHosting)
            {
                return !string.IsNullOrWhiteSpace(HostIPAddress); //if you entered something for this, then you can choose local hosting
            }
            return true; //well see.
        }
        public bool CanChangeServerOptions(EnumServerMode mode) => CanProcess(mode);
        
        public void ChangeServerOptions(EnumServerMode mode)
        {
            ServerMode = mode;
        }

        public void DefaultAzure()
        {
            AzureEndPointAddress = MainAzureHostAddress; //set back to default one.
        }
        public void ClearAzure()
        {
            AzureEndPointAddress = "";
        }
        public bool CanBackToMainNickName => !string.IsNullOrWhiteSpace(SecondaryNickName);
        public async Task BackToMainNickNameAsync()
        {
            SecondaryNickName = "";
            await SaveAsync();
        }

        public string LocalIPAddress { get; private set; }
        
    }
}