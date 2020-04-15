using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using Xamarin.Forms;
namespace GameLoaderXF
{
    public interface ILoaderVM
    {
        //void Init(IGamePlatform platform, IStartUp starts, INavigation navigation);
        GamePackageLoaderPickerCP? PackagePicker { get; set; }

        //not sure what we need here.


        //cc.Command? ChooseGameCommand { get; set; }
    }
}