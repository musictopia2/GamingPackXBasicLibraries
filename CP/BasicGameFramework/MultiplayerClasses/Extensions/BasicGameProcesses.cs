using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.Extensions
{
    public static class BasicGameProcesses
    {
        public static async Task ProtectedGameOverNextAsync<P>(this IBasicGameProcesses<P> thisGame, IMultiplayerSaveState thisState)
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.NormalTurn = "None";
            if (thisGame.ThisData!.MultiPlayer == true && thisGame.ThisData.Client == true)
            {
                thisGame.CurrentMod.CommandContainer!.ManuelFinish = true;
                thisGame.CurrentMod.CommandContainer.IsExecuting = true; //to double check.  since its waiting for message.
                thisGame.ThisCheck!.IsEnabled = true;
                return;
            }
            await thisState.DeleteGameAsync();
            thisGame.CurrentMod.NewGameVisible = true;
            thisGame.CurrentMod.CommandContainer!.IsExecuting = false; //i think
        }
        public static void RoundOverNext<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            if (thisGame.ThisData!.MultiPlayer == false || thisGame.ThisData.Client == false)
            {
                thisGame.CurrentMod.Status = "Goto the next round";
                thisGame.CurrentMod.NewRoundVisible = true; //brand new.
                thisGame.CurrentMod.CommandContainer!.ManuelFinish = false;
                thisGame.CurrentMod.CommandContainer.IsExecuting = false;
            }
            else
            {
                thisGame.CurrentMod.Status = "Waiting for host to goto the next round";
                thisGame.ThisCheck!.IsEnabled = true;
                thisGame.CurrentMod.CommandContainer!.ManuelFinish = true;
                thisGame.CurrentMod.CommandContainer.IsExecuting = true; //to double check.  since its waiting for message.
            }
        }
        public static void ShowConnected<P>(this IBasicGameProcesses<P> thisGame)
            where P : class, IPlayerItem, new()
        {
            thisGame.CurrentMod.Status = "Connected";
        }
    }
}