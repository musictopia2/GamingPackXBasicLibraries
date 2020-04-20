using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Newtonsoft.Json;
using SkiaSharp;
using System.Linq;
using static BasicGameFrameworkLibrary.DIContainers.Helpers;
namespace BasicGameFrameworkLibrary.RegularDeckOfCards
{
    public class RegularSimpleCard : SimpleDeckObject, IRegularCard //i think i can inherit from this.
    {
        private EnumCardValueList _value;
        public EnumCardValueList Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value)) { }
            }
        }
        private EnumSuitList _suit;
        public EnumSuitList Suit
        {
            get { return _suit; }
            set
            {
                if (SetProperty(ref _suit, value)) { }
            }
        }
        private EnumSuitList _displaySuit;
        public EnumSuitList DisplaySuit
        {
            get { return _displaySuit; }
            set
            {
                if (SetProperty(ref _displaySuit, value)) { }
            }
        }
        private EnumCardValueList _displayNumber;
        public EnumCardValueList DisplayNumber
        {
            get { return _displayNumber; }
            set
            {
                if (SetProperty(ref _displayNumber, value)) { }
            }
        }
        private EnumCardTypeList _cardType;
        public EnumCardTypeList CardType
        {
            get { return _cardType; }
            set
            {
                if (SetProperty(ref _cardType, value)) { }
            }
        }
        private EnumColorList _color;
        public EnumColorList Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value)) { }
            }
        }
        public int Points { get; set; } //points is common enough here that decided to go ahead and include here.
        public RegularSimpleCard()
        {
            DefaultSize = new SKSize(55, 72); //that is always default size.
        }
        public EnumSuitList GetSuit => Suit;
        public EnumColorList GetColor => Color;
        public int Section { get; private set; }

        protected static IRegularDeckInfo? ThisInfo; //use dependency injection to populate this.
        protected static IRegularAceCalculator? ThisAce;

        //public static Clear
        public static void ClearSavedList()
        {
            ThisInfo = null;
            ThisAce = null;
            _list.Clear();
        }

        [JsonIgnore]
        public IGamePackageResolver? MainContainer { get; set; }
        int ISimpleValueObject<int>.ReadMainValue => (int)Value;
        private static IRegularDeckWild? _thisWild;
        private void SetObjects()
        {
            if (MainContainer != null && ThisInfo != null)
                return;
            PopulateContainer(this);
            if (ThisInfo != null)
            {
                return;
            }    
            ThisInfo = MainContainer!.Resolve<IRegularDeckInfo>();
            ThisAce = MainContainer.Resolve<IRegularAceCalculator>();
            if (MainContainer.RegistrationExist<IRegularDeckWild>() == true)
                _thisWild = MainContainer.Resolve<IRegularDeckWild>(); //decided to do this way so i don't have to inherit just for the wild part.
        }
        public virtual bool IsObjectWild
        {
            get
            {
                SetObjects();
                if (ThisInfo!.UseJokers == false)
                    return false;
                if (_thisWild != null)
                    return _thisWild.IsWild(this);
                return Value == EnumCardValueList.Joker; //most of the time, only the jokers are wild.
            }
        }
        private bool CanUse(EnumSuitList thisSuit, int number)
        {
            if (ThisInfo!.ExcludeList.Count == 0)
                return true;
            return !(ThisInfo.ExcludeList.Any(ThisExclude =>
            {

                if (ThisExclude.Number == 1 || ThisExclude.Number == 14)
                {
                    if (number == 1 || number == 14)
                        return ThisExclude.Suit == thisSuit;
                    else
                        return false; //i think
                }
                else
                {
                    return ThisExclude.Number == number && ThisExclude.Suit == thisSuit;
                }
            }));
        }
        protected virtual void FinishPopulatingCard() { }
        protected virtual void PopulateAceValue() //for games like chinazo, i can do something else.  since i am already inheriting to add extras.
        {
            ThisAce!.PopulateAceValues(this);
        }

        //for regular deck of cards, populating has to be done completely differently now.

        //public static void ClearSavedList()
        //{
        //    _list.Clear();
        //}

        private static readonly DeckRegularDict<RegularSimpleCard> _list = new DeckRegularDict<RegularSimpleCard>();

        public void Populate(int chosen)
        {
            SetObjects();
            if (_list.Count == 0)
            {
                //await UIPlatform.ShowMessageAsync("Populating cards");
                GenerateList();
            }
            var item = _list.GetSpecificItem(chosen);
            Deck = chosen;
            Suit = item.Suit;
            Section = item.Section;
            Value = item.Value;
            Color = item.Color;
            CardType = item.CardType;
            FinishPopulatingCard();
        }

        private void GenerateList()
        {
            _list.Clear();
            int x, y, z, q = 0;
            for (x = 1; x <= ThisInfo!.HowManyDecks; x++)
            {
                for (y = 0; y < ThisInfo.SuitList.Count; y++)
                {
                    for (z = ThisInfo.LowestNumber; z <= ThisInfo.HighestNumber; z++)
                    {
                        if (CanUse(ThisInfo.SuitList[y], z))
                        {
                            q++;

                            RegularSimpleCard card = new RegularSimpleCard();
                            card.Deck = q;
                            card.Suit = ThisInfo.SuitList[y];
                            if (z == 1 || z == 14)
                                PopulateAceValue();
                            else
                            {
                                Value = (EnumCardValueList)z;
                            }
                            card.Value = Value;
                            card.Section = x;
                            card.CardType = EnumCardTypeList.Regular;
                            if (card.Suit == EnumSuitList.Clubs || card.Suit == EnumSuitList.Spades)
                            {
                                card.Color = EnumColorList.Black;
                            }
                            else
                            {
                                card.Color = EnumColorList.Red;
                            }
                            _list.Add(card);
                        }
                    }
                }
            }
            if (ThisInfo.UseJokers == true)
            {
                for (int r = 1; r <= ThisInfo.HowManyDecks; r++)
                {
                    for (int p = 1; p <= 2; p++)
                    {
                        q++;
                        RegularSimpleCard card = new RegularSimpleCard();
                        card.Value = EnumCardValueList.Joker;
                        card.Deck = q;
                        card.CardType = EnumCardTypeList.Joker;
                        if (p == 1)
                        {
                            card.Color = EnumColorList.Black;
                        }
                        else
                        {
                            card.Color = EnumColorList.Red;
                        }
                        card.Section = r;
                        _list.Add(card);
                    }
                }
                EnumColorList last = EnumColorList.Red;
                int s = ThisInfo.HowManyDecks;
                for (int i = 0; i < ThisInfo.GetExtraJokers; i++)
                {
                    q++;
                    if (last == EnumColorList.Red)
                        last = EnumColorList.Black;
                    else
                        last = EnumColorList.Red;
                    s++;
                    if (s > ThisInfo.HowManyDecks)
                    {
                        s = 1;
                    }
                    RegularSimpleCard card = new RegularSimpleCard();
                    card.Value = EnumCardValueList.Joker;
                    card.Deck = q;
                    card.CardType = EnumCardTypeList.Joker;
                    card.Section = s;
                    card.Color = last;
                    _list.Add(card);
                }
            }
        }

        //public void OldPopulate(int chosen) //the basics would already be done.
        //{
        //    SetObjects();
        //    int x, y, z, q = 0;
        //    for (x = 1; x <= ThisInfo!.HowManyDecks; x++)
        //    {
        //        for (y = 0; y < ThisInfo.SuitList.Count; y++)
        //        {
        //            for (z = ThisInfo.LowestNumber; z <= ThisInfo.HighestNumber; z++)
        //            {
        //                if (CanUse(ThisInfo.SuitList[y], z))
        //                {
        //                    q++;
        //                    if (q == chosen)
        //                    {
        //                        Deck = q;
        //                        Suit = ThisInfo.SuitList[y];
        //                        if (z == 1 || z == 14)
        //                            PopulateAceValue();
        //                        else
        //                        {
        //                            Value = (EnumCardValueList)z;
        //                        }
        //                        Section = x;
        //                        CardType = EnumCardTypeList.Regular;
        //                        if (Suit == EnumSuitList.Clubs || Suit == EnumSuitList.Spades)
        //                            Color = EnumColorList.Black;
        //                        else
        //                            Color = EnumColorList.Red;
        //                        FinishPopulatingCard(); //other cards can but don't have to finish populating it.
        //                        return; // because we have everything needed.
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (ThisInfo.UseJokers == true)
        //    {
        //        for (int r = 1; r <= ThisInfo.HowManyDecks; r++)
        //        {
        //            for (int p = 1; p <= 2; p++)
        //            {
        //                q++;
        //                if (q == chosen)
        //                {
        //                    Value = EnumCardValueList.Joker;
        //                    Deck = chosen;
        //                    CardType = EnumCardTypeList.Joker;
        //                    Section = r;
        //                    if (p == 1)
        //                        Color = EnumColorList.Black;
        //                    else
        //                        Color = EnumColorList.Red;
        //                    FinishPopulatingCard();
        //                    return;
        //                }
        //            }
        //        }
        //        EnumColorList last = EnumColorList.Red;
        //        int s = ThisInfo.HowManyDecks;
        //        for (int i = 0; i < ThisInfo.GetExtraJokers; i++)
        //        {
        //            q++;
        //            if (last == EnumColorList.Red)
        //                last = EnumColorList.Black;
        //            else
        //                last = EnumColorList.Red;
        //            s++;
        //            if (s > ThisInfo.HowManyDecks)
        //                s = 1;
        //            if (q == chosen)
        //            {
        //                Value = EnumCardValueList.Joker;
        //                Deck = chosen;
        //                CardType = EnumCardTypeList.Joker;
        //                Section = s;
        //                Color = last;
        //                FinishPopulatingCard();
        //                return;
        //            }
        //        }
        //    }
        //    throw new BasicBlankException($"Cannot find the card with the deck {chosen} The number of decks is {ThisInfo.HowManyDecks}");
        //}
        public void Reset() //not sure about this part (?)
        {
            DisplayNumber = EnumCardValueList.None;
            DisplaySuit = EnumSuitList.None;
        }
        public override string ToString()
        {
            if (Deck == 0)
                throw new BasicBlankException("Deck cannot be 0 when populating the tostring");
            if (Value == EnumCardValueList.Joker)
                return $"{Color} Joker";
            if (Value == EnumCardValueList.Continue)
                return "Continue";
            if (Value == EnumCardValueList.Stop)
                return "Stop";
            EnumSuitList tempSuit;
            if (DisplaySuit == EnumSuitList.None)
                tempSuit = Suit;
            else
                tempSuit = DisplaySuit;
            EnumCardValueList tempValue;
            if (DisplayNumber == EnumCardValueList.None)
                tempValue = Value;
            else
                tempValue = DisplayNumber;
            string stringValue;
            if (tempValue == EnumCardValueList.HighAce || tempValue == EnumCardValueList.LowAce)
                stringValue = "A";
            else
                stringValue = tempValue.ToString();
            var stringSuit = tempSuit switch
            {
                EnumSuitList.None => throw new BasicBlankException("Suit Can't be None"),
                EnumSuitList.Clubs => "C♣︎",
                EnumSuitList.Diamonds => "D♦︎",
                EnumSuitList.Spades => "S♠",
                EnumSuitList.Hearts => "H♥",
                _ => throw new BasicBlankException("Not Supported"),
            };
            return $"{stringValue} {stringSuit} Deck {Deck}";
        }
    }
}