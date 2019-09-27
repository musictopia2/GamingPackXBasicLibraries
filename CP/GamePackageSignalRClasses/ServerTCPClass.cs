using BasicGameFramework.NetworkingClasses.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.NetworkingClasses.Misc.GlobalStaticClasses;
namespace GamePackageSignalRClasses
{
    public class ServerTCPClass : ITCPInfo
    {
        Task<string> ITCPInfo.GetIPAddressAsync()
        {
            return Task.FromResult(LocalIPAddress);
        }
        Task<int> ITCPInfo.GetPortAsync()
        {
            return Task.FromResult(8010);
        }
    }
}