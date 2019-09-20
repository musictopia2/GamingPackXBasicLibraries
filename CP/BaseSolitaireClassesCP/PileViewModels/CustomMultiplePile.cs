using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.ViewModelInterfaces;
namespace BaseSolitaireClassesCP.PileViewModels
{
    public class CustomMultiplePile : BasicMultiplePilesCP<SolitaireCard>
    {
        protected override bool CanAutoUnselect()
        {
            return false;
        }
        public CustomMultiplePile(IBasicGameVM thisMod) : base(thisMod) { }
    }
}