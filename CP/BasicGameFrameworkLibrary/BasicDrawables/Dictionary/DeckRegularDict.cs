using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Generic;
namespace BasicGameFrameworkLibrary.BasicDrawables.Dictionary
{
    public class DeckRegularDict<D> : CustomBasicList<D>, IDeckDict<D>, IEnumerableDeck<D> where D : IDeckObject
    {
        private DictionaryBehavior<D>? _thisB;
        public DeckRegularDict() : base() { }
        public DeckRegularDict(IEnumerable<D> thisList) : base(thisList) { }
        protected override void LoadBehavior()
        {
            _thisB = new DictionaryBehavior<D>();
            Behavior = _thisB;
        }
        public D GetSpecificItem(int deck)
        {
            return _thisB!.SearchItem(deck);
        }
        public bool ObjectExist(int deck)
        {
            return _thisB!.ObjectExist(deck);
        }
        public D RemoveObjectByDeck(int deck) //i think
        {
            D thisCard = GetSpecificItem(deck);
            RemoveSpecificItem(thisCard);
            return thisCard;
        }
        

       
    }
}