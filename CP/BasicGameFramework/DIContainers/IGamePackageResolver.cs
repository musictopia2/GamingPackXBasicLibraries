using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
namespace BasicGameFramework.DIContainers
{
    public interface IGamePackageResolver : IResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Tag">This is extra info so it can more easily return the proper implementation  its an object so can represent anything</param>
        /// <returns></returns>
        T Resolve<T>(string tag); //sometimes a person has a tag that will be used to resolve. was going to be string but decided to make it object.
        /// <summary>
        /// This is used in cases where the object was replaced.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NewObject"></param>
        void ReplaceObject<T>(T newObject);

        /// <summary>
        /// This is used in cases where the object was replaced.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NewObject"></param>
        void ReplaceObject<T>(T newObject, string tag);

        /// <summary>
        /// If the tag looking up is not found, then it returns false
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        bool LookUpValue(string tag);
        bool ObjectExist<T>(string tag);
        bool ObjectExist<T>(); //sometimes, there is a tag.  sometimes there is not.
    }
}