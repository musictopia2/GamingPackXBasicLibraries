using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFramework.GameGraphicsCP.GamePieces
{
    public class DeckPieceCP : BaseGraphicsCP, IEnumPiece<EnumSuitList>
    {

        private EnumSuitList _Suit;
        public EnumSuitList Suit
        {
            get { return _Suit; }
            set
            {
                if (SetProperty(ref _Suit, value)) { }
            }
        }
        public EnumSuitList EnumValue { get => Suit; set => Suit = value; }
        private readonly SKPaint _blackPaint;
        private readonly SKPaint _redPaint;
        public DeckPieceCP()
        {
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            NeedsHighlighting = true;
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (Suit == EnumSuitList.None)
                return;// because no suit
            dc.Clear(); // i think
            DrawSelector(dc);
            var ThisRect = GetMainRect();
            SKPaint SuitPaint;
            if (Suit == EnumSuitList.Hearts || Suit == EnumSuitList.Diamonds)
                SuitPaint = _redPaint;
            else
                SuitPaint = _blackPaint;

            var newRect = SKRect.Create(5, 5, ThisRect.Width - 10, ThisRect.Height - 10);
            dc.DrawCardSuit(Suit, newRect, SuitPaint, null); // unable to do borders here.
        }
    }
}