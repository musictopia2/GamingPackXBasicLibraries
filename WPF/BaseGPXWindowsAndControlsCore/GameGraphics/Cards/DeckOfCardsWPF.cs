using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.GameGraphicsCP.Cards;
using BasicGameFramework.RegularDeckOfCards;
using System.Windows;
using System.Windows.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.GameGraphics.Cards
{
    public class DeckOfCardsWPF<R> : BaseDeckGraphicsWPF<R, DeckOfCardsCP>
        where R : IRegularCard, new()
    {
        public bool NeedsExtraSuitForSolitaire { get; set; } //if needed, the solitaire classes has to show its needed.
        public override void Init() // can't do anything this time.
        {
            MainObject!.DrawTopRight = NeedsExtraSuitForSolitaire;
        }
        public static readonly DependencyProperty DisplaySuitProperty = DependencyProperty.Register("DisplaySuit", typeof(EnumSuitList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(DisplaySuitPropertyChanged)));
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
        private static void DisplaySuitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            thisItem.MainObject!.DisplaySuit = (EnumSuitList)e.NewValue;
        }
        public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register("DisplayValue", typeof(EnumCardValueList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(DisplayValuePropertyChanged)));
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
        private static void DisplayValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            thisItem.MainObject!.DisplayValue = (EnumCardValueList)e.NewValue;
        }
        public static readonly DependencyProperty CardNumberProperty = DependencyProperty.Register("CardNumber", typeof(EnumCardValueList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardNumberPropertyChanged)));
        public EnumCardValueList CardNumber
        {
            get
            {
                return (EnumCardValueList)GetValue(CardNumberProperty);
            }
            set
            {
                SetValue(CardNumberProperty, value);
            }
        }
        private static void CardNumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            if ((int)e.NewValue == 1 || (int)e.NewValue == 14)
                thisItem.MainObject!.CardValue = EnumCardValueList.LowAce;
            else
                thisItem.MainObject!.CardValue = (EnumCardValueList)e.NewValue;
        }
        public static readonly DependencyProperty SuitValueProperty = DependencyProperty.Register("SuitValue", typeof(EnumSuitList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(SuitValuePropertyChanged)));
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
        private static void SuitValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            thisItem.MainObject!.SuitValue = (EnumSuitList)e.NewValue;
        }
        public static new readonly DependencyProperty MainColorProperty = DependencyProperty.Register("MainColor", typeof(EnumColorList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(MainColorPropertyChanged)));
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
        private static void MainColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            thisItem.MainObject!.MainColor = (EnumColorList)e.NewValue;
        }
        public static readonly DependencyProperty CardTypeValueProperty = DependencyProperty.Register("CardTypeValue", typeof(EnumCardTypeList), typeof(DeckOfCardsWPF<R>), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardTypeValuePropertyChanged)));
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
        private static void CardTypeValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (DeckOfCardsWPF<R>)sender;
            thisItem.MainObject!.CardTypeValue = (EnumCardTypeList)e.NewValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
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