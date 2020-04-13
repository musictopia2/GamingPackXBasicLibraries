using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces
{
    public class MarblePiecesCP<E> : BaseGraphicsCP, IEnumPiece<E>
        where E : struct, Enum
    {

        private bool _UseTrouble;
        public bool UseTrouble
        {
            get
            {
                return _UseTrouble;
            }
            set
            {
                if (SetProperty(ref _UseTrouble, value) == true)
                {
                    if (PaintUI != null)
                        PaintUI.DoInvalidate();

                }
            }
        }
        private string tempValue = "";
        E IEnumPiece<E>.EnumValue
        {
            get
            {
                return (E)Enum.Parse(typeof(E), tempValue);
            }
            set
            {
                tempValue = value.ToString();
                MainColor = value.ToColor();
            }
        }
        public override void DrawImage(SKCanvas dc)
        {
            var thisRect = GetMainRect();
            if (NeedsToClear == true)
                dc.Clear();
            if (UseTrouble == true)
                DrawTrouble(dc, thisRect);
            else
                DrawMarble(dc, thisRect);
        }
        private void DrawTrouble(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKColor firstColor;
            SKColor secondColor;
            firstColor = new SKColor(255, 255, 255, 150); // 60 instead of 100 seems to do the trick
            secondColor = new SKColor(0, 0, 0, 150);
            var br_Fill = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            SKRect rect_Temp;
            float int_Offset = bounds.Width / 6;
            gp.AddLine(new SKPoint(bounds.Left + int_Offset, bounds.Top + (bounds.Height / 4)), new SKPoint((bounds.Left + bounds.Width) - (int_Offset), bounds.Top + (bounds.Height / 4)), true);
            gp.ArcTo(SKRect.Create(bounds.Left + int_Offset, (bounds.Top + (bounds.Height / 2)) - int_Offset, bounds.Width - (int_Offset * 2), bounds.Height / 2), 0, 180, false);
            gp.Close();
            canvas.DrawPath(gp, MainPaint);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            rect_Temp = SKRect.Create(bounds.Left + int_Offset, bounds.Top, bounds.Width - (int_Offset * 2), bounds.Height / 2);
            gp.AddOval(rect_Temp);
            gp.Close();
            canvas.DrawPath(gp, MainPaint);
            rect_Temp = SKRect.Create(rect_Temp.Left + (rect_Temp.Width / 4), rect_Temp.Top + (rect_Temp.Height / 4), (rect_Temp.Width / 2), rect_Temp.Height / 2);
            br_Fill = MiscHelpers.GetLinearGradientPaint(secondColor, firstColor, bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawOval(rect_Temp, br_Fill);
        }
        private void DrawMarble(SKCanvas canvas, SKRect thisRect) // start with the marble piece.
        {
            var br_Fill = MiscHelpers.GetCenterGradientPaint(SKColors.White, MainColor.ToSKColor(), new SKPoint(thisRect.Left + (thisRect.Width / 2), thisRect.Top + (thisRect.Height / 2)), thisRect.Height / 2);
            SKPath gp = new SKPath();
            gp.AddOval(thisRect);
            SKColor firstColor;
            SKColor secondColor;
            firstColor = new SKColor(255, 255, 255, 50); // 60 instead of 100 seems to do the trick
            secondColor = new SKColor(0, 0, 0, 50);
            var br_Shade = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, thisRect, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, br_Shade);
        }
    }
}