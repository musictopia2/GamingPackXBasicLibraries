using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.TestUtilities;
using System.IO;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.AutoresumeClasses
{
    public class MultiplayerProductionSave : IMultiplayerSaveState
    {
        private readonly string _localPath;
        private readonly string _multiPath; //best 
        private readonly TestOptions _thisTest;
        private readonly IGameInfo _thisGame;
        private readonly BasicData _thisData;
        private readonly string _parentPath = ""; //only the host should do anything as well.
        private bool _showCurrent; //if no autoresume, then obviously would not show the last game played.
        private readonly string _lastPath = "";
        public MultiplayerProductionSave(IGameInfo thisGame, BasicData thisData, TestOptions thisTest)
        {
            string tempPath;
            if (thisData.IsXamarinForms == false)
            {
                tempPath = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.ApplicationPath.GetApplicationPath();
                tempPath = Path.Combine(tempPath, "json"); //only desktop does it.  since xamarin form does something else.
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);
            }
            else
            {
                tempPath = GetWriteLocationForExternalOnAndroid();
                tempPath = Path.Combine(tempPath, "GPXV2"); //this is now version 2.
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath);
                _parentPath = tempPath;
                tempPath = Path.Combine(tempPath, "AutoResume");
                if (DirectoryExists(tempPath) == false)
                    Directory.CreateDirectory(tempPath); //step 1 is seeing if it can create proper folders.  step 2 would actually have autoresume.
                _lastPath = Path.Combine(_parentPath, "lastgame.txt");
            }
            _thisGame = thisGame;
            _thisData = thisData;
            _thisTest = thisTest;
            _localPath = Path.Combine(tempPath, $"{thisGame.GameName} Single.json");
            _multiPath = Path.Combine(tempPath, $"{thisGame.GameName} Multiplayer.json");

        }
        private bool CanChange()
        {
            if (_thisGame.CanAutoSave == false || _thisTest.SaveOption != EnumTestSaveCategory.Normal)
                return false;
            return true;
        }
        async Task IMultiplayerSaveState.DeleteGameAsync()
        {
            if (CanChange() == false)
                return;
            if (_thisData.MultiPlayer && _showCurrent == true)
                await DeleteFileAsync(_lastPath);
            if (_thisData.MultiPlayer == false)
                await DeleteFileAsync(_localPath);
            else
                await DeleteFileAsync(_multiPath);
        }
        private async Task PrivateSaveStateAsync(object thisObject)
        {
            if (CanChange() == false)
                return;
            if (_thisData.MultiPlayer == true && _showCurrent == false && _thisData.IsXamarinForms)
            {
                await WriteAllTextAsync(_lastPath, _thisGame.GameName); //since we have ui friendly, then when reaches the server, the server has to do the proper thing (do the parsing).
                _showCurrent = true; //because already shown.
            }
            if (_thisData.MultiPlayer == false)
                await fs.SaveObjectAsync(_localPath, thisObject);
            else
                await fs.SaveObjectAsync(_multiPath, thisObject);
        }
        async Task<EnumRestoreCategory> IMultiplayerSaveState.MultiplayerRestoreCategoryAsync()
        {
            await Task.CompletedTask;
            if (_thisTest.SaveOption == EnumTestSaveCategory.NoSave)
                return EnumRestoreCategory.NoRestore;
            bool rets = FileExists(_multiPath);
            if (rets == false)
                return EnumRestoreCategory.NoRestore; //because there is no autoresume game.
            if (_thisTest.SaveOption == EnumTestSaveCategory.RestoreOnly)
                return EnumRestoreCategory.MustRestore;
            return EnumRestoreCategory.CanRestore;
        }
        async Task<string> IMultiplayerSaveState.SavedDataAsync()
        {
            if (_thisGame.CanAutoSave == false)
                return "";
            if (_thisTest.SaveOption == EnumTestSaveCategory.NoSave)
                return "";
            if (_thisData.MultiPlayer == false)
            {
                if (FileExists(_localPath) == false)
                    return "";
                return await AllTextAsync(_localPath); //they have to deseriaize it later.
            }
            if (FileExists(_multiPath) == false)
                return "";
            return await AllTextAsync(_multiPath);
        }
        async Task IMultiplayerSaveState.SaveStateAsync(object thisState)
        {
            await PrivateSaveStateAsync(thisState);
        }
        async Task<EnumRestoreCategory> IMultiplayerSaveState.SinglePlayerRestoreCategoryAsync()
        {
            await Task.CompletedTask;
            if (_thisTest.SaveOption == EnumTestSaveCategory.NoSave)
                return EnumRestoreCategory.NoRestore;
            bool rets = FileExists(_localPath);
            if (rets == false)
                return EnumRestoreCategory.NoRestore; //because there is no autoresume game.
            if (_thisTest.SaveOption == EnumTestSaveCategory.RestoreOnly)
                return EnumRestoreCategory.MustRestore;
            return EnumRestoreCategory.CanRestore;
        }
        async Task<string> IMultiplayerSaveState.TempMultiSavedAsync()
        {
            if (_thisGame.CanAutoSave == false)
                return "";
            if (_thisTest.SaveOption == EnumTestSaveCategory.NoSave)
                return "";
            if (FileExists(_multiPath) == false)
                return "";
            return await AllTextAsync(_multiPath);
        }
    }
}