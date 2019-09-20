using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.RegularDeckOfCards
{
    public abstract class RegularDeckOfCardsGameClass<R> where R : IRegularCard, new() //needs to be this way still because the cribbage one needs the rummy ones.
    {
        private readonly ISoloCardGameVM<R> _thisMod;
        public RegularCardsBasicShuffler<R> DeckList; //decided to use the regularcardsbasicshuffler.
        private bool _opened;
        public virtual async Task NewGameAsync()
        {
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
            _thisMod.DeckPile!.OriginalList(DeckList);
            _thisMod.DeckPile.Visible = true; // should be made visible most of the time.
            AfterShuffle();
        }
        public abstract Task<bool> CanOpenSavedSinglePlayerGameAsync();
        public abstract Task OpenSavedGameAsync(); //since you have to do the entire part of the autoresume, then no need for after loading saved game.
        protected virtual void AfterLoadingBasicControls() { }
        public RegularDeckOfCardsGameClass(ISoloCardGameVM<R> thisMod)
        {
            _thisMod = thisMod;
            DeckList = new RegularCardsBasicShuffler<R>(); //i think this should be fine.
        }
    }
}