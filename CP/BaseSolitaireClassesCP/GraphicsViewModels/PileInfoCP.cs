using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BaseSolitaireClassesCP.GraphicsViewModels
{
    public class PileInfoCP : ObservableObject
    {
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value)) { }
            }
        }
        public DeckRegularDict<SolitaireCard> TempList = new DeckRegularDict<SolitaireCard>(); //this is needed because otherwise the piles has performance problems.
        public DeckObservableDict<SolitaireCard> CardList = new DeckObservableDict<SolitaireCard>();
    }
}