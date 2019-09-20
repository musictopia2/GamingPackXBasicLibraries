using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.GameGraphics.GamePieces
{
    public class ListPieceXF : BaseGraphicsXF<ListViewPieceCP>
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(string), declaringType: typeof(ListPieceXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextPropertyChanged);
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (ListPieceXF)bindable;
            thisItem.Mains.DisplayText = (string)newValue;
        }
        public static readonly BindableProperty IndexProperty = BindableProperty.Create(propertyName: "Index", returnType: typeof(int), declaringType: typeof(ListPieceXF), defaultValue: -1, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IndexPropertyChanged);
        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (ListPieceXF)bindable;
            thisItem.Mains.Index = (int)newValue;
        }
        public static readonly BindableProperty CanHighlightProperty = BindableProperty.Create(propertyName: "CanHighlight", returnType: typeof(bool), declaringType: typeof(ListPieceXF), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanHighlightPropertyChanged);
        public bool CanHighlight
        {
            get
            {
                return (bool)GetValue(CanHighlightProperty);
            }
            set
            {
                SetValue(CanHighlightProperty, value);
            }
        }
        private static void CanHighlightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (ListPieceXF)bindable;
            thisItem.Mains.CanHighlight = (bool)newValue;
        }
        public void UseSmallerSize()
        {
            Mains.SmallerFontSize = true; // just a method should be fine.
        }
    }
}