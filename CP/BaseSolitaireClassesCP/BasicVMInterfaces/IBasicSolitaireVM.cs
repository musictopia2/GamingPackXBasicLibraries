using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileInterfaces;
using BasicGameFramework.DrawableListsViewModels;
namespace BaseSolitaireClassesCP.BasicVMInterfaces
{
    public interface IBasicSolitaireVM : IBasicScoreVM
    {
        DeckViewModel<SolitaireCard>? DeckPile { get; set; } //i think
        PileViewModel<SolitaireCard>? MainDiscardPile { get; set; }
        IWaste? WastePiles1 { get; set; }
        IMain? MainPiles1 { get; set; }
        bool CanStartNewGameImmediately { get; set; } //games like spider solitiare, can't start right away
    }
}