using BaseSolitaireClassesCP.Cards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BaseSolitaireClassesCP.TriangleClasses
{
    public class SavedTriangle
    {
        public bool InPlay { get; set; }
        public CustomBasicList<SolitaireCard> CardList { get; set; } = new CustomBasicList<SolitaireCard>(); //for saved, should be no need for observable.  if i change my mind, rethink.
    }
}