namespace BasicGameFramework.StandardImplementations.XamarinForms.Interfaces
{
    /// <summary>
    /// This is the details needed in order to figure out the default font sizes for various pages.  something has to implement this.
    /// Whoever does has to figure out what the font sizes will be.
    /// </summary>
    public interface IFontProcesses
    {
        double SmallFontSize { get; }
        double NormalFontSize { get; }
        double HeightRequest { get; }
    }
}