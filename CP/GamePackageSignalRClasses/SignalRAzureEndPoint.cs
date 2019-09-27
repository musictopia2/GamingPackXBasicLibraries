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
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses;
//i think this is the most common things i like to do
namespace GamePackageSignalRClasses
{
    public class SignalRAzureEndPoint : ISignalRInfo
    {
        private readonly GlobalDataModel _global;
        public SignalRAzureEndPoint(GlobalDataModel global)
        {
            _global = global;
        }
        Task<string> ISignalRInfo.GetEndPointAsync()
        {
            return Task.FromResult("/hubs/gamepackage/messages");
        }
        Task<string> ITCPInfo.GetIPAddressAsync()
        {
            if (string.IsNullOrWhiteSpace(_global.AzureEndPointAddress))
                throw new BasicBlankException("Should not have allowed hosting in azure if there is no end point");
            return Task.FromResult(_global.AzureEndPointAddress);
        }
        Task<int> ITCPInfo.GetPortAsync()
        {
            return Task.FromResult(80);
        }

        Task<bool> ISignalRInfo.IsAzureAsync()
        {
            return Task.FromResult(true);
        }
    }
}
