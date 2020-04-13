using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GamePackageSignalRClasses
{
    public class ClientTCPClass : ITCPInfo
    {
        private readonly GlobalDataModel _data;
        public ClientTCPClass(GlobalDataModel data)
        {
            _data = data;
        }
        Task<string> ITCPInfo.GetIPAddressAsync()
        {
            if (string.IsNullOrWhiteSpace(_data.HostIPAddress))
                throw new BasicBlankException("No IP Address.  This should not have ran.  Rethink");
            return Task.FromResult(_data.HostIPAddress);
        }
        Task<int> ITCPInfo.GetPortAsync()
        {
            return Task.FromResult(8010);
        }
    }
}
