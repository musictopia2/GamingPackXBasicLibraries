using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections;
using System.Reflection;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Cards
{
    public class DeckOfCardsCP : ObservableObject, IDeckGraphicsCP
    {
        public static string TagUsed => "regularcard";
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private bool _isUnknown;
        public bool IsUnknown
        {
            get { return _isUnknown; }
            set
            {
                if (SetProperty(ref _isUnknown, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumCardValueList _cardValue;
        public EnumCardValueList CardValue
        {
            get { return _cardValue; }
            set
            {
                if (SetProperty(ref _cardValue, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumCardValueList _displayValue;
        public EnumCardValueList DisplayValue
        {
            get { return _displayValue; }
            set
            {
                if (SetProperty(ref _displayValue, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumSuitList _suitValue;
        public EnumSuitList SuitValue
        {
            get { return _suitValue; }
            set
            {
                if (SetProperty(ref _suitValue, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumSuitList _displaySuit;
        public EnumSuitList DisplaySuit
        {
            get { return _displaySuit; }
            set
            {
                if (SetProperty(ref _displaySuit, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumCardTypeList _cardTypeValue;
        public EnumCardTypeList CardTypeValue
        {
            get { return _cardTypeValue; }
            set
            {
                if (SetProperty(ref _cardTypeValue, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        public static int ExtraBorders { get; set; }
        private bool _drawTopRight;
        public bool DrawTopRight
        {
            get
            {
                return _drawTopRight;
            }
            set
            {
                if (SetProperty(ref _drawTopRight, value) == true)
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumColorList _mainColor;
        public EnumColorList MainColor
        {
            get { return _mainColor; }
            set
            {
                if (SetProperty(ref _mainColor, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public bool NeedsToDrawBacks => true; //for sure yes on this one.
        private Assembly? _thisAssembly;
        private SKPaint? _blackSolid;
        private SKPaint? _redSolid;
        private SKPaint? _eyeWhenBlack;
        private SKPaint? _blackBorder;
        private SKPaint? _aquaSolid;
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        #region Implemented Stuff
        public bool CanStartDrawing() //done.
        {
            if (SuitValue == EnumSuitList.None && CardTypeValue == EnumCardTypeList.Regular && IsUnknown == false)
            {
                return false;
            }
            return true;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card) //done
        {
            if (MainGraphics!.IsUnknown == true && MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _tempPaint!);
            else if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, ExtraBorders); //i think 3 is enough.
            MainGraphics.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) //done
        {
            var thisImage = ImageExtensions.GetSkBitmap(_thisAssembly!, "back.png");
            canvas.DrawBitmap(thisImage, rect_Card, MainGraphics!.BitPaint);
        }
        public void DrawImage(SKCanvas Canvas, SKRect rect_Card)
        {
            var rect_Center = MiscHelpers.GetRectangle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 8), rect_Card.Width - (rect_Card.Width / 2), rect_Card.Height * 3 / 4);
            int int_CornerSuitWidth;
            EnumCardValueList tempValue;
            if (CardValue == 0 && CardTypeValue == EnumCardTypeList.None)
                throw new BasicBlankException("The card value cannot be 0.  Rethink");
            if (DisplayValue == EnumCardValueList.None)
                tempValue = CardValue;
            else
                tempValue = DisplayValue;
            if (CardTypeValue == EnumCardTypeList.Regular)
            {
                if (tempValue > EnumCardValueList.Ten)
                    DrawFaceCards(Canvas, rect_Center);
                else
                    DrawCenterSuits(Canvas, rect_Center);
                DrawCardText(Canvas, rect_Card);
                var rect_Number = MiscHelpers.GetRectangle(rect_Card.Left, rect_Card.Top + rect_Card.Width / 40, rect_Center.Left - rect_Card.Left, rect_Card.Height / 4);
                int_CornerSuitWidth = (int)rect_Number.Width * 3 / 4;
                var rect_Suit = MiscHelpers.GetRectangle(rect_Number.Left + ((rect_Number.Width - int_CornerSuitWidth) / 2), rect_Card.Top + rect_Card.Width / 40 + (rect_Card.Height / 5) + (rect_Card.Height / 30), int_CornerSuitWidth, int_CornerSuitWidth);
                DrawSuit(Canvas, rect_Suit, false);
                Canvas.Save();
                Canvas.RotateDegrees(180, rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2));
                DrawSuit(Canvas, rect_Suit, false);
                Canvas.Restore();
                if (DrawTopRight)
                {
                    rect_Suit = MiscHelpers.GetRectangle(rect_Number.Left + ((rect_Number.Width - int_CornerSuitWidth) / 2) + rect_Card.Width - rect_Number.Width, rect_Card.Top + rect_Card.Height / 20, int_CornerSuitWidth, int_CornerSuitWidth);
                    DrawSuit(Canvas, rect_Suit, false);
                }
            }
            else
            {
                DrawIrregularCard(Canvas, rect_Card, rect_Center);
            }
        }
        public SKColor GetFillColor() //done
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle() //done
        {
            return MainGraphics!.GetDrawingRectangle(); //try this one.
        }
        private SKPaint? _tempPaint;
        public void Init()
        {
            if (MainGraphics == null)
                throw new BasicBlankException("You never sent in the main graphics for helpers");
            _blackSolid = MiscHelpers.GetSolidPaint(SKColors.Black);
            _redSolid = MiscHelpers.GetSolidPaint(SKColors.Red);
            _eyeWhenBlack = MiscHelpers.GetStrokePaint(SKColors.Aqua, 2);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _aquaSolid = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            _tempPaint = MiscHelpers.GetSolidPaint(otherColor);
            MainGraphics.ActualWidth = 55;
            MainGraphics.ActualHeight = 72;
            MiscHelpers.MostlyBold = true;
            _thisAssembly = Assembly.GetAssembly(this.GetType());
        }
        #endregion
        #region Drawings
        private void DrawCenterSuits(SKCanvas canvas, SKRect rect_Center)
        {
            Hashtable arr_Rectangles = new Hashtable(); // hashtable is supported with the .net standard 2.0.  collection is not
            CustomBasicList<SKPoint> arr_Current = new CustomBasicList<SKPoint>();
            int int_Row;
            int int_Col;
            int int_Count;
            SKRect rect_Temp;
            int int_Height;
            SKPoint pt_Temp;
            int_Height = (int)rect_Center.Height / 5;
            for (int_Row = 0; int_Row <= 6; int_Row++)
            {
                for (int_Col = 0; int_Col <= 2; int_Col++)
                {
                    rect_Temp = MiscHelpers.GetRectangle(rect_Center.Left + ((rect_Center.Width / 3) * int_Col), rect_Center.Top + ((rect_Center.Height / 7) * int_Row) - (int_Height - (rect_Center.Height / 7)), rect_Center.Width / 3, int_Height);
                    arr_Rectangles.Add(new SKPoint(int_Col + 1, int_Row + 1), rect_Temp);
                }
            }
            EnumCardValueList tempValue;
            if (DisplayValue == EnumCardValueList.None)
                tempValue = CardValue;
            else
                tempValue = DisplayValue;
            switch (tempValue)
            {
                case EnumCardValueList.LowAce:
                case EnumCardValueList.HighAce:
                    {
                        arr_Current.Add(new SKPoint(2, 4));
                        break;
                    }
                case EnumCardValueList.Two:
                    {
                        arr_Current.Add(new SKPoint(2, 1));
                        arr_Current.Add(new SKPoint(2, 7));
                        break;
                    }
                case EnumCardValueList.Three:
                    {
                        arr_Current.Add(new SKPoint(2, 1));
                        arr_Current.Add(new SKPoint(2, 4));
                        arr_Current.Add(new SKPoint(2, 7));
                        break;
                    }
                case EnumCardValueList.Four:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 7));
                        break;
                    }
                case EnumCardValueList.Five:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 7));
                        arr_Current.Add(new SKPoint(2, 4));
                        break;
                    }
                case EnumCardValueList.Six:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 4));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 4));
                        arr_Current.Add(new SKPoint(3, 7));
                        break;
                    }
                case EnumCardValueList.Seven:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 4));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 4));
                        arr_Current.Add(new SKPoint(3, 7));
                        arr_Current.Add(new SKPoint(2, 3));
                        break;
                    }
                case EnumCardValueList.Eight:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 3));
                        arr_Current.Add(new SKPoint(1, 5));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 3));
                        arr_Current.Add(new SKPoint(3, 5));
                        arr_Current.Add(new SKPoint(3, 7));
                        break;
                    }
                case EnumCardValueList.Nine:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 3));
                        arr_Current.Add(new SKPoint(1, 5));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 3));
                        arr_Current.Add(new SKPoint(3, 5));
                        arr_Current.Add(new SKPoint(3, 7));
                        arr_Current.Add(new SKPoint(2, 4));
                        break;
                    }
                case EnumCardValueList.Ten:
                    {
                        arr_Current.Add(new SKPoint(1, 1));
                        arr_Current.Add(new SKPoint(1, 3));
                        arr_Current.Add(new SKPoint(1, 5));
                        arr_Current.Add(new SKPoint(1, 7));
                        arr_Current.Add(new SKPoint(3, 1));
                        arr_Current.Add(new SKPoint(3, 3));
                        arr_Current.Add(new SKPoint(3, 5));
                        arr_Current.Add(new SKPoint(3, 7));
                        arr_Current.Add(new SKPoint(2, 2));
                        arr_Current.Add(new SKPoint(2, 6));
                        break;
                    }
            }
            var loopTo = arr_Current.Count;
            for (int_Count = 1; int_Count <= loopTo; int_Count++)
            {
                pt_Temp = arr_Current[int_Count - 1]; // because its 0 based
                rect_Temp = (SKRect)arr_Rectangles[pt_Temp];
                if (pt_Temp.Y > 4)
                    DrawSuit(canvas, rect_Temp, true);
                else
                    DrawSuit(canvas, rect_Temp, false);
            }
        }
        private void DrawIrregularCard(SKCanvas canvas, SKRect rect_CardIn, SKRect rect_CenterIn)
        {
            SKRect rect_Center;
            var rect_Card = MiscHelpers.GetRectangle(rect_CardIn.Left, rect_CardIn.Top, rect_CardIn.Width, rect_CardIn.Height);
            rect_Center = MiscHelpers.GetRectangle(rect_CardIn.Width / 4, rect_CardIn.Height / 8, rect_CenterIn.Width, rect_CenterIn.Height);
            var widths = rect_CardIn.Width / 3;
            if (CardTypeValue == EnumCardTypeList.Joker)
                rect_Center = MiscHelpers.GetRectangle(widths, rect_CardIn.Top, rect_CardIn.Width - widths - 2, rect_CardIn.Height);// only joker its needed (for now)
            SKColor clr_Suit;
            if (MainColor == EnumColorList.Black)
                clr_Suit = SKColors.Black;
            else
                clr_Suit = SKColors.Red;
            if (CardTypeValue == EnumCardTypeList.Joker)
            {
                SKRect firstRect;
                SKRect secondRect;
                SKRect middleRect;
                middleRect = MainGraphics!.GetActualRectangle(2, 25, 50, 23);
                var fontSize = middleRect.Height * 0.8f; // can adjust as needed
                firstRect = MainGraphics.GetActualRectangle(18, 6, 22, 22);
                secondRect = MainGraphics.GetActualRectangle(18, 46, 22, 22);
                SKPaint textPaint;
                SKPaint tempEye;
                SKPaint tempSolid;
                SKPaint tempBorder;
                if (clr_Suit == SKColors.Black)
                {
                    textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
                    tempSolid = _blackSolid!;
                    tempBorder = _eyeWhenBlack!;
                    tempEye = _aquaSolid!;
                }
                else
                {
                    textPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
                    tempSolid = _redSolid!;
                    tempBorder = _blackBorder!;
                    tempEye = _blackSolid!;
                }
                canvas.DrawSmiley(firstRect, tempSolid, tempBorder, tempEye);
                canvas.DrawSmiley(secondRect, tempSolid, tempBorder, tempEye);
                canvas.DrawCustomText("Joker", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, middleRect, out _);
                return;
            }
            if (CardTypeValue == EnumCardTypeList.Continue)
            {
                using SKPaint pPaint = new SKPaint();
                pPaint.IsAntialias = true;
                pPaint.FilterQuality = SKFilterQuality.High;
                pPaint.Color = new SKColor(0, 255, 0, 255);
                pPaint.Style = SKPaintStyle.Stroke;
                pPaint.StrokeWidth = rect_Center.Width / 10;
                canvas.DrawOval(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2), rect_Center.Width / 2, rect_Center.Width / 2, pPaint);
                return;
            }
            if (CardTypeValue == EnumCardTypeList.Stop)
            {
                var sPaint = MiscHelpers.GetStrokePaint(SKColors.Red, rect_Center.Width / 10);
                var rect_Stop = MiscHelpers.GetRectangle(rect_Center.Left, rect_Center.Top + (rect_Center.Height / 2) - (rect_Center.Width / 2), rect_Center.Width, (rect_Center.Width));
                canvas.DrawLine(rect_Stop.Left, rect_Stop.Top, rect_Stop.Left + rect_Stop.Width, rect_Stop.Top + rect_Stop.Height, sPaint);
                canvas.DrawLine(rect_Stop.Left, rect_Stop.Top + rect_Stop.Height, rect_Stop.Left + rect_Stop.Width, rect_Stop.Top, sPaint);
            }
        }
        private void DrawSuit(SKCanvas canvas, SKRect rect_Suit, bool bln_Flip)
        {
            using SKPaint thisPaint = new SKPaint();
            thisPaint.Style = SKPaintStyle.Stroke;
            thisPaint.Color = SKColors.Black;
            thisPaint.StrokeWidth = rect_Suit.Width / 6;
            EnumSuitList tempSuit;
            if (DisplaySuit == EnumSuitList.None)
                tempSuit = SuitValue;
            else
                tempSuit = DisplaySuit;
            if (tempSuit == EnumSuitList.Spades)
            {
                if (bln_Flip)
                    canvas.DrawLine(rect_Suit.Left + (rect_Suit.Width / 2), rect_Suit.Top + (rect_Suit.Height / 2), rect_Suit.Left + (rect_Suit.Width / 2), rect_Suit.Top - rect_Suit.Height * (0.1f), thisPaint);
                else
                    canvas.DrawLine(rect_Suit.Left + (rect_Suit.Width / 2), rect_Suit.Top + (rect_Suit.Height / 2), rect_Suit.Left + (rect_Suit.Width / 2), rect_Suit.Top + rect_Suit.Height * (1.1f), thisPaint);
            }
            SKPaint newPaint;
            if (tempSuit == EnumSuitList.Hearts || tempSuit == EnumSuitList.Diamonds)
                newPaint = _redSolid!;
            else
                newPaint = _blackSolid!;

            if (bln_Flip == true)
            {
                canvas.Save();
                canvas.RotateDrawing(RotateExtensions.EnumRotateCategory.FlipOnly180, rect_Suit);
                if (tempSuit != EnumSuitList.Diamonds)
                    canvas.DrawCardSuit(tempSuit, rect_Suit, newPaint, null);
                else
                    canvas.DrawLargerDiamond(rect_Suit, newPaint, null);

                canvas.Restore();
            }
            else if (tempSuit != EnumSuitList.Diamonds)
            {
                canvas.DrawCardSuit(tempSuit, rect_Suit, newPaint, null);
            }
            else
            {
                canvas.DrawLargerDiamond(rect_Suit, newPaint, null);
            }
        }
        private string GetTextOfCard()
        {
            EnumCardValueList tempValue;
            if (DisplayValue == EnumCardValueList.None)
                tempValue = CardValue;
            else
                tempValue = DisplayValue;
            switch (tempValue)
            {
                case EnumCardValueList.LowAce:
                case EnumCardValueList.HighAce:
                    {
                        return "A";
                    }
                case EnumCardValueList.Jack:
                    {
                        return "J";
                    }
                case EnumCardValueList.Queen:
                    {
                        return "Q";
                    }
                case EnumCardValueList.King:
                    {
                        return "K";
                    }
                default:
                    {
                        return tempValue.FromEnum().ToString();
                    }
            }
        }
        private void DrawCardText(SKCanvas canvas, SKRect rect_Card)
        {
            SKPaint thisPaint = new SKPaint();
            EnumSuitList tempSuit;
            if (DisplaySuit == EnumSuitList.None)
                tempSuit = SuitValue;
            else
                tempSuit = DisplaySuit;
            if (tempSuit == EnumSuitList.Clubs || tempSuit == EnumSuitList.Spades)
                thisPaint.Color = SKColors.Black;
            else
                thisPaint.Color = SKColors.Red;
            // an example of special style text is here
            // https://stackoverflow.com/questions/39706244/how-to-draw-rich-text-with-skia-or-skiasharp
            thisPaint.FakeBoldText = true; // to make bold
            thisPaint.Typeface = SKTypeface.FromFamilyName("Tahoma");
            thisPaint.IsAntialias = true;
            string thisText;
            thisText = GetTextOfCard();
            SKPoint thisPoint = new SKPoint();
            var extraY = rect_Card.Top + (rect_Card.Height / 40);
            if (thisText == "10")
            {
                thisPaint.TextSize = rect_Card.Height / 6;
                thisPoint.X = rect_Card.Left;
            }
            else
            {
                thisPaint.TextSize = rect_Card.Height / 5;
                thisPoint.X = rect_Card.Left + (rect_Card.Width / 20);
            }
            SKRect tempRect = default;
            _ = thisPaint.MeasureText(thisText, ref tempRect);
            thisPoint.Y = tempRect.Top * -1;
            thisPoint.Y += (extraY * 3);
            canvas.DrawText(thisText, thisPoint.X, thisPoint.Y, thisPaint);
            canvas.Save();
            canvas.RotateDegrees(180, rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2)); // maybe this will work
            canvas.DrawText(thisText, thisPoint.X, thisPoint.Y, thisPaint);
            canvas.Restore(); // i think its this instead of pop
        }
        private void DrawFaceCards(SKCanvas canvas, SKRect rect_Center)
        {
            SKBitmap img_Card;
            string thisText = "";
            EnumCardValueList tempValue;
            if (DisplayValue == EnumCardValueList.None)
                tempValue = CardValue;
            else
                tempValue = DisplayValue;
            EnumSuitList tempSuit;
            if (DisplaySuit == EnumSuitList.None)
                tempSuit = SuitValue;
            else
                tempSuit = DisplaySuit;
            switch (tempValue)
            {
                case EnumCardValueList.Jack:
                    {
                        switch (tempSuit)
                        {
                            case EnumSuitList.Clubs:
                                {
                                    thisText = "jack_clubs";
                                    break;
                                }
                            case EnumSuitList.Diamonds:
                                {
                                    thisText = "jack_diamonds";
                                    break;
                                }
                            case EnumSuitList.Hearts:
                                {
                                    thisText = "jack_hearts";
                                    break;
                                }
                            case EnumSuitList.Spades:
                                {
                                    thisText = "jack_spades";
                                    break;
                                }
                        }
                        break;
                    }
                case EnumCardValueList.Queen:
                    {
                        switch (tempSuit)
                        {
                            case EnumSuitList.Clubs:
                                {
                                    thisText = "queen_clubs";
                                    break;
                                }
                            case EnumSuitList.Diamonds:
                                {
                                    thisText = "queen_diamonds";
                                    break;
                                }
                            case EnumSuitList.Hearts:
                                {
                                    thisText = "queen_hearts";
                                    break;
                                }
                            case EnumSuitList.Spades:
                                {
                                    thisText = "queen_spades";
                                    break;
                                }
                        }
                        break;
                    }
                case EnumCardValueList.King:
                    {
                        switch (tempSuit)
                        {
                            case EnumSuitList.Clubs:
                                {
                                    thisText = "king_clubs";
                                    break;
                                }
                            case EnumSuitList.Diamonds:
                                {
                                    thisText = "king_diamonds";
                                    break;
                                }
                            case EnumSuitList.Hearts:
                                {
                                    thisText = "king_hearts";
                                    break;
                                }
                            case EnumSuitList.Spades:
                                {
                                    thisText = "king_spades";
                                    break;
                                }
                        }
                        break;
                    }
            }
            if (thisText == "")
                return;
            thisText += ".png";
            img_Card = ImageExtensions.GetSkBitmap(_thisAssembly!, thisText);
            canvas.DrawBitmap(img_Card, rect_Center, MainGraphics!.BitPaint);
        }
        #endregion
    }
}