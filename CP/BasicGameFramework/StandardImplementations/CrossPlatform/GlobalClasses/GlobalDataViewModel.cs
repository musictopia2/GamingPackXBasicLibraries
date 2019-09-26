using CommonBasicStandardLibraries.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.SpecializedViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.NetworkingClasses.Misc.GlobalStaticClasses;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses
{
    public class GlobalDataViewModel : DataEntryViewModel
    {
        private readonly GlobalDataLoaderClass _procs;
        private GlobalDataModel? _data;
        private EnumServerMode _ServerMode = EnumServerMode.AzureHosting; //default with azure.  easiest anyways.
        public EnumServerMode ServerMode
        {
            get { return _ServerMode; }
            set
            {
                if (SetProperty(ref _ServerMode, value))
                {
                    _data!.ServerMode = value;
                }
            }
        }
        private string _MainNickName = "";
        [Required]
        public string MainNickName
        {
            get { return _MainNickName; }
            set
            {
                if (SetProperty(ref _MainNickName, value))
                {
                    _data!.MainNickName = value;
                    OnPropertyChanged(nameof(CurrentNickName));
                }

            }
        }
        private string _SecondaryNickName = "";
        public string SecondaryNickName
        {
            get { return _SecondaryNickName; }
            set
            {
                if (SetProperty(ref _SecondaryNickName, value))
                {
                    _data!.SecondaryNickName = value;
                    OnPropertyChanged(nameof(CurrentNickName));
                }
            }
        }
        private string _AzureEndPointAddress = MainAzureHostAddress;
        [Required]
        public string AzureEndPointAddress
        {
            get { return _AzureEndPointAddress; }
            set
            {
                if (SetProperty(ref _AzureEndPointAddress, value))
                {
                    _data!.AzureEndPointAddress = value;
                }
            }
        }
        private string _HostIPAddress = "";
        public string HostIPAddress
        {
            get { return _HostIPAddress; }
            set
            {
                if (SetProperty(ref _HostIPAddress, value))
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
        public Command<EnumServerMode> ChangeServerOptionsCommand { get; set; }
        public Command MainNickCommand { get; set; }
        public Command DefaultAzureCommand { get; set; }
        public GlobalDataViewModel(GlobalDataLoaderClass procs)
        {
            _procs = procs;
            ChangeServerOptionsCommand = new Command<EnumServerMode>(mode =>
            {
                ServerMode = mode;
            }, mode =>
            {
                return CanProcess(mode);
            }, this);
            MainNickCommand = new Command(async items =>
            {
                SecondaryNickName = "";
                await ProcessSave(null!);
            }, items =>
            {
                return !string.IsNullOrWhiteSpace(SecondaryNickName);
            }, this);
            DefaultAzureCommand = new Command(items =>
            {
                AzureEndPointAddress = MainAzureHostAddress; //set back to default one.
            }, items => true, this);
        }
        public async Task InitAsync()
        {
            _data = await _procs.OpenAsync();
        }
        protected override async Task ProcessSave(object thisObj)
        {
            if (_data == null)
                throw new BasicBlankException("Should had loaded data.  Rethink");
            await _procs.SaveAsync(_data); //i think
            ThisMessage.CloseProgram();
        }
    }
}