using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Linq; //sometimes i do use linq.
using System.Threading.Tasks;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using System;
using System.Reflection;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.DIContainers;

namespace BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses
{
    public class ClockObservable : ObservableObject
    {
        public bool ShowCenter { get; set; }
        public CustomBasicList<ClockInfo>? ClockList;
        private SKSize _individualSize;
        public ISpecialSolitaireReposition? PositionUI;
        public SolitaireCard? CurrentCard;


        public PlainCommand ClockCommand { get; set; } //decided its not worth taking a risk on this one.
        //public PlainCommand<SolitaireCard> ClockCommand { get; set; }
        public void ClearBoard()
        {
            int decks = 10000;
            ClockList!.ForEach(thisClock =>
            {
                thisClock.CardList.Clear();
                var thisCard = new SolitaireCard();
                decks++;
                thisCard.Deck = decks; //because of how the dictionary works.
                thisClock.CardList.Add(thisCard);
                if (ShowCenter || ClockList.IndexOf(thisClock) + 1 <= 4)
                    thisClock.LeftGuide = 4;
                else
                    thisClock.LeftGuide = 3;
            });
        }
        public void AddCardToPile(int pile, SolitaireCard thisCard)
        {
            if (ShowCenter)
                throw new BasicBlankException("Cannot add card to pile because the goal is to get rid of cards");
            var thisClock = ClockList![pile];
            thisClock.LeftGuide--;
            thisClock.CardList.Add(thisCard);
        }
        public void RemoveCardFromPile(int pile)
        {
            if (ShowCenter == false)
                throw new BasicBlankException("Cannot remove card because the goal is to put cards to pile");
            var thisClock = ClockList![pile];
            thisClock.CardList.RemoveAt(1);
            thisClock.LeftGuide++;
            if (thisClock.CardList.Count == 1)
                thisClock.IsEnabled = false;
        }
        public bool HasCard(int pile)
        {
            var thisClock = ClockList![pile];
            return thisClock.CardList.Count > 1;
        }
        public void EnablePiles()
        {
            ClockList!.ForEach(thisClock =>
            {
                thisClock.IsEnabled = true;
                if (ShowCenter)
                    thisClock.LeftGuide = 5 - thisClock.CardList.Count;
                else
                    if (ShowCenter || ClockList.IndexOf(thisClock) + 1 <= 4)
                    thisClock.LeftGuide = 5 - thisClock.CardList.Count;
                else
                    thisClock.LeftGuide = 4 - thisClock.CardList.Count;
            });
        }
        public SolitaireCard GetLastCard(int pile)
        {
            ClockInfo thisClock = ClockList![pile];
            if (thisClock.CardList.Count < 2 && ShowCenter)
                throw new BasicBlankException("There are no cards to get");
            if (ShowCenter == false)
                return thisClock.CardList.Last();
            else
                return thisClock.CardList[1];
        }
        public CustomBasicList<ClockInfo> GetSavedClocks()
        {
            return ClockList!.ToCustomBasicList();
        }
        public virtual void LoadSavedClocks(CustomBasicList<ClockInfo> thisList)
        {
            ClockList = new CustomBasicList<ClockInfo>(thisList); //try this way.
            SolitaireCard tempCard = new SolitaireCard();
            _individualSize = tempCard.DefaultSize.GetSizeUsed(_thisP.Proportion);
            PositionClock(); //i think this too.
            if (PositionUI != null)
                PositionUI.RepositionCardsOnUI();
        }
        protected virtual async Task OnClockClickedAsync(int index)
        {
            await ThisMod.ClockClickedAsync(index);
        }
        protected virtual async Task ClickCurrentCardProcessAsync()
        {
            await Task.CompletedTask;
        }
        public void LoadBoard()
        {
            ClockList = new CustomBasicList<ClockInfo>();
            int maxs;
            if (ShowCenter)
                maxs = 13;
            else
                maxs = 12;
            SolitaireCard tempCard = new SolitaireCard();
            _individualSize = tempCard.DefaultSize.GetSizeUsed(_thisP.Proportion);
            int decks = 10000;
            maxs.Times(x =>
            {
                ClockInfo thisClock = new ClockInfo();
                if (ShowCenter)
                {
                    var thisCard = new SolitaireCard();
                    thisCard.Deck = decks;
                    decks++;
                    thisClock.CardList.Add(thisCard);
                    thisClock.NumberGuide = x;
                }
                if (ShowCenter || x <= 4)
                    thisClock.LeftGuide = 4;
                else
                    thisClock.LeftGuide = 3;
                ClockList.Add(thisClock);
            });
            PositionClock();
        }
        private void PositionClock()
        {
            float diffLeft = _individualSize.Width + 5;
            float diffTop = _individualSize.Height - 5;
            float startLeft = diffLeft * 4;
            float startTop = diffTop;
            float realLeft = startLeft;
            float realTop = startTop;
            int x = 0;
            ClockList!.ForEach(thisClock =>
            {
                x++;
                if (x == 9 && realLeft != 0)
                    throw new BasicBlankException("9 was supposed to be 0");
                if (x == 12 && realTop != 0)
                    throw new BasicBlankException("12 was supposed to be 0");
                thisClock.Location = new SKPoint(realLeft, realTop);
                if (x == 1 || x == 2)
                {
                    realTop += diffTop;
                    realLeft += diffLeft;
                }
                else if (x == 3 || x == 4 || x == 5)
                {
                    realLeft -= diffLeft;
                    realTop += diffTop;
                }
                else if (x == 6 || x == 7 || x == 8)
                {
                    realLeft -= diffLeft;
                    realTop -= diffTop;
                }
                else if (x == 9 || x == 10 || x == 11)
                {
                    realLeft += diffLeft;
                    realTop -= diffTop;
                    if (x == 11)
                        realTop = 0; //has to manually set for this case
                }
                else if (x == 12)
                    realTop = realLeft;
            });

            if (PositionUI != null)
                PositionUI.RepositionCardsOnUI();
        }
        protected IClockVM ThisMod; //if i need something else, can do new version like other games.
        private readonly IProportionImage _thisP;

        private async Task PrivateClickAsync(SolitaireCard card)
        {
            if (CurrentCard != null)
                if (CurrentCard.Deck == card.Deck)
                {
                    await ClickCurrentCardProcessAsync();
                    return;
                }
            foreach (var thisClock in ClockList!)
                if (thisClock.CardList.Last().Equals(card))
                {
                    await OnClockClickedAsync(ClockList.IndexOf(thisClock));
                    return;
                }
            throw new BasicBlankException("Found no card.  Rethink");
        }
        private bool CanPrivateClick(SolitaireCard card)
        {
            return card.IsEnabled == true && card.CardType != EnumCardTypeList.Stop;
        }

        public ClockObservable(IClockVM thisMod, CommandContainer command, IGamePackageResolver resolver)
        {
            ThisMod = thisMod;
            _thisP = resolver.Resolve<IProportionImage>(ts.TagUsed);
            MethodInfo method = this.GetPrivateMethod(nameof(PrivateClickAsync));
            MethodInfo fun = this.GetPrivateMethod(nameof(CanPrivateClick));
            ClockCommand = new PlainCommand(this, method, fun, command);
        }
    }
}