using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using System;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.GameGraphics.GamePieces
{
    public class MarblePiecesXF<E> : BaseGraphicsXF<MarblePiecesCP<E>> where E : struct, Enum
    {
        public static readonly BindableProperty UseTroubleProperty = BindableProperty.Create(propertyName: "UseTrouble", returnType: typeof(bool), declaringType: typeof(MarblePiecesXF<E>), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: UseTroublePropertyChanged);
        public bool UseTrouble
        {
            get
            {
                return (bool)GetValue(UseTroubleProperty);
            }
            set
            {
                SetValue(UseTroubleProperty, value);
            }
        }
        private static void UseTroublePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MarblePiecesXF<E>)bindable;
            thisItem.UseTrouble = (bool)newValue;
        }
    }
}