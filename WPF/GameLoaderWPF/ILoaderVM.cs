using BasicGameFrameworkLibrary.ChooserClasses;

namespace GameLoaderWPF
{
    public interface ILoaderVM
    {
        GamePackageLoaderPickerCP? PackagePicker { get; set; }
    }
}