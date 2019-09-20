using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    /// <summary>
    /// i guess its okay to use this also for kismet since its yahtzee styled game.
    /// </summary>
    public class YahtzeeSaveInfo<D> : BasicSavedDiceClass<D, YahtzeePlayerItem<D>>
        where D : SimpleDice, new()
    {
        public int Begins { get; set; }
        private int _Round;
        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.Round = value;
                }

            }
        }
        private YahtzeeViewModel<D>? _thisMod; //this is needed so it can hook up.
        public void LoadMod(YahtzeeViewModel<D> thisMod)
        {
            _thisMod = thisMod;
            thisMod.Round = Round;
        }
    }
}