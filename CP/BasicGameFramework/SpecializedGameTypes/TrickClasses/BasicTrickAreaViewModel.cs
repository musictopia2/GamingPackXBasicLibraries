using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public abstract class BasicTrickAreaViewModel<S, T> : SimpleControlViewModel
        where S : Enum
        where T : class, ITrickCard<S>
    {
        public DeckObservableDict<T> CardList = new DeckObservableDict<T>();
        public T? WinCard;
        private readonly EventAggregator _thisE;
        public ControlCommand<T> CardSingleClickCommand { get; set; }
        public BasicTrickAreaViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            _thisE = thisMod.MainContainer!.Resolve<EventAggregator>();
            CardSingleClickCommand = new ControlCommand<T>(this, async thisCard =>
            {
                if (thisCard == null)
                    throw new BasicBlankException("Card can't be Nothing");
                if (CardList.IndexOf(thisCard) == -1)
                    throw new BasicBlankException("Can't be -1 for index");
                await ProcessCardClickAsync(thisCard);
            }, thisMod, thisMod.CommandContainer!);
        }
        protected abstract Task ProcessCardClickAsync(T thisCard);
        public int MaxPlayers { get; set; }
        protected async Task AnimateWinAsync()
        {
            if (WinCard == null)
                throw new BasicBlankException("Can't animate win because no card sent");
            await _thisE.PublishAsync(new AnimateTrickEventModel()); //hopefully this simple this time.
        }
        protected override void EnableChange()
        {
            CardSingleClickCommand.ReportCanExecuteChange();
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        public void TradeCard(int index, T newCard)
        {
            SKPoint oldLocation;
            oldLocation = CardList[index].Location;
            newCard.Location = oldLocation;
            newCard.IsSelected = false;
            newCard.Drew = false;
            var oldCard = CardList[index];
            CardList.ReplaceItem(oldCard, newCard); // hopefully that simple.
        }
    }
}