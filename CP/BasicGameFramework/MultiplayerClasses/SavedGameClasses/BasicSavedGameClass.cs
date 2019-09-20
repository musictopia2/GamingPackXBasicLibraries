using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using Newtonsoft.Json;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    /// <summary>
    /// This is the basic data needed to save a game.
    /// </summary>
    /// <typeparam name="P">This is the player data.</typeparam>
    public abstract class BasicSavedGameClass<P> : ObservableObject where P : class, IPlayerItem, new()
    {
        public bool NewRound { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        public PlayOrderClass PlayOrder = new PlayOrderClass();
        public PlayerCollection<P> PlayerList = new PlayerCollection<P>();
        public bool ImmediatelyStartTurn { get; set; }// if set to true, then will mean that will go to startnewturn instead of continueturn.
    }
}