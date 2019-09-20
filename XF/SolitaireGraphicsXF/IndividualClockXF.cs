using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.ClockClasses;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SolitaireGraphicsXF
{
    public class IndividualClockXF : ContentView, IHandle<CurrentCardEventModel>
    {
        private ClockViewModel? _parentMod;
        private ClockInfo? _thisMod;
        private DeckOfCardsXF<SolitaireCard>? _graphicsCard;
        private DeckObservableDict<SolitaireCard>? _cardList;
        private SolitaireCard? _display;
        private Binding GetCommandBinding(string path)
        {
            Binding output = new Binding(path);
            output.Source = _parentMod;
            return output;
        }
        private void LinkUpCard()
        {
            _graphicsCard!.BindingContext = null; //may need this first now.
            _graphicsCard.SendSize(ts.TagUsed, _display!); //hopefully this is fine now.
            _display!.IsSelected = _thisMod!.IsSelected;
            _display.IsEnabled = _thisMod.IsEnabled;
            _display.IsUnknown = true; //default to this.

            if (_parentMod!.CurrentCard != null && _parentMod.CurrentCard.Deck == _display.Deck)
                _display.IsUnknown = false;
            else if (_parentMod.ShowCenter && _cardList!.Count == 1)
            {
                _display.CardType = EnumCardTypeList.Stop;
                _display.IsUnknown = false;
            }
            else if (_parentMod.ShowCenter)
                _display.IsUnknown = true;
            else
                _display.IsUnknown = false;
            _graphicsCard.CommandParameter = _display;
            var thisBind = GetCommandBinding(nameof(ClockViewModel.ClockCommand));
            _graphicsCard.SetBinding(DeckOfCardsXF<SolitaireCard>.CommandProperty, thisBind);
        }
        public void Init(ClockInfo thisClock, ClockViewModel parentMod)
        {
            _cardList = thisClock.CardList;
            if (_cardList.Count == 0)
                throw new BasicBlankException("Must have at least one card always.  Otherwise rethinking required");
            _thisMod = thisClock;
            _parentMod = parentMod;
            StackLayout thisStack = new StackLayout();
            BindingContext = _thisMod;
            _graphicsCard = new DeckOfCardsXF<SolitaireCard>();
            _display = _cardList.Last();
            LinkUpCard(); //hopefully that somehow works.
            Label defaultLabel;
            if (parentMod.ShowCenter)
            {
                defaultLabel = GetDefaultLabel();
                defaultLabel.SetBinding(Label.TextProperty, new Binding(nameof(ClockInfo.NumberGuide)));
                defaultLabel.HorizontalOptions = LayoutOptions.Center;
                thisStack.Children.Add(defaultLabel);
            }
            thisStack.Children.Add(_graphicsCard);
            defaultLabel = GetDefaultLabel();
            defaultLabel.SetBinding(Label.TextProperty, new Binding(nameof(ClockInfo.LeftGuide)));
            defaultLabel.HorizontalOptions = LayoutOptions.Center;
            thisStack.Children.Add(defaultLabel);
            EventAggregator thisE = Resolve<EventAggregator>();
            _thisMod.PropertyChanged += PropertyChange;
            _cardList.CollectionChanged += CollectionChange;
            thisE.Subscribe(this);
            Content = thisStack;
        }
        private void CollectionChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_cardList!.Count != 0)
            {
                _display = _cardList.Last();
                LinkUpCard();
            }
        }
        private void PropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClockInfo.IsSelected))
                _display!.IsSelected = _thisMod!.IsSelected;
            if (e.PropertyName == nameof(ClockInfo.IsEnabled))
                _display!.IsEnabled = _thisMod!.IsEnabled;
        }
        public void Handle(CurrentCardEventModel message)
        {
            if (message.ThisClock!.Equals(_thisMod))
            {
                if (message.ThisCategory == EnumCardMessageCategory.Known)

                    _display = _parentMod!.CurrentCard; //i think
                else
                    _display = _cardList.Last();
                LinkUpCard();
            }
        }
    }
}