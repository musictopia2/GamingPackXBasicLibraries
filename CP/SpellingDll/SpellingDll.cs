using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
namespace SpellingDll
{
    public enum EnumDifficulty
    {
        Easy = 1,
        Medium = 3,
        Hard = 3
    }
    public class WordInfo
    {
        public EnumDifficulty Difficulty { get; set; }
        public string Word { get; set; } = "";
        public int Letters { get; set; }
    }
    public class SpellingDll : IDisposable
    {
        public CustomBasicList<WordInfo> GetWords(EnumDifficulty? difficulty, int? letters)
        {
            if (difficulty.HasValue == false && letters.HasValue == false)
                return _thisList.ToCustomBasicList();// to make a new list so if any get removed; won't be removed from the dll
            if (difficulty.HasValue == false)
                return (from Items in _thisList
                        where Items.Letters == letters
                        select Items).ToCustomBasicList();
            if (letters.HasValue == false)
                return _thisList.Where(Items => Items.Difficulty == difficulty!.Value).ToCustomBasicList();
            return _thisList.Where(Items => Items.Difficulty == difficulty!.Value && Items.Letters == letters!.Value).ToCustomBasicList();
        }

        private readonly CustomBasicList<WordInfo> _thisList;
        public SpellingDll()
        {
            Assembly thisAssembly = Assembly.GetAssembly(GetType());
            string thisText = thisAssembly.ResourcesAllTextFromFile("spelling.json");
            _thisList = JsonConvert.DeserializeObject<CustomBasicList<WordInfo>>(thisText);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SpellingDll()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}