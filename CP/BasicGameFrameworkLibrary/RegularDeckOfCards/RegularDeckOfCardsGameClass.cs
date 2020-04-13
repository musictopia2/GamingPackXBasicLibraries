using BasicGameFrameworkLibrary.DrawableListsObservable;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.RegularDeckOfCards
{
    public abstract class RegularDeckOfCardsGameClass<R> where R : IRegularCard, new() //needs to be this way still because the cribbage one needs the rummy ones.
    {
        public RegularCardsBasicShuffler<R> DeckList; //decided to use the regularcardsbasicshuffler.
        private bool _opened;
        protected DeckObservablePile<R>? DeckPile;
        //hopefully this simple.
        //since this only needs the deckpile, and you do it on new game, then maybe no problem (?)
        public virtual async Task NewGameAsync(DeckObservablePile<R> deck)
        {
            DeckPile = deck;
            if (_opened == false && await CanOpenSavedSinglePlayerGameAsync())
            {
                _opened = true; //so when going to new game each time, won't keep restoring if you choose new game.
                await OpenSavedGameAsync();
                return;
            }
            ShuffleCards();
        }
        protected abstract void AfterShuffle();
        private void ShuffleCards() // the basics should not worry about how to handle the clicking.  besides, since its shared, something else can call it and do what it wants.
        {
            DeckList.ShuffleObjects();
            DeckPile!.OriginalList(DeckList);
            //_thisMod.DeckPile.Visible = true; // should be made visible most of the time.
            AfterShuffle();
        }
        public abstract Task<bool> CanOpenSavedSinglePlayerGameAsync();
        public abstract Task OpenSavedGameAsync(); //since you have to do the entire part of the autoresume, then no need for after loading saved game.
        protected virtual void AfterLoadingBasicControls() { }
        public RegularDeckOfCardsGameClass()
        {
            DeckList = new RegularCardsBasicShuffler<R>(); //i think this should be fine.
        }
    }
}