using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using System;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public interface ITrickDummyHand<SU, TR> //only when possible dummy does this have to be used.
        where SU : Enum
        where TR : ITrickCard<SU>, new()
    {
        DeckObservableDict<TR> GetCurrentHandList(); //i think
        int CardSelected(); //in a case of dummy, has to figure out which card is actually selected.  otherwise, its from your hand.
        void RemoveCard(int deck); //this will only handle the removing of the card.
    }
}