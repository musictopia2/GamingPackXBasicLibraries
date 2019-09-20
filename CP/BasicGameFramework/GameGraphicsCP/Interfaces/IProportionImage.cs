namespace BasicGameFramework.GameGraphicsCP.Interfaces
{
    /// <summary>
    /// This is used so sizes can be different on different devices.
    /// if 1.0 is used, then whoever uses it will use default size.
    /// less than 1 means will be smaller
    /// greater than one means will be greater.
    /// </summary>
    public interface IProportionImage
    {
        float Proportion { get; }
    }
}