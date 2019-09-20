using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.Dice
{
    /// <summary>
    /// this is intended for standard dice.  try to make it work with colored dice.
    /// however, in order to make this one easier, its used when the tag is rolled.
    /// this will implement the interface for rolled for the message.
    /// maybe use generics.  however, it has to be related to standard dice (even risk dice should qualify).
    /// was going to be simpledice but instead instead be IStandardDice.
    /// </summary>
    public class StandardRollProcesses<D, P> : IRolledNM, ISelectDiceNM
        where D : IStandardDice, new()
        where P : class, IPlayerItem, new()
    {
        private readonly bool _moreRollProcs;
        private readonly IAdditionalRollProcess? _objMoreRoll;
        public StandardRollProcesses(IStandardRoller<D, P> thisGame)
        {
            ThisGame = thisGame;
            _moreRollProcs = thisGame.MainContainer.ObjectExist<IAdditionalRollProcess>();
            if (_moreRollProcs == true)
                _objMoreRoll = thisGame.MainContainer.Resolve<IAdditionalRollProcess>();
        }
        public IStandardRoller<D, P> ThisGame;
        public int HowManySections { get; set; } = 6;
        public async Task RollReceivedAsync(string data)
        {
            CustomBasicList<CustomBasicList<D>> thisList = await ThisGame.ThisCup!.GetDiceList(data);
            await RollDiceAsync(thisList);
        }
        public async Task SelectUnSelectDiceAsync(int id)
        {
            if (ThisGame.SingleInfo!.CanSendMessage(ThisGame.ThisData!) == true)
                await ThisGame.ThisNet!.SendAllAsync("selectdice", id);
            ThisGame.ThisCup!.SelectUnselectDice(id);
            await ThisGame.AfterSelectUnselectDiceAsync();
        }
        public async Task SelectDiceReceivedAsync(int id)
        {
            await SelectUnSelectDiceAsync(id);
        }
        public async Task RollDiceAsync()
        {
            if (ThisGame.ThisCup!.HowManyDice == 0)
                throw new BasicBlankException("Can't have 0 dice.  This means forgot to set the number of dice in the view models");
            if (_objMoreRoll != null && _moreRollProcs == true)
            {
                if (await _objMoreRoll.CanRollAsync() == false)
                    return;
                await _objMoreRoll.BeforeRollingAsync();
            }
            var ThisCol = ThisGame.ThisCup.RollDice(HowManySections);
            if (ThisGame.SingleInfo!.CanSendMessage(ThisGame.ThisData!) == true)
                await ThisGame.ThisCup.SendMessageAsync(ThisCol);
            await RollDiceAsync(ThisCol);
        }
        private async Task RollDiceAsync(CustomBasicList<CustomBasicList<D>> ThisList)
        {
            await ThisGame.ThisCup!.ShowRollingAsync(ThisList);
            await ThisGame.AfterRollingAsync();
        }
    }
}