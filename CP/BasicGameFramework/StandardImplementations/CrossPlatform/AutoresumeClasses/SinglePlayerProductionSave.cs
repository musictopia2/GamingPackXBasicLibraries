using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.IO;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.AutoresumeClasses
{
    public class SinglePlayerProductionSave : ISaveSinglePlayerClass
    {
        private readonly string _gamePath;
        private readonly IGameInfo _thisData;
        public SinglePlayerProductionSave(IGameInfo thisGame, BasicData data)
        {
            _thisData = thisGame;
            string tempPath;
            if (data.IsXamarinForms == false)
            {
                tempPath = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.ApplicationPath.GetApplicationPath();
                tempPath = Path.Combine(tempPath, "json");
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);
            }
            else
            {
                tempPath = GetWriteLocationForExternalOnAndroid();
                tempPath = Path.Combine(tempPath, "GPXV2"); //this is now version 2.
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);
                tempPath = Path.Combine(tempPath, "AutoResume");
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath); //step 1 is seeing if it can create proper folders.  step 2 would actually have autoresume.
            }
            _gamePath = Path.Combine(tempPath, $"{_thisData.GameName}.json");
        }
        Task<bool> ISaveSinglePlayerClass.CanOpenSavedSinglePlayerGameAsync()
        {
            if (_thisData.CanAutoSave == false)
                return Task.FromResult(false);
            return Task.FromResult(FileExists(_gamePath));
        }
        async Task ISaveSinglePlayerClass.DeleteSinglePlayerGameAsync()
        {
            if (_thisData.CanAutoSave == false)
                return;
            await DeleteFileAsync(_gamePath);
        }
        async Task<T> ISaveSinglePlayerClass.RetrieveSinglePlayerGameAsync<T>()
        {
            if (_thisData.CanAutoSave == false)
                throw new BasicBlankException("Should not have autosaved.  Should have first called CanOpenSavedSinglePlayerGameAsync To See");
            return await fs.RetrieveSavedObjectAsync<T>(_gamePath);
        }
        async Task ISaveSinglePlayerClass.SaveSimpleSinglePlayerGameAsync(object thisObject)
        {
            if (_thisData.CanAutoSave == false)
                return;
            await fs.SaveObjectAsync(_gamePath, thisObject);
        }
    }
}