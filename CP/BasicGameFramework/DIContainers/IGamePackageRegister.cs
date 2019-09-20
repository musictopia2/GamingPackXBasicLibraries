using System;
namespace BasicGameFramework.DIContainers
{
    public interface IGamePackageRegister
    {
        void RegisterSingleton<TIn, TOut>() where TOut : TIn;
        void RegisterSingleton(Type thisType); //this means that you will register one type as singleton.
        void RegisterSingleton<TIn, TOut>(string tag);
        void RegisterSingleton(Type thisType, string tag);
        void RegisterTrue(string tag); //this means if somebody looks up the tag, it will be true;
    }
}