using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.Messenging;

namespace BasicGameFrameworkLibrary.SolitaireClasses.PileObservable
{
    public class CustomMultiplePile : BasicMultiplePilesCP<SolitaireCard>
    {
        protected override bool CanAutoUnselect()
        {
            return false;
        }
        public CustomMultiplePile(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator) { }
    }
}