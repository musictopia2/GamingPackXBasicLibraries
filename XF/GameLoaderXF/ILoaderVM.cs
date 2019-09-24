using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using Xamarin.Forms;
using cc = CommonBasicStandardLibraries.MVVMHelpers;
namespace GameLoaderXF
{
    public interface ILoaderVM
    {
        void Init(IGamePlatform platform, IStartUp starts, INavigation navigation);
        GamePackageLoaderPickerCP? PackagePicker { get; set; }
        cc.Command? ChooseGameCommand { get; set; }
    }
}