using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.GameGraphics.GamePieces
{
    public class DeckPieceXF : BaseGraphicsXF<DeckPieceCP>
    {
        public static readonly BindableProperty SuitProperty = BindableProperty.Create(propertyName: "Suit", returnType: typeof(EnumSuitList), declaringType: typeof(DeckPieceXF), defaultValue: EnumSuitList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SuitPropertyChanged);
        public EnumSuitList Suit
        {
            get
            {
                return (EnumSuitList)GetValue(SuitProperty);
            }
            set
            {
                SetValue(SuitProperty, value);
            }
        }
        private static void SuitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (DeckPieceXF)bindable;
            thisItem.Mains.Suit = (EnumSuitList)newValue;
        }
    }
}