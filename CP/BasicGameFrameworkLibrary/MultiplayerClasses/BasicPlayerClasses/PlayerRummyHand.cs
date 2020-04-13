using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses
{
    public class PlayerRummyHand<D> : PlayerSingleHand<D>, IPlayerRummyHand<D>, IHandle<UpdateCountEventModel>
        where D : IDeckObject, new()
    {
        public DeckRegularDict<D> AdditionalCards { get; set; } = new DeckRegularDict<D>(); //taking a risk.  hopefully it pays off.

        protected override int GetTotalObjectCount => base.GetTotalObjectCount + _tempCards;

        private int _tempCards;

        public void Handle(UpdateCountEventModel message)
        {
            _tempCards = message.ObjectCount;
            ObjectCount = GetTotalObjectCount;
        }
    }
}
