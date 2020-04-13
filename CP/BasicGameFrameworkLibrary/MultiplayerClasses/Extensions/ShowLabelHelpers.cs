using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.Extensions
{
    public static class ShowLabelHelpers
    {
        public static void ShowTurn<P>(this IBasicGameProcesses<P> game)
            where P : class, IPlayerItem, new()
        {
            if (game.CurrentMod == null)
                return;
            if (game.SingleInfo == null)
                return;
            game.SingleInfo = game.PlayerList!.GetWhoPlayer();
            game.CurrentMod.NormalTurn = game.SingleInfo.NickName;
        }
        public static void StartingStatus<P>(this IBasicGameProcesses<P> game)
            where P : class, IPlayerItem, new()
        {
            if (game.CurrentMod == null)
                return;
            if (game.BasicData!.MultiPlayer == true)
                game.CurrentMod.Status = "Multiplayer game in progress";
            else
                game.CurrentMod.Status = "Single player game in progress";
        }
        public static async Task ProtectedShowTieAsync<P>(this IBasicGameProcesses<P> game)
            where P : class, IPlayerItem, new()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = "Game Over.  It was a tie";
            await UIPlatform.ShowMessageAsync("It was a tie");
        }
        public static async Task ProtectedShowWinAsync<P>(this IBasicGameProcesses<P> game)
            where P : class, IPlayerItem, new()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game over.  {game.SingleInfo!.NickName} has won";
            if (game.BasicData!.MultiPlayer == false)
                await UIPlatform.ShowMessageAsync($"{game.SingleInfo.NickName} has won");
            else if (game.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                await UIPlatform.ShowMessageAsync($"{game.SingleInfo.NickName} wins the game");
            else
                await UIPlatform.ShowMessageAsync("You lose the game");
        }
        public static void ProtectedShowCustomWin<P>(this IBasicGameProcesses<P> game, string playersWonMessage)
            where P : class, IPlayerItem, new()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game Over.  {playersWonMessage} has won"; //this time, no messagebox.
        }
        public static async Task ProtectedShowLossAsync<P>(this IBasicGameProcesses<P> game) //this is for games like old maid.
            where P : class, IPlayerItem, new()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game over.  {game.SingleInfo!.NickName} has lost";
            await UIPlatform.ShowMessageAsync($"{game.SingleInfo.NickName} is a loser");
        }
    }
}
