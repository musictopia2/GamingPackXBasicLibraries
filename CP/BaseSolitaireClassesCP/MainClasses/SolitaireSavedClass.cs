﻿using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.DrawableListsViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BaseSolitaireClassesCP.MainClasses
{
    public class SolitaireSavedClass : ObservableObject //decided to have generics now.  decided to inherit from observable object after all for games like persian.
    {
        public int Score { get; set; } //try this way.
        public SavedDiscardPile<SolitaireCard> Discard { get; set; } = new SavedDiscardPile<SolitaireCard>(); //i think this is fine.
        public string MainPileData { get; set; } = "";
        public SavedWaste WasteData { get; set; } = new SavedWaste();
        public CustomBasicList<int> IntDeckList { get; set; } = new CustomBasicList<int>();
    }
}