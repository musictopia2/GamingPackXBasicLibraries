using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace GamePackageSignalRClasses
{
    public class SignalRLocalEndPoint : ISignalRInfo
    {
        Task<bool> ISignalRInfo.IsAzureAsync()
        {
            return Task.FromResult(false); //this is local
        }
        Task<string> ISignalRInfo.GetEndPointAsync()
        {
            return Task.FromResult("/hubs/gamepackage/messages"); //has to now add the extra part for messages.
        }
        Task<string> ITCPInfo.GetIPAddressAsync()
        {
            return Task.FromResult("http://192.168.0.150");
        }
        Task<int> ITCPInfo.GetPortAsync()
        {
            return Task.FromResult(9000); //so we can try out signalr.
        }
    }
}