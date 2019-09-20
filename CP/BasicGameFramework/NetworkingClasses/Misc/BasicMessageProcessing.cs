using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.NetworkingClasses.Data;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ms = BasicGameFramework.MultiplayerClasses.InterfaceMessages;
namespace BasicGameFramework.NetworkingClasses.Misc
{
    public class BasicMessageProcessing : IMessageProcessor
    {
        private readonly ISimpleUI _thisBox;
        private readonly IGamePackageResolver _thisContainer;
        private readonly TestOptions _thisTest;
        public BasicMessageProcessing(ISimpleUI thisBox, IGamePackageResolver thisContainer, TestOptions thisTest) //we need the isimpleui for the errors if any.
        {
            _thisBox = thisBox;
            _thisContainer = thisContainer;
            _thisTest = thisTest;
        }
        public Task ProcessErrorAsync(string errorMessage) //done.
        {
            _thisBox.ShowError(errorMessage);
            return Task.CompletedTask;
        }
        public async Task ProcessMessageAsync(SentMessage thisMessage)
        {
            try
            {
                switch (thisMessage.Status.ToLower())
                {
                    case "ready":
                        ms.IReadyNM thisReady = _thisContainer.Resolve<ms.IReadyNM>();
                        await thisReady.ProcessReadyAsync(thisMessage.Body);
                        break;
                    case "loadgame":
                        ms.ILoadGameNM thisLoad = _thisContainer.Resolve<ms.ILoadGameNM>();
                        await thisLoad.LoadGameAsync(thisMessage.Body);
                        break;
                    case "savedgame":
                        _thisBox.ShowError("savedgame is obsolete.  Try to use loadgame now");
                        break;
                    case "reshuffledcards":
                        ms.IReshuffledCardsNM thisReshuffle = _thisContainer.Resolve<ms.IReshuffledCardsNM>();
                        await thisReshuffle.ReshuffledCardsReceived(thisMessage.Body);
                        break;
                    case "newgame":
                        ms.INewGameNM thisGame = _thisContainer.Resolve<ms.INewGameNM>();
                        await thisGame.NewGameReceivedAsync(thisMessage.Body);
                        break;
                    case "restoregame":
                        IRestoreMultiPlayerGame thisRestore = _thisContainer.Resolve<IRestoreMultiPlayerGame>();
                        await thisRestore.RestoreMessageAsync(thisMessage.Body);
                        break;
                    case "endturn":
                        ms.IEndTurnNM thisEnd = _thisContainer.Resolve<ms.IEndTurnNM>();
                        await thisEnd.EndTurnReceivedAsync(thisMessage.Body);
                        break;
                    case "drawcard":
                        ms.IDrawCardNM thisDrawCard = _thisContainer.Resolve<ms.IDrawCardNM>();
                        await thisDrawCard.DrawCardReceivedAsync(thisMessage.Body);
                        break;
                    case "chosepiece":
                        ms.IChoosePieceNM thisPiece = _thisContainer.Resolve<ms.IChoosePieceNM>();
                        await thisPiece.ChoosePieceReceivedAsync(thisMessage.Body);
                        break;
                    case "pickup":
                        ms.IPickUpNM thisPick = _thisContainer.Resolve<ms.IPickUpNM>();
                        await thisPick.PickUpReceivedAsync(thisMessage.Body);
                        break;
                    case "discard":
                        ms.IDiscardNM thisDicard = _thisContainer.Resolve<ms.IDiscardNM>();
                        await thisDicard.DiscardReceivedAsync(thisMessage.Body);
                        break;
                    case "move":
                        ms.IMoveNM thisMove = _thisContainer.Resolve<ms.IMoveNM>();
                        await thisMove.MoveReceivedAsync(thisMessage.Body);
                        break;
                    case "rolled":
                        ms.IRolledNM thisRoll = _thisContainer.Resolve<ms.IRolledNM>();
                        await thisRoll.RollReceivedAsync(thisMessage.Body);
                        break;
                    case "processhold":
                        ms.IProcessHoldNM thisHold = _thisContainer.Resolve<ms.IProcessHoldNM>();
                        await thisHold.ProcessHoldReceivedAsync(int.Parse(thisMessage.Body)); //if you send something not int, will get casting errors.
                        break;
                    case "selectdice":
                        ms.ISelectDiceNM thisDice = _thisContainer.Resolve<ms.ISelectDiceNM>();
                        await thisDice.SelectDiceReceivedAsync(int.Parse(thisMessage.Body));
                        break;
                    case "drew":
                        _thisBox.ShowError("drew is obsolete.  Try drawdomino");
                        break;
                    case "drawdomino":
                        ms.IDrewDominoNM thisDomino = _thisContainer.Resolve<ms.IDrewDominoNM>();
                        await thisDomino.DrewDominoReceivedAsync(int.Parse(thisMessage.Body));
                        break;
                    case "playdomino":
                        ms.IPlayDominoNM playDomino = _thisContainer.Resolve<ms.IPlayDominoNM>();
                        await playDomino.PlayDominoMessageAsync(int.Parse(thisMessage.Body));
                        break;
                    case "trickplay":
                        ms.ITrickNM thisTrick = _thisContainer.Resolve<ms.ITrickNM>();
                        await thisTrick.TrickPlayReceivedAsync(int.Parse(thisMessage.Body));
                        break;
                    default:
                        ms.IMiscDataNM thisMisc = _thisContainer.Resolve<ms.IMiscDataNM>();
                        await thisMisc.MiscDataReceived(thisMessage.Status, thisMessage.Body);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_thisTest.ShowErrorMessageBoxes == true)
                    _thisBox.ShowError(ex.Message); //which will close out but at least you get a messagebox.
                else
                    throw ex;
            }
        }
    }
}