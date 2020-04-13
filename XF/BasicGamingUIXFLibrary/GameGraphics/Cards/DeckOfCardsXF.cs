using BasicGameFrameworkLibrary.GameGraphicsCP.Cards;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.GameGraphics.Cards
{
    public class DeckOfCardsXF<R> : BaseDeckGraphicsXF<R, DeckOfCardsCP>
        where R : IRegularCard, new()
    {
        public bool NeedsExtraSuitForSolitaire { get; set; } //if needed, the solitaire classes has to show its needed.
        public override void Init() // can't do anything this time.
        {
            MainObject!.DrawTopRight = NeedsExtraSuitForSolitaire;
        }
        public static readonly BindableProperty CardNumberProperty = BindableProperty.Create(propertyName: "CardNumber", defaultValue: 0, returnType: typeof(int), declaringType: typeof(DeckOfCardsXF<R>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardNumberPropertyChanged);
        public int CardNumber
        {
            get
            {
                return (int)GetValue(CardNumberProperty);
            }
            set
            {
                SetValue(CardNumberProperty, value);
            }
        }// had to do integer because one type was corrupted.  would probably be corrupted for mobile as well
        private static void CardNumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckOfCardsXF<R>)bindable;
            if ((int)newValue == 1 || (int)newValue == 14)
                thisItem.MainObject!.CardValue = EnumCardValueList.LowAce;
            else
                thisItem.MainObject!.CardValue = (EnumCardValueList)newValue;
        }
        public static readonly BindableProperty DisplayValueProperty = BindableProperty.Create(propertyName: "DisplayValue", returnType: typeof(EnumCardValueList), declaringType: typeof(DeckOfCardsXF<R>), defaultValue: EnumCardValueList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DisplayValuePropertyChanged);
        public EnumCardValueList DisplayValue
        {
            get
            {
                return (EnumCardValueList)GetValue(DisplayValueProperty);
            }
            set
            {
                SetValue(DisplayValueProperty, value);
            }
        }
        private static void DisplayValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ThisItem = (DeckOfCardsXF<R>)bindable;
            ThisItem.MainObject!.DisplayValue = (EnumCardValueList)newValue;
        }
        public static readonly BindableProperty DisplaySuitProperty = BindableProperty.Create(propertyName: "DisplaySuit", returnType: typeof(EnumSuitList), declaringType: typeof(DeckOfCardsXF<R>), defaultValue: EnumSuitList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DisplaySuitPropertyChanged);
        public EnumSuitList DisplaySuit
        {
            get
            {
                return (EnumSuitList)GetValue(DisplaySuitProperty);
            }
            set
            {
                SetValue(DisplaySuitProperty, value);
            }
        }
        private static void DisplaySuitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckOfCardsXF<R>)bindable;
            thisItem.MainObject!.DisplaySuit = (EnumSuitList)newValue;
        }
        public static readonly BindableProperty SuitValueProperty = BindableProperty.Create(propertyName: "SuitValue", defaultValue: EnumSuitList.None, returnType: typeof(EnumSuitList), declaringType: typeof(DeckOfCardsXF<R>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: SuitValuePropertyChanged);
        public EnumSuitList SuitValue
        {
            get
            {
                return (EnumSuitList)GetValue(SuitValueProperty);
            }
            set
            {
                SetValue(SuitValueProperty, value);
            }
        }
        private static void SuitValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckOfCardsXF<R>)bindable;
            thisItem.MainObject!.SuitValue = (EnumSuitList)newValue;
        }
        public static new readonly BindableProperty MainColorProperty = BindableProperty.Create(propertyName: "MainColor", defaultValue: EnumColorList.None, returnType: typeof(EnumColorList), declaringType: typeof(DeckOfCardsXF<R>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: MainColorPropertyChanged);
        public new EnumColorList MainColor
        {
            get
            {
                return (EnumColorList)GetValue(MainColorProperty);
            }
            set
            {
                SetValue(MainColorProperty, value);
            }
        }
        private static void MainColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckOfCardsXF<R>)bindable;
            thisItem.MainObject!.MainColor = (EnumColorList)newValue;
        }
        public static readonly BindableProperty CardTypeValueProperty = BindableProperty.Create(propertyName: "CardTypeValue", defaultValue: EnumCardTypeList.None, returnType: typeof(EnumCardTypeList), declaringType: typeof(DeckOfCardsXF<R>), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardTypeValuePropertyChanged);
        public EnumCardTypeList CardTypeValue
        {
            get
            {
                return (EnumCardTypeList)GetValue(CardTypeValueProperty);
            }
            set
            {
                SetValue(CardTypeValueProperty, value);
            }
        }
        private static void CardTypeValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckOfCardsXF<R>)bindable;
            thisItem.MainObject!.CardTypeValue = (EnumCardTypeList)newValue;
        }
        protected override void PopulateInitObject()
        {
            if (MainObject == null)
                throw new BasicBlankException("Needs to create main object before you can init.  Rethink");
            MainObject.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardNumberProperty, new Binding(nameof(IRegularCard.Value))); //not working
            SetBinding(SuitValueProperty, new Binding(nameof(IRegularCard.Suit)));
            SetBinding(MainColorProperty, new Binding(nameof(IRegularCard.Color)));
            SetBinding(CardTypeValueProperty, new Binding(nameof(IRegularCard.CardType)));
            SetBinding(DisplaySuitProperty, new Binding(nameof(IRegularCard.DisplaySuit)));
            SetBinding(DisplayValueProperty, new Binding(nameof(IRegularCard.DisplayNumber)));
        }
    }
}
