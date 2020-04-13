namespace BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses
{
    //this focuses just on the information needed to see who has to play.
    public class PlayOrderClass : IPlayOrder
    {
        public int WhoTurn { get; set; }
        public int OtherTurn { get; set; }
        public bool IsReversed { get; set; }
        public int WhoStarts { get; set; }
    }
}