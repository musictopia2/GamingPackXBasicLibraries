using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    public class YahtzeePlayerItem<D> : SimplePlayer
        where D : SimpleDice, new()
    {
        private int _Points;
        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value)) { }
            }
        }
        [JsonIgnore]
        public ScoresheetVM<D>? Scoresheet { get; set; }
        public CustomBasicList<RowInfo> RowList { get; set; } = new CustomBasicList<RowInfo>(); //this would be used for the scoresheets.
    }
}