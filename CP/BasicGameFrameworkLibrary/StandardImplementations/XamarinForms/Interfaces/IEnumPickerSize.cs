namespace BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces
{
    public interface IEnumPickerSize
    {
        float NormalGraphicsWidthHeight { get; }
        float SmallGraphicsWidthHeight { get; } //on desktop, was 150 but was reduced to 100.  whoever implements this can decide what is used for this.
    }
}
