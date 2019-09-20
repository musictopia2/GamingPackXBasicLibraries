using System.Threading.Tasks;
namespace BasicGameFramework.NetworkingClasses.Interfaces
{
    public interface ISignalRInfo : ITCPInfo
    {
        Task<bool> IsAzureAsync();
        Task<string> GetEndPointAsync();
    }
}