using System.Threading.Tasks;
namespace BasicGameFramework.NetworkingClasses.Interfaces
{
    public interface ITCPInfo
    {
        Task<string> GetIPAddressAsync();
        Task<int> GetPortAsync();
    }
}