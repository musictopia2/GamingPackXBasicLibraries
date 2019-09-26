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
using CommonBasicStandardLibraries.MVVMHelpers.SpecializedViewModels;
using CommonBasicStandardLibraries.Attributes;
using static BasicGameFramework.NetworkingClasses.Misc.GlobalStaticClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
//i think this is the most common things i like to do
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
        }
    }
}