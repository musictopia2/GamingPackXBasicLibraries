using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFramework.GameGraphicsCP.GamePieces
{
    public class NumberPieceCP : BaseGraphicsCP, IEnumPiece<EnumCardValueList>, ISimpleValueObject<int>
    {
        private EnumCardValueList _CardValue;
        public EnumCardValueList CardValue
        {
            get { return _CardValue; }
            set
            {
                if (SetProperty(ref _CardValue, value))
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private int _NumberValue = -1; // even 0 can be a value
        public int NumberValue
        {
            get
            {
                return _NumberValue;
            }
            set
            {
                if (SetProperty(ref _NumberValue, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private bool _CanHighlight = true;
        public bool CanHighlight
        {
            get
            {
                return _CanHighlight;
            }
            set
            {
                if (SetProperty(ref _CanHighlight, value) == true)
                {
                    NeedsHighlighting = CanHighlight;
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private SKColor _TextColor = SKColors.Navy;
        public SKColor TextColor
        {
            get
            {
                return _TextColor;
            }
            set
            {
                if (SetProperty(ref _TextColor, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private readonly SKPaint _borderPaint;
        EnumCardValueList IEnumPiece<EnumCardValueList>.EnumValue { get => CardValue; set => CardValue = value; }
        int ISimpleValueObject<int>.ReadMainValue => NumberValue;
        public NumberPieceCP()
        {
            NeedsHighlighting = true; // this implies the other.
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); // that could be cool (well see)
        }
        protected virtual string GetValueToPrint() // so the overrided version can display something else.
        {
            if (NumberValue < 0 && CardValue == EnumCardValueList.None)
                return "";
            if (CardValue != EnumCardValueList.None)
            {
                if (CardValue == EnumCardValueList.Joker || CardValue == EnumCardValueList.Stop || CardValue == EnumCardValueList.Continue)
                    throw new BasicBlankException("Unable to use jokers, stops, or continue ones");
                if (CardValue == EnumCardValueList.LowAce || CardValue == EnumCardValueList.HighAce)
                    return "A";
                if (CardValue == EnumCardValueList.Jack)
                    return "J";
                if (CardValue == EnumCardValueList.Queen)
                    return "Q";
                if (CardValue == EnumCardValueList.King)
                    return "K";
                return CardValue.FromEnum().ToString();
            }
            return NumberValue.ToString();
        }
        protected virtual bool CanDrawNumber()
        {
            return true;
        }
        protected virtual void SelectProcesses() { }
        public override void DrawImage(SKCanvas dc)
        {
            if (CanDrawNumber() == false)
                return;
            var thisRect = GetMainRect();
            var thisValue = GetValueToPrint();
            if (thisValue == "")
                return;
            if (NeedsToClear == true)
                dc.Clear();// has to clear everytime.
            SelectProcesses();
            DrawSelector(dc);
            float fontSize = 0.0f; ;
            if (thisValue.Length == 2)
                fontSize = thisRect.Height * 0.6f;
            else if (thisValue.Length == 1)
                fontSize = thisRect.Height * 0.9f;
            else if (thisValue.Length == 3)
                fontSize = thisRect.Height * 0.4f;
            var textPaint = MiscHelpers.GetTextPaint(TextColor, fontSize);
            dc.DrawBorderText(thisValue, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _borderPaint, thisRect); // needs the text borders though.
        }
    }
}