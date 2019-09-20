using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using nm = BasicGameFramework.MultiplayerClasses.InterfaceMessages;
namespace BasicGameFramework.BasicDrawables.MiscClasses
{
    /// <summary>
    /// This class is used for games that is not technically a card game but has cards like Candyland, or Sorry Board Game.
    /// Has routines for drawing, shuffling and reshuffling.
    /// </summary>
    public class DrawShuffleClass<D, P> : nm.IDrawCardNM, nm.IReshuffledCardsNM
        where D : class, IDeckObject, new() where P : class, IPlayerItem, new()
    {
        public ISavedCardList<D>? SaveRoot; //this is all it needs.
        private readonly IAfterDraw<P> _thisGame;
        private readonly IListShuffler<D> _deckList;
        public async Task DrawAsync()
        {
            if (SaveRoot!.CardList!.Count == 0)
            {
                if (_isBeginning == true)
                    throw new BasicBlankException("Should not already be reshuffling because its the beginning of the game");
                bool canSendMessage;
                canSendMessage = _thisGame.SingleInfo!.CanSendMessage(_thisGame.ThisData!);
                if (canSendMessage == true || _thisGame.ThisData!.MultiPlayer == false)
                {
                    await ReshuffleCardsAsync(canSendMessage);
                }
                else
                {
                    _thisGame.ThisCheck!.IsEnabled = true;
                }
                return;
            }
            SaveRoot.CurrentCard = SaveRoot.CardList.GetFirstObject(true);
            if (_isBeginning == false)
                await _thisGame.AfterDrawingAsync();
            else
                _isBeginning = false;
        }
        private async Task ReshuffleCardsAsync(bool canSend)
        {
            _deckList.ClearObjects();
            _deckList.ShuffleObjects();
            if (canSend == true)
            {
                CustomBasicList<int> newList = _deckList.ExtractIntegers(Items => Items.Deck);
                await _thisGame.ThisNet!.SendAllAsync("reshuffledcards", newList);
            }
            SaveRoot!.CardList = _deckList.ToRegularDeckDict();
            await AfterReshuffleAsync();
        }
        private async Task AfterReshuffleAsync()
        {
            await _thisGame.CurrentMod.ShowGameMessageAsync("Its the end of the deck; therefore; the cards are being reshuffled");
            await DrawAsync();
        }
        private bool _isBeginning;
        public async Task FirstShuffleAsync(bool canAutoDraw)
        {
            _deckList.ClearObjects(); //just in case.
            _deckList.ShuffleObjects();
            SaveRoot!.CardList = _deckList.ToRegularDeckDict();
            if (canAutoDraw == true)
            {
                _isBeginning = true;
                await DrawAsync();
            }
            else
            {
                SaveRoot.CurrentCard = new D(); //not sure if we need this (but could).
            }
        }
        public DrawShuffleClass(IAfterDraw<P> thisGame, IListShuffler<D> deckList) //main game can call this now from init (brand new from version 2).
        {
            _thisGame = thisGame;
            _deckList = deckList;
        }
        async Task nm.IDrawCardNM.DrawCardReceivedAsync(string data)
        {
            await DrawAsync();
        }
        async Task nm.IReshuffledCardsNM.ReshuffledCardsReceived(string data)
        {
            CustomBasicList<int> firstList = await js.DeserializeObjectAsync<CustomBasicList<int>>(data);
            if (_deckList.Count == 0)
                _deckList.OrderedObjects(); //maybe this was needed.  i think this is the best way to handle this situation.
            DeckObservableDict<D> NewList = firstList.GetNewObjectListFromDeckList(_deckList);
            SaveRoot!.CardList = NewList.ToRegularDeckDict();
            await AfterReshuffleAsync();
        }
    }
}