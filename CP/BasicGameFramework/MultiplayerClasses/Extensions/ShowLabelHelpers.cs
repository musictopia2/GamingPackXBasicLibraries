using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.Extensions
{
    public static class ShowLabelHelpers
    {
        public static void ShowTurn<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            if (thisGame.CurrentMod == null)
                return;
            if (thisGame.SingleInfo == null)
                return;
            thisGame.SingleInfo = thisGame.PlayerList!.GetWhoPlayer();
            thisGame.CurrentMod.NormalTurn = thisGame.SingleInfo.NickName;
        }
        public static void StartingStatus<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            if (thisGame.CurrentMod == null)
                return;
            if (thisGame.ThisData!.MultiPlayer == true)
                thisGame.CurrentMod.Status = "Multiplayer game in progress";
            else
                thisGame.CurrentMod.Status = "Single player game in progress";
        }
        public static async Task ProtectedShowTieAsync<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.NormalTurn = "None";
            thisGame.CurrentMod.Status = "Game Over.  It was a tie";
            await thisGame.CurrentMod.ShowGameMessageAsync("It was a tie");
        }
        public static async Task ProtectedShowWinAsync<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.NormalTurn = "None";
            thisGame.CurrentMod.Status = $"Game over.  {thisGame.SingleInfo!.NickName} has won";
            if (thisGame.ThisData!.MultiPlayer == false)
                await thisGame.CurrentMod.ShowGameMessageAsync($"{thisGame.SingleInfo.NickName} has won");
            else if (thisGame.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                await thisGame.CurrentMod.ShowGameMessageAsync($"{thisGame.SingleInfo.NickName} wins the game");
            else
                await thisGame.CurrentMod.ShowGameMessageAsync("You lose the game");
        }
        public static void ProtectedShowCustomWin<P>(this IBasicGameProcesses<P> thisGame, string playersWonMessage)
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.NormalTurn = "None";
            thisGame.CurrentMod.Status = $"Game Over.  {playersWonMessage} has won"; //this time, no messagebox.
        }
        public static async Task ProtectedShowLossAsync<P>(this IBasicGameProcesses<P> thisGame) //this is for games like old maid.
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.NormalTurn = "None";
            thisGame.CurrentMod.Status = $"Game over.  {thisGame.SingleInfo!.NickName} has lost";
            await thisGame.CurrentMod.ShowGameMessageAsync($"{thisGame.SingleInfo.NickName} is a loser");
        }
    }
}