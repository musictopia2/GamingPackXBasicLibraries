using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFramework.GameGraphicsCP.MiscClasses
{
    public enum EnumArrowDirection
    {
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }
    public static class Extensions
    {
        private static SKRect GetActualRectangle(int x, int y, int width, int height, TempPosition thisTemp)
        {
            return GetActualRectangle(SKRect.Create(x, y, width, height), thisTemp);
        }
        private static SKRect GetActualRectangle(SKRect originalRectangle, TempPosition thisTemp)
        {
            SKPoint oldPoint;
            oldPoint = new SKPoint(originalRectangle.Left, originalRectangle.Top);
            SKSize oldSize;
            oldSize = new SKSize(originalRectangle.Width, originalRectangle.Height);
            SKPoint newPoint;
            SKSize newSize;
            newPoint = GetActualPoint(oldPoint, thisTemp);
            newSize = GetActualSize(oldSize, thisTemp);
            return SKRect.Create(newPoint, newSize);
        }
        private static SKPoint GetActualPoint(int x, int y, TempPosition thisTemp)
        {
            return GetActualPoint(new SKPoint(x, y), thisTemp);
        }
        private static SKPoint GetActualPoint(SKPoint pt_Current, TempPosition thisTemp)
        {
            float int_X;
            float int_Y;
            int_X = ((thisTemp.ActualWidth / thisTemp.OriginalSize.Width) * pt_Current.X);
            int_Y = ((thisTemp.ActualHeight / thisTemp.OriginalSize.Height) * pt_Current.Y);
            return new SKPoint(int_X + thisTemp.Location.X, int_Y + thisTemp.Location.Y);
        }
        private static SKSize GetActualSize(SKSize size_Current, TempPosition thisTemp)
        {
            float int_Width;
            float int_Height;
            int_Width = ((thisTemp.ActualWidth / thisTemp.OriginalSize.Width) * size_Current.Width);
            int_Height = ((thisTemp.ActualHeight / thisTemp.OriginalSize.Height) * size_Current.Height);
            return new SKSize(int_Width, int_Height);
        }
        private static TempPosition GetTempRect(SKRect thisRect)
        {
            TempPosition output = new TempPosition();
            output.Location = thisRect.Location;
            output.ActualHeight = thisRect.Height;
            output.ActualWidth = thisRect.Width;
            return output;
        }
        public static void DrawLargerDiamond(this SKCanvas thisCanvas, SKRect thisRect, SKPaint solidPaint, SKPaint? borderPaint)
        {
            TempPosition temps = GetTempRect(thisRect);
            SKPoint firstPoint;
            SKPoint secondPoint;
            SKPoint thirdPoint;
            SKPoint fourthPoint;
            firstPoint = GetActualPoint(200, 2, temps);
            secondPoint = GetActualPoint(398, 200, temps);
            thirdPoint = GetActualPoint(200, 398, temps);
            fourthPoint = GetActualPoint(2, 200, temps);
            SKPoint[] pts = new[] { firstPoint, secondPoint, thirdPoint, fourthPoint };
            SKPath thisPath = new SKPath();
            thisPath.AddLines(pts, true);
            thisCanvas.DrawPath(thisPath, solidPaint);
            if (borderPaint != null)
                thisCanvas.DrawPath(thisPath, borderPaint);
        }
        public static void DrawCardSuit(this SKCanvas thisCanvas, EnumSuitList suitCategory, SKRect thisRect, SKPaint solidPaint, SKPaint? borderPaint)
        {
            TempPosition temps = GetTempRect(thisRect);
            if (solidPaint == null)
                throw new BasicBlankException("All Cards Must Have Solid Brushes");
            if (borderPaint != null && suitCategory == EnumSuitList.Clubs)
                throw new BasicBlankException("Clubs can't have stroke paint currently");
            if (borderPaint != null && suitCategory == EnumSuitList.Spades)
                throw new BasicBlankException("Spades can't have stroke paint currently");
            switch (suitCategory)
            {
                case EnumSuitList.Clubs:
                    {
                        SKPath thisPath = new SKPath(); //used proportions here.  seemed to work great.
                        var firstRect = GetActualRectangle(125, 0, 150, 150, temps);
                        thisPath.AddOval(firstRect, SKPathDirection.Clockwise);
                        var secondRect = GetActualRectangle(0, 150, 150, 150, temps);
                        thisPath.AddOval(secondRect, SKPathDirection.Clockwise);
                        var thirdRect = GetActualRectangle(250, 150, 150, 150, temps);
                        thisPath.AddOval(thirdRect, SKPathDirection.Clockwise);
                        SKPoint point1;
                        SKPoint point2;
                        point1 = GetActualPoint(185, 150, temps);
                        point2 = GetActualPoint(150, 200, temps);
                        thisPath.MoveTo(point1);
                        point1 = GetActualPoint(175, 180, temps);
                        thisPath.QuadTo(point1, point2);
                        point2 = GetActualPoint(150, 250, temps);
                        thisPath.LineTo(point2);
                        point1 = GetActualPoint(175, 270, temps);
                        point2 = GetActualPoint(175, 280, temps);
                        thisPath.QuadTo(point1, point2);
                        point2 = GetActualPoint(150, 400, temps);
                        thisPath.LineTo(point2);
                        var tempLine = GetActualPoint(250, 400, temps);
                        thisPath.LineTo(tempLine);
                        point1 = GetActualPoint(225, 350, temps);
                        point2 = GetActualPoint(225, 280, temps);
                        thisPath.QuadTo(point1, point2);
                        point1 = GetActualPoint(225, 270, temps);
                        point2 = GetActualPoint(250, 250, temps);
                        thisPath.QuadTo(point1, point2);
                        point2 = GetActualPoint(250, 200, temps);
                        thisPath.LineTo(point2);
                        point1 = GetActualPoint(230, 180, temps);
                        point2 = GetActualPoint(220, 150, temps);
                        thisPath.QuadTo(point1, point2);
                        thisPath.Close();
                        thisCanvas.DrawPath(thisPath, solidPaint);
                        break;
                    }
                case EnumSuitList.Diamonds:
                    {
                        SKPoint[] pts = new SKPoint[4];
                        pts[0] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y);
                        pts[1] = new SKPoint(thisRect.Location.X + (thisRect.Width * 3 / 4), thisRect.Location.Y + (thisRect.Height / 2));
                        pts[2] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + thisRect.Height);
                        pts[3] = new SKPoint(thisRect.Location.X + (thisRect.Width / 4), thisRect.Location.Y + (thisRect.Height / 2));
                        SKPath ThisPath = new SKPath();
                        ThisPath.AddLines(pts, true);
                        thisCanvas.DrawPath(ThisPath, solidPaint);
                        if (borderPaint == null == false)
                            thisCanvas.DrawPath(ThisPath, borderPaint);
                        break;
                    }
                case EnumSuitList.Hearts:
                    {
                        int avg;
                        avg = System.Convert.ToInt32((thisRect.Width + thisRect.Height) / 2);
                        int radius;
                        radius = System.Convert.ToInt32(avg / (double)2);
                        var topleftcorner = new SKPoint(thisRect.Location.X, thisRect.Location.Y);
                        var topleftsquare = SKRect.Create(topleftcorner.X, topleftcorner.Y, radius, radius);
                        var toprightsquare = SKRect.Create(topleftcorner.X + radius, topleftcorner.Y, radius, radius);
                        var thisPath = new SKPath();
                        thisPath.ArcTo(topleftsquare, 135, 225, false);
                        thisPath.ArcTo(toprightsquare, 180, 225, false);
                        thisPath.LineTo(radius + thisRect.Location.X, avg + thisRect.Location.Y);
                        thisPath.Close();
                        thisCanvas.DrawPath(thisPath, solidPaint);
                        if (borderPaint == null == false)
                            thisCanvas.DrawPath(thisPath, borderPaint);
                        break;
                    }
                case EnumSuitList.Spades:
                    {
                        var firstRect = GetActualRectangle(0, 100, 200, 200, temps);
                        thisCanvas.DrawOval(firstRect, solidPaint);
                        var secondRect = GetActualRectangle(200, 100, 200, 200, temps);
                        thisCanvas.DrawOval(secondRect, solidPaint);
                        var nextRect = GetActualRectangle(175, 175, 50, 200, temps);
                        thisCanvas.DrawRect(nextRect, solidPaint);
                        var tempRect = GetActualRectangle(0, 0, 400, 175, temps);
                        thisCanvas.DrawTriangle(tempRect, solidPaint, null);
                        break;
                    }
                default:
                    throw new BasicBlankException("Must choose one of the 4 suits to draw");
            }
        }
        public static void DrawStar(this SKCanvas thisCanvas, SKRect thisRect, SKPaint solidPaint, SKPaint borderPaint) // done
        {
            TempPosition temps = GetTempRect(thisRect);
            if (solidPaint == null == true && borderPaint == null == true)
                throw new BasicBlankException("Must send at least pen or solid brush.  Otherwise, nothing will draw obviously");
            SKPath thisPath = new SKPath();
            var firstPoint = GetActualPoint(200, 2, temps);
            var secondPoint = GetActualPoint(245, 138, temps); // done now maybe.
            thisPath.AddLine(firstPoint, secondPoint, true);
            var thirdPoint = GetActualPoint(390, 138, temps); // done now.
            thisPath.AddLine(secondPoint, thirdPoint);
            var fourthPoint = GetActualPoint(270, 223, temps); // could be fine now.
            thisPath.AddLine(thirdPoint, fourthPoint);
            var fifthPoint = GetActualPoint(317, 361, temps); // done
            thisPath.AddLine(fourthPoint, fifthPoint);
            var sixthPoint = GetActualPoint(200, 280, temps); // could be fine now
            thisPath.AddLine(fifthPoint, sixthPoint);
            var seventhPoint = GetActualPoint(82, 361, temps); // done
            thisPath.AddLine(sixthPoint, seventhPoint);
            var eighthPoint = GetActualPoint(130, 223, temps);
            thisPath.AddLine(seventhPoint, eighthPoint);
            var ninthPoint = GetActualPoint(9, 138, temps); // done
            thisPath.AddLine(eighthPoint, ninthPoint);
            var tenthPoint = GetActualPoint(154, 138, temps); // done now maybe
            thisPath.AddLine(ninthPoint, tenthPoint);
            thisPath.Close();
            if (solidPaint == null == false)
                thisCanvas.DrawPath(thisPath, solidPaint);
            if (borderPaint == null == false)
                thisCanvas.DrawPath(thisPath, borderPaint);
        }
        public static void DrawSmiley(this SKCanvas thisCanvas, SKRect thisRect, SKPaint solidPaint, SKPaint borderPaint, SKPaint eyePaint)
        {
            if (borderPaint == null)
                throw new BasicBlankException("Must specify borders.  Otherwise, can't draw the happy face obviously");
            if (eyePaint == null)
                throw new BasicBlankException("Must use a paint for the eyes.  Otherwise, eyes will not be drawn obviously");
            SKPath thisPath = new SKPath();
            var firstPoint = new SKPoint(thisRect.Location.X + (thisRect.Width * 0.1f), thisRect.Location.Y + (thisRect.Height * 4 / 7));
            thisPath.MoveTo(firstPoint);
            SKPoint secondPoint;
            SKPoint thirdPoint;
            secondPoint = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + (thisRect.Height * 1.1f));
            thirdPoint = new SKPoint(thisRect.Location.X + (thisRect.Width * 0.9f), thisRect.Location.Y + (thisRect.Height * 4 / 7));
            thisPath.QuadTo(secondPoint, thirdPoint);
            if (solidPaint == null == false)
                thisCanvas.DrawOval(thisRect, solidPaint);
            thisCanvas.DrawPath(thisPath, borderPaint);
            thisCanvas.DrawOval(thisRect, borderPaint);
            var rect_Eye = SKRect.Create(thisRect.Location.X + (thisRect.Width / 4), thisRect.Location.Y + (thisRect.Height / 4), thisRect.Width / 10, thisRect.Width / 10);
            thisCanvas.DrawOval(rect_Eye, eyePaint);
            rect_Eye = SKRect.Create(thisRect.Location.X + (thisRect.Width * 3 / 4) - (thisRect.Width / 10), thisRect.Location.Y + (thisRect.Height / 4), thisRect.Width / 10, thisRect.Width / 10);
            thisCanvas.DrawOval(rect_Eye, eyePaint);
        }
        public static void DrawArrow(this SKCanvas thisCanvas, SKRect thisRect, SKPaint thisPaint, EnumArrowDirection direction)
        {
            SKRect newRect;
            SKPath thisPath = new SKPath();
            var diffs = thisRect.Width / 2;
            var others = diffs / 2;
            float percs;
            float finals;
            float middles;
            float firstValue;
            float secondvalue;
            if (direction == EnumArrowDirection.Down || direction == EnumArrowDirection.Up)
            {
                percs = thisRect.Height * 0.25f;
                firstValue = thisRect.Location.X;
                secondvalue = thisRect.Right;
                middles = (secondvalue - firstValue) / 2;
                middles = firstValue + middles;
                if (percs < thisRect.Width)
                    finals = percs;
                else
                    finals = thisRect.Width;
            }
            else
            {
                diffs = thisRect.Height / 2;
                others = diffs / 2;
                percs = thisRect.Width * 0.25f;
                firstValue = thisRect.Location.Y;
                secondvalue = thisRect.Bottom;
                middles = (secondvalue - firstValue) / 2;
                middles = firstValue + middles;
                if (percs < thisRect.Height)
                    finals = percs;
                else
                    finals = thisRect.Height;
            }
            switch (direction)
            {
                case EnumArrowDirection.Down:
                    {
                        newRect = SKRect.Create(thisRect.Location.X + others, thisRect.Location.Y, diffs, thisRect.Height - finals);
                        thisPath.MoveTo(thisRect.Location.X, thisRect.Bottom - finals);
                        thisPath.LineTo(thisRect.Right, thisRect.Bottom - finals);
                        thisPath.LineTo(middles, thisRect.Bottom);
                        break;
                    }

                case EnumArrowDirection.Up:
                    {
                        newRect = SKRect.Create(thisRect.Location.X + others, thisRect.Location.Y + finals, diffs, thisRect.Height - finals);
                        thisPath.MoveTo(middles, thisRect.Location.Y);
                        thisPath.LineTo(thisRect.Location.X, thisRect.Location.Y + finals);
                        thisPath.LineTo(thisRect.Right, thisRect.Location.Y + finals);
                        break;
                    }

                case EnumArrowDirection.Right:
                    {
                        newRect = SKRect.Create(thisRect.Location.X, thisRect.Location.Y + others, thisRect.Width - finals, diffs);
                        thisPath.MoveTo(thisRect.Right - finals, thisRect.Location.Y);
                        thisPath.LineTo(thisRect.Right - finals, thisRect.Bottom);
                        thisPath.LineTo(thisRect.Right, middles);
                        break;
                    }

                case EnumArrowDirection.Left:
                    {
                        newRect = SKRect.Create(thisRect.Location.X + finals, thisRect.Location.Y + others, thisRect.Width - finals, diffs);
                        thisPath.MoveTo(thisRect.Location.X, middles);
                        thisPath.LineTo(thisRect.Location.X + finals, thisRect.Location.Y);
                        thisPath.LineTo(thisRect.Location.X + finals, thisRect.Bottom);
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Direction Not Supported");
                    }
            }
            thisPath.Close();
            thisCanvas.DrawRect(newRect, thisPaint);
            thisCanvas.DrawPath(thisPath, thisPaint);
        }
    }
}