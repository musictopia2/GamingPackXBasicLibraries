using BasicGameFramework.NetworkingClasses.Data;
using System.Threading.Tasks;
namespace BasicGameFramework.NetworkingClasses.Interfaces
{
    public interface IServerMessage
    {
        Task ProcessDataAsync(SentMessage thisData);
    }
}