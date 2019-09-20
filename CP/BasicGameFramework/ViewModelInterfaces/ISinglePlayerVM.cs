using System.Threading.Tasks;
namespace BasicGameFramework.ViewModelInterfaces
{
    public interface ISinglePlayerVM : IBasicGameVM
    {
        Task StartNewGameAsync(); //maybe this makes most sense.
    }
}