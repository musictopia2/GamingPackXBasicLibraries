using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.AutoresumeClasses
{
    public class MultiplayerProductionSave : IMultiplayerSaveState
    {
        private readonly string _localPath;
        private readonly string _multiPath; //best 
        private readonly TestOptions _test;
        private readonly IGameInfo _game;
        private readonly BasicData _data;
        private readonly string _parentPath = ""; //only the host should do anything as well.
        private bool _showCurrent; //if no autoresume, then obviously would not show the last game played.
        private readonly string _lastPath = "";

        private LimitedList<IMappable>? _list; //has to be initialized eventually.  needs lazy loading.
        private IMappable? _previousObject;

        public MultiplayerProductionSave(IGameInfo game, BasicData data, TestOptions test)
        {
            JsonSettingsGlobals.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None; //try this way.  because otherwise, does not work if not everybody is .net core unfortunately.
            JsonSettingsGlobals.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None; //try this as well.  otherwise, gets hosed with .net core and xamarin forms.
            //this is needed even for this way because otherwise, a desktop cannot host while the clients are android devices.
            string tempPath;
            if (data.IsXamarinForms == false)
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
            _game = game;
            _data = data;
            _test = test;
            _localPath = Path.Combine(tempPath, $"{game.GameName} Single.json");
            _multiPath = Path.Combine(tempPath, $"{game.GameName} Multiplayer.json");

        }
        private bool CanChange()
        {
            if (_game.CanAutoSave == false || _test.SaveOption != EnumTestSaveCategory.Normal)
                return false;
            return true;
        }
        async Task IMultiplayerSaveState.DeleteGameAsync()
        {
            if (CanChange() == false)
                return;
            if (_list != null)
            {
                _list = new LimitedList<IMappable>(); //i think
            }
            _previousObject = null;
            if (_data.MultiPlayer && _showCurrent == true)
                await DeleteFileAsync(_lastPath);
            if (_data.MultiPlayer == false)
                await DeleteFileAsync(_localPath);
            else
                await DeleteFileAsync(_multiPath);
        }
        private async Task PrivateSaveStateAsync<T>(T payLoad)
            where T : IMappable, new()
        {
            if (CanChange() == false)
                return;
            if (_data.MultiPlayer == true && _showCurrent == false && _data.IsXamarinForms)
            {
                await WriteAllTextAsync(_lastPath, _game.GameName); //since we have ui friendly, then when reaches the server, the server has to do the proper thing (do the parsing).
                _showCurrent = true; //because already shown.
            }
            string pathUsed;
            if (_data.MultiPlayer == false)
            {
                pathUsed = _localPath;
            }
            else
            {
                pathUsed = _multiPath;
            }

            bool repeat;

            if (_previousObject != null)
            {
                string oldstr = JsonConvert.SerializeObject(_previousObject);
                string newStr = JsonConvert.SerializeObject(payLoad);
                repeat = oldstr == newStr;
                _previousObject = payLoad.AutoMap<T>();
            }
            else
            {
                _previousObject = payLoad.AutoMap<T>();
                repeat = false;
            }
            if (_list == null)
            {
                _list = new LimitedList<IMappable>();
            }


            if (repeat == false)
            {
                _list.Add(_previousObject);
                await fs.SaveObjectAsync(pathUsed, _list); //hopefully okay.
                
            }
        }
        async Task<EnumRestoreCategory> IMultiplayerSaveState.MultiplayerRestoreCategoryAsync()
        {
            await Task.CompletedTask;
            if (_test.SaveOption == EnumTestSaveCategory.NoSave)
                return EnumRestoreCategory.NoRestore;
            bool rets = FileExists(_multiPath);
            if (rets == false)
                return EnumRestoreCategory.NoRestore; //because there is no autoresume game.
            if (_test.SaveOption == EnumTestSaveCategory.RestoreOnly)
                return EnumRestoreCategory.MustRestore;
            return EnumRestoreCategory.CanRestore;
        }

        async Task IMultiplayerSaveState.SaveStateAsync<T>(T thisState)
        {
            await PrivateSaveStateAsync(thisState);
        }
        async Task<EnumRestoreCategory> IMultiplayerSaveState.SinglePlayerRestoreCategoryAsync()
        {
            await Task.CompletedTask;
            if (_test.SaveOption == EnumTestSaveCategory.NoSave)
                return EnumRestoreCategory.NoRestore;
            bool rets = FileExists(_localPath);
            if (rets == false)
                return EnumRestoreCategory.NoRestore; //because there is no autoresume game.
            if (_test.SaveOption == EnumTestSaveCategory.RestoreOnly)
                return EnumRestoreCategory.MustRestore;
            return EnumRestoreCategory.CanRestore;
        }

        async Task<string> IMultiplayerSaveState.TempMultiSavedAsync()
        {
            if (_game.CanAutoSave == false)
                return "";
            if (_test.SaveOption == EnumTestSaveCategory.NoSave)
                return "";
            if (FileExists(_multiPath) == false)
                return "";
            LimitedList<object> temps = await fs.RetrieveSavedObjectAsync<LimitedList<object>>(_multiPath);
            var item = temps.MostRecent;
            return JsonConvert.SerializeObject(item);

        }


        async Task<string> IMultiplayerSaveState.SavedDataAsync<T>()
        {
            if (_game.CanAutoSave == false)
                return "";
            if (_test.SaveOption == EnumTestSaveCategory.NoSave)
                return "";

            string pathUsed = "";
            if (_data.MultiPlayer == false)
            {
                if (FileExists(_localPath) == false)
                    return "";
                pathUsed = _localPath;
                //return await AllTextAsync(_localPath); //they have to deseriaize it later.
            }

            if (_data.MultiPlayer == true && FileExists(_multiPath) == false)
                return "";
            if (_data.MultiPlayer)
            {
                pathUsed = _multiPath;
            }

            LimitedList<T> temps = await fs.RetrieveSavedObjectAsync<LimitedList<T>>(pathUsed);
            _list = new LimitedList<IMappable>();
            _list.PopulateSavedList(temps);
            //looks like i have to do generics after all.
            //if i did as string, then even if i could get it to work eventually, the problem is i can't easily debug anymore.
            //can't deserialize as string or even imappable.
            //if i made it more picky, then later causes other issues.


            if (_test.StatePosition == 0)
            {
                _previousObject = _list.MostRecent;
            }
            else
            {
                _previousObject = _list[_test.StatePosition];
            }
            return JsonConvert.SerializeObject(_previousObject);
        }

        
    }
}