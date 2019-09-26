using CommonBasicStandardLibraries.Exceptions;
using System.IO;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses
{
    public class GlobalDataLoaderClass
    {
        public GlobalDataLoaderClass(IHostedUIInterface ui) //unfortunately had to do this way since it can load via dependency injection.
        {
            IsXamarinForms = ui.IsXamarinForms;
            if (IsXamarinForms == false)
            {
                if (DirectoryExists(_desktopPath) == false)
                    Directory.CreateDirectory(_desktopPath);
            }
            else
            {
                string tempPath = GetWriteLocationForExternalOnAndroid();
                tempPath = Path.Combine(tempPath, "GPXV2"); //this is now version 2.
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);
            }
        }
        private bool IsXamarinForms { get; }
        private const string _desktopPath = @"C:\Gaming Pack X";
        private static string GetPath(bool isXamarinForms)
        {
            if (isXamarinForms == false)
                return Path.Combine(_desktopPath, "gpxsettings.json");
            string tempPath = GetWriteLocationForExternalOnAndroid();
            tempPath = Path.Combine(tempPath, "GPXV2"); //this is now version 2.
            return Path.Combine(tempPath, "gpxsettings.json");
        }

        public async Task SaveAsync(GlobalDataModel payLoad)
        {
            string path = GetPath(IsXamarinForms);
            await fs.SaveObjectAsync(path, payLoad);
        }
        public async Task<GlobalDataModel> OpenAsync()
        {
            if (HasSettings(IsXamarinForms) == false)
                return new GlobalDataModel(); //just return a new one period.
            string path = GetPath(IsXamarinForms);
            return await fs.RetrieveSavedObjectAsync<GlobalDataModel>(path);
        }
        public static string CurrentNickName(GlobalDataModel data) //i think should be able to do without object if i choose.
        {
            if (data.MainNickName == "")
                throw new BasicBlankException("No nick name can be found.  Should have called HasSettings and acted accordingly");
            if (data.SecondaryNickName != "")
                return data.SecondaryNickName;
            return data.MainNickName;
        }
        public static bool HasSettings(bool isXamarinForms)
        {
            string path = GetPath(isXamarinForms);
            return FileExists(path);
        }
    }
}