using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace BasicGameFramework.GameGraphicsCP.Dominos
{
    public class DominosCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public static string TagUsed => "dominos";
        private bool _Drew;
        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                {
                    if (MainGraphics!.PaintUI == null)
                        return; //because sometimes something else is doing it like mexican train dominos.
                    MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private bool _Visible;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    if (MainGraphics!.PaintUI == null)
                        return; //because sometimes something else is doing it like mexican train dominos.
                    MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private bool _IsUnknown;
        public bool IsUnknown
        {
            get { return _IsUnknown; }
            set
            {
                if (SetProperty(ref _IsUnknown, value))
                {
                    MainGraphics!.PaintUI!.DoInvalidate();
                }
            }
        }
        private int _CurrentFirst = -1;
        public int CurrentFirst
        {
            get { return _CurrentFirst; }
            set
            {
                if (SetProperty(ref _CurrentFirst, value))
                {
                    _firstPaint = GetPaint(value);
                    if (MainGraphics!.PaintUI == null)
                        return; //because sometimes something else is doing it like mexican train dominos.
                    MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private int _CurrentSecond = -1;
        public int CurrentSecond
        {
            get { return _CurrentSecond; }
            set
            {
                if (SetProperty(ref _CurrentSecond, value))
                {
                    _secondPaint = GetPaint(value);
                    if (MainGraphics!.PaintUI == null)
                        return; //because sometimes something else is doing it like mexican train dominos.
                    MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private int _HighestDomino;

        public int HighestDomino
        {
            get { return _HighestDomino; }
            set
            {
                if (SetProperty(ref _HighestDomino, value))
                {
                    if (value > 6)
                    {
                        if (MainGraphics!.PaintUI == null)
                            return; //because sometimes something else is doing it like mexican train dominos.
                        _separatePaint = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
                    }
                }
            }
        }
        public bool NeedsToDrawBacks => true;
        public bool CanStartDrawing()
        {
            if (HighestDomino == 0 || CurrentFirst == -1 || CurrentSecond == -1)
                return false;
            return true;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _dDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetMainRect();
        }
        private SKPaint? _separatePaint;
        private SKPaint? _firstPaint;
        private SKPaint? _secondPaint;
        private SKPaint? _dicePaint;
        private SKPaint? _selectPaint;
        private SKPaint? _dDrewPaint;
        private SKPaint GetPaint(int value)
        {
            switch (value)
            {
                case 0:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Transparent);
                    }

                case 1:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Blue);
                    }

                case 2:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.DeepPink);
                    }

                case 3:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Red);
                    }

                case 4:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Yellow);
                    }

                case 5:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.DarkOrange);
                    }

                case 6:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Lime);
                    }

                case 7:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Aqua);
                    }

                case 8:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.DimGray);
                    }

                case 9:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.LightPink);
                    }

                case 10:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Purple);
                    }

                case 11:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Green);
                    }

                case 12:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Navy);
                    }

                case 13:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Gold);
                    }

                case 14:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Teal);
                    }

                case 15:
                    {
                        return MiscHelpers.GetSolidPaint(SKColors.Silver);
                    }
                default:
                    {
                        throw new BasicBlankException("Only 0 to 15 is supported for the paints"); // we never went past 15 even though otherwise, supports up to 21
                    }
            }
        }
        public void Init()
        {
            _dDrewPaint = MainGraphics!.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _separatePaint = MiscHelpers.GetStrokePaint(SKColors.Black, 4); // try 4 well see how it goes.
            _dicePaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            float firstX;
            float secondX;
            float firstY;
            float secondY;
            firstY = MainGraphics!.Location.Y; //i think
            secondY = rect_Card.Location.Y + (float)MainGraphics.ActualHeight;
            secondX = rect_Card.Location.X + (float)MainGraphics.ActualWidth / 2;
            firstX = secondX;
            canvas.DrawLine(firstX, firstY, secondX, secondY, _separatePaint);
            float tempFirst1 = (float)MainGraphics.ActualWidth / 2;

            SKSize thisSize = new SKSize(tempFirst1, (float)MainGraphics.ActualHeight);
            DrawDice(canvas, _firstPaint!, thisSize, new SKPoint(rect_Card.Location.X, rect_Card.Location.Y), CurrentFirst);
            DrawDice(canvas, _secondPaint!, thisSize, new SKPoint(rect_Card.Location.X + (rect_Card.Width / 2), rect_Card.Location.Y), CurrentSecond);
        }
        private void DrawDice(SKCanvas canvas, SKPaint dotColor, SKSize diceSize, SKPoint dicePoint, int value)
        {
            SKRect rect_Dice;
            CustomBasicList<SKRect> arr_Dots = new CustomBasicList<SKRect>();
            int int_DotWidth;
            float int_DotOffset1;
            float int_DotOffset2;
            float int_DotOffset3;
            int int_Count;
            int int_Row1 = 0;
            int int_Row2 = 0;
            int int_Row3 = 0;
            int int_Start1;
            int int_Start2;
            int int_Start3;
            float int_Width;
            SKRect rect_InnerWidth;
            int int_MaxCols = 0;
            double int_CenterCol;
            float int_CenterOffset;
            rect_Dice = SKRect.Create(dicePoint.X, dicePoint.Y, diceSize.Width, diceSize.Height);
            if (HighestDomino == 6)
                int_MaxCols = 3;
            else if (HighestDomino == 9)
                int_MaxCols = 3;
            else if (HighestDomino == 12)
                int_MaxCols = 4; // well see
            else if (HighestDomino == 15)
                int_MaxCols = 5;
            else if (HighestDomino == 18)
                int_MaxCols = 6;
            else if (HighestDomino == 21)
                int_MaxCols = 7;
            rect_InnerWidth = SKRect.Create(rect_Dice.Left + (rect_Dice.Width / 12), rect_Dice.Top, rect_Dice.Width - (rect_Dice.Width / 5), rect_Dice.Height);
            float int_W1 = (rect_InnerWidth.Width - (2 * (int_MaxCols + 1))) / int_MaxCols;
            float int_W2 = (rect_InnerWidth.Height * 3 / 4 - (2 * (4))) / 3;
            int_DotWidth = Math.Min((int)int_W1, (int)int_W2); // well see how it works when doing the .net standard (?)
            int_DotOffset1 = rect_Dice.Height / 8;
            int_DotOffset2 = rect_Dice.Height / 2 - int_DotWidth / 2;
            int_DotOffset3 = rect_Dice.Height - int_DotOffset1 - int_DotWidth;
            int_Width = (rect_InnerWidth.Width / int_MaxCols);
            if (value > 9)
            {
                int_Start1 = 1;
                int_Start2 = 1;
                int_Start3 = 1;
                switch (value)
                {
                    case 10:
                        {
                            int_Row1 = 3;
                            int_Row2 = 4;
                            int_Row3 = 3;
                            break;
                        }

                    case 11:
                        {
                            int_Row1 = 4;
                            int_Row2 = 3;
                            int_Row3 = 4;
                            break;
                        }

                    case 12:
                        {
                            int_Row1 = 4;
                            int_Row2 = 4;
                            int_Row3 = 4;
                            break;
                        }

                    case 13:
                        {
                            int_Row1 = 4;
                            int_Row2 = 5;
                            int_Row3 = 4;
                            break;
                        }

                    case 14:
                        {
                            int_Row1 = 5;
                            int_Row2 = 4;
                            int_Row3 = 5;
                            break;
                        }

                    case 15:
                        {
                            int_Row1 = 5;
                            int_Row2 = 5;
                            int_Row3 = 5;
                            break;
                        }

                    case 16:
                        {
                            int_Row1 = 5;
                            int_Row2 = 6;
                            int_Row3 = 5;
                            break;
                        }

                    case 17:
                        {
                            int_Row1 = 6;
                            int_Row2 = 5;
                            int_Row3 = 6;
                            break;
                        }

                    case 18:
                        {
                            int_Row1 = 6;
                            int_Row2 = 6;
                            int_Row3 = 6;
                            break;
                        }

                    case 19:
                        {
                            int_Row1 = 6;
                            int_Row2 = 7;
                            int_Row3 = 6;
                            break;
                        }

                    case 20:
                        {
                            int_Row1 = 7;
                            int_Row2 = 6;
                            int_Row3 = 7;
                            break;
                        }

                    case 21:
                        {
                            int_Row1 = 7;
                            int_Row2 = 7;
                            int_Row3 = 7;
                            break;
                        }
                }
                int_CenterOffset = int_Width * ((int_MaxCols - Math.Max(int_Row1, int_Row2)) / 2);
                var loopTo = (int_Row1 + int_Start1) - 1;
                for (int_Count = int_Start1; int_Count <= loopTo; int_Count++)
                    arr_Dots.Add(SKRect.Create(int_CenterOffset + rect_InnerWidth.Left + ((int_Count - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                var loopTo1 = (int_Row2 + int_Start2) - 1;
                for (int_Count = int_Start2; int_Count <= loopTo1; int_Count++)
                    arr_Dots.Add(SKRect.Create(int_CenterOffset + rect_InnerWidth.Left + ((int_Count - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                var loopTo2 = (int_Row3 + int_Start3) - 1;
                for (int_Count = int_Start3; int_Count <= loopTo2; int_Count++)
                    arr_Dots.Add(SKRect.Create(int_CenterOffset + rect_InnerWidth.Left + ((int_Count - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
            }
            else
            {
                float thisDec = (int_MaxCols + 1) / 2;
                int_CenterCol = Math.Floor(thisDec);
                if (value == 1)
                {
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                }
                else if (value == 2)
                {
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol - 2) * int_Width + (int_Width / 2) - (int_DotWidth / 2)), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol) * int_Width + (int_Width / 2) - (int_DotWidth / 2)), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                }
                else if (value == 3)
                {
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                }
                else if (value == 4)
                {
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)(int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)(int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)(int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + ((float)(int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                }
                else if (value == 5)
                {
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create(rect_InnerWidth.Left + (((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                }
                else if (value == 6)
                {
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                }
                else if (value == 7)
                {
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                }
                else if (value == 8)
                {
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                }
                else if (value == 9)
                {
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 2) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset1, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset2, int_DotWidth, int_DotWidth));
                    arr_Dots.Add(SKRect.Create((rect_InnerWidth.Left + ((float)int_CenterCol - 1) * int_Width) + (int_Width / 2) - (int_DotWidth / 2), rect_Dice.Top + int_DotOffset3, int_DotWidth, int_DotWidth));
                }
            }
            var loopTo3 = arr_Dots.Count;
            for (int_Count = 1; int_Count <= loopTo3; int_Count++)
            {
                SKRect rect_Temp = arr_Dots[int_Count - 1];
                canvas.DrawOval(rect_Temp, dotColor);
                canvas.DrawOval(rect_Temp, _dicePaint);
            }
        }
    }
}