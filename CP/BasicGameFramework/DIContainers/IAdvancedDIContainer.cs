namespace BasicGameFramework.DIContainers
{
    public interface IAdvancedDIContainer
    {
        IGamePackageResolver? MainContainer { get; set; }
    }
}