using CommonBasicStandardLibraries.MVVMHelpers;
using Newtonsoft.Json;
namespace BasicGameFramework.NetworkingClasses.Data
{
    public class SentMessage : ObservableObject
    {
        private string _Status = "";
        public string Status
        {
            get { return _Status; }
            set
            {
                if (SetProperty(ref _Status, value)) { }
            }
        }
        private string _Body = "";
        public string Body
        {
            get { return _Body; }
            set
            {
                if (SetProperty(ref _Body, value)) { }
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented); //i do like indented.   has to be this way because can't use async in this situation
        }
    }
}