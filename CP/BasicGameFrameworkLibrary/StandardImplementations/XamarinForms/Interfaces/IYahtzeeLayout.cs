namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces
{
    public interface IYahtzeeLayout
    {
        double FooterFontSize { get; }
        double DescriptionFontSize { get; } //maybe we don't need header font size now (?)
        int GetPixelHeight { get; }
        double StandardFontSize { get; }
    }
}