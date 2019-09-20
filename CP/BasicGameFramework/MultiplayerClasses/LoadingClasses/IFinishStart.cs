using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    /// <summary>
    /// if special things has to happen when its finished starting, will be here.
    /// crazy eights for example does have something special.
    /// </summary>
    public interface IFinishStart
    {
        Task FinishStartAsync();
    }
}