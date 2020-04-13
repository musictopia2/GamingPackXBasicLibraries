using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Generic;
namespace BasicGameFrameworkLibrary.BasicDrawables.Dictionary
{
    public class DeckObservableDict<D> : CustomBasicCollection<D>, IDeckDict<D>, IEnumerableDeck<D> where D : IDeckObject
    {
        private DictionaryBehavior<D>? _thisB;
        public DeckObservableDict() : base() { }
        public DeckObservableDict(IEnumerable<D> thisList) : base(thisList) { }
        protected override void LoadBehavior()
        {
            _thisB = new DictionaryBehavior<D>();
            Behavior = _thisB;
        }
        public D GetSpecificItem(int deck)
        {
            return _thisB!.SearchItem(deck);
        }
        public void ReplaceDictionary(int oldValue, int deck, D newValue)
        {
            _thisB!.ReplaceDictionaryValue(oldValue, deck, newValue);
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