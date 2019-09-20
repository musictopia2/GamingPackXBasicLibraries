using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.BasicGameDataClasses
{
    /// <summary>
    /// this is used for card games.  this is all the data that has to be populated to change behaviors based on information.
    /// </summary>
    public interface ICardInfo<D> where D : IDeckObject, new()
    {
        int CardsToPassOut { get; }
        CustomBasicList<int> ExcludeList { get; } //this is needed if some cards can't be in the discard face up to begin with.
        bool AddToDiscardAtBeginning { get; }
        bool ReshuffleAllCardsFromDiscard { get; }
        bool ShowMessageWhenReshuffling { get; } //i think read only
        bool PassOutAll { get; }
        bool PlayerGetsCards { get; }
        bool NoPass { get; }
        bool NeedsDummyHand { get; }
        DeckObservableDict<D> DummyHand { get; set; } //unfortunately, i need it after all.  because this is used when passing out cards.
        bool HasDrawAnimation { get; } //can have a class for default stuff.  but you can adjust as needed.
        bool CanSortCardsToBeginWith { get; } //some games can't sort cards to begin with.
    }
}