using System.Threading.Tasks;
namespace BasicGameFramework.MultiplayerClasses.LoadingClasses
{
    public interface ISetObjects
    {
        /// <summary>
        /// You have to set the object of the saveroot to whatever you need.
        /// some games has nothing but others to.  
        /// </summary>
        /// <returns></returns>
        Task SetSaveRootObjectsAsync();
        //i think this should handle setup as well.
    }
}