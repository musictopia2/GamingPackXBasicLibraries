using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.IO;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileFunctions;
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using Newtonsoft.Json;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.AutoresumeClasses
{
    public class SinglePlayerProductionSave : ISaveSinglePlayerClass
    {


        public static int RecentOne { get; set; } = 0; //0 means will be most recent.  otherwise, will show a past one.  helpful in testing.

        private readonly string _gamePath;
        private readonly IGameInfo _thisData;
        private readonly BasicData _data;
        private LimitedList<IMappable>? _list; //has to be initialized eventually.  needs lazy loading.

        public SinglePlayerProductionSave(IGameInfo thisGame, BasicData data)
        {
            _thisData = thisGame;
            _data = data;
            //in production, history just won't work.


            //if (data.GamePackageMode == EnumGamePackageMode.Production && )
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
            if (_list != null)
            {
                _list = new LimitedList<IMappable>(); //i think
            }
            await DeleteFileAsync(_gamePath);
        }
        async Task<T> ISaveSinglePlayerClass.RetrieveSinglePlayerGameAsync<T>()
        {
            if (_thisData.CanAutoSave == false)
                throw new BasicBlankException("Should not have autosaved.  Should have first called CanOpenSavedSinglePlayerGameAsync To See");


            //here, get the list.
            //_list = await fs.RetrieveSavedObjectAsync<LimitedList<IMappable>>(_gamePath); //hopefully no problem (?)

            LimitedList<T> temps = await fs.RetrieveSavedObjectAsync<LimitedList<T>>(_gamePath);
            _list = new LimitedList<IMappable>();
            _list.PopulateSavedList(temps);

            if (_data.GamePackageMode == EnumGamePackageMode.Production || RecentOne == 0)
            {
                _previousObject = _list.MostRecent.AutoMap<T>();
                return (T)_list.MostRecent!; //take a risk here too.
            }
            //in production, can add to history though.
            _previousObject = _list[RecentOne].AutoMap<T>();
            return (T)_list[RecentOne]!;

            //await Task.CompletedTask;
            //return fs.RetrieveSavedObject<T>(_gamePath);
            //return await fs.RetrieveSavedObjectAsync<T>(_gamePath);
        }
        private IMappable? _previousObject;
        async Task ISaveSinglePlayerClass.SaveSimpleSinglePlayerGameAsync<T>(T thisObject)
        {
            if (_thisData.CanAutoSave == false)
                return;
            if (thisObject == null)
            {
                throw new BasicBlankException("Cannot save null object.  Rethink");
            }
            //fs.SaveObject(_gamePath, thisObject);
            //string content = JsonConvert.SerializeObject(thisObject, Formatting.Indented);
            ////i guess i can't do the serializing that way anymore.
            //WriteAllText(_gamePath, content);
            //await Task.CompletedTask;
            bool repeat;
            if (_previousObject != null)
            {
                //bool rets = _previousObject.Equals(thisObject);
                string oldstr = JsonConvert.SerializeObject(_previousObject);
                string newStr = JsonConvert.SerializeObject(thisObject);
                repeat = oldstr == newStr;
                _previousObject = thisObject.AutoMap<T>();
            }
            else
            {
                _previousObject = thisObject.AutoMap<T>();
                repeat = false;
            }
            if (_list == null)
            {
                _list = new LimitedList<IMappable>();
            }

            
            if (repeat == false)
            {
                _list.Add(_previousObject);
                await fs.SaveObjectAsync(_gamePath, _list); //hopefully okay.
            }
        }
    }
}