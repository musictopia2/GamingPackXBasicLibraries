using BasicGameFramework.Dice;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public abstract class BasicSavedDiceClass<D, P> : BasicSavedGameClass<P>
        where D : IStandardDice, new()
        where P : class, IPlayerItem, new()
    {
        public DiceList<D> DiceList = new DiceList<D>(); //forgot to set to new dicelst.
        private int _RollNumber;
        public int RollNumber
        {
            get { return _RollNumber; }
            set
            {
                if (SetProperty(ref _RollNumber, value))
                {
                    if (ThisMod == null)
                        return;
                    ThisMod.RollNumber = value;
                }
            }
        }
        [JsonIgnore]
        public IDiceVM<D, P>? ThisMod;
    }
}