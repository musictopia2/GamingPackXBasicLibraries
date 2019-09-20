using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace BasicGameFramework.GameGraphicsCP.GamePieces
{
    public class PawnPiecesCP<E> : BaseGraphicsCP, IEnumPiece<E>
        where E : struct, Enum
    {
        private string _tempValue = "";
        E IEnumPiece<E>.EnumValue
        {
            get
            {
                return (E)Enum.Parse(typeof(E), _tempValue);
            }
            set
            {
                _tempValue = value.ToString();
                MainColor = value.ToColor();
            }
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (NeedsToClear == true)
                dc.Clear();
            SKRect rect_Piece;
            rect_Piece = GetMainRect();
            SKRect rect_Base;
            rect_Base = SKRect.Create(rect_Piece.Left, (rect_Piece.Top + rect_Piece.Height) - (rect_Piece.Height / 8), rect_Piece.Width, rect_Piece.Height / 8);
            SKPath gp_Temp;
            SKPoint[] pts_Curve;
            gp_Temp = new SKPath();
            pts_Curve = new SKPoint[3];
            pts_Curve[0] = new SKPoint(rect_Piece.Left + (rect_Piece.Width / 2), rect_Piece.Top + (rect_Piece.Height / 8));
            pts_Curve[1] = new SKPoint(rect_Piece.Left + (rect_Piece.Width / 3), rect_Piece.Top + ((rect_Piece.Height * 8) / 11));
            pts_Curve[2] = new SKPoint(rect_Piece.Left, (rect_Piece.Top + rect_Piece.Height) - rect_Base.Height);
            gp_Temp.MoveTo(pts_Curve[0]);
            gp_Temp.LineTo(pts_Curve[1]);
            gp_Temp.LineTo(pts_Curve[2]);
            pts_Curve = new SKPoint[3];
            pts_Curve[0] = new SKPoint(rect_Piece.Left + rect_Piece.Width, (rect_Piece.Top + rect_Piece.Height) - rect_Base.Height);
            pts_Curve[1] = new SKPoint((rect_Piece.Left + rect_Piece.Width) - (rect_Piece.Width / 3), rect_Piece.Top + ((rect_Piece.Height * 8) / 11));
            pts_Curve[2] = new SKPoint(rect_Piece.Left + (rect_Piece.Width / 2), rect_Piece.Top + (rect_Piece.Height / 8));
            SKRect rect_Temp;
            float int_Temp;
            int_Temp = rect_Piece.Width / 3;
            rect_Temp = SKRect.Create(rect_Piece.Left + (rect_Piece.Width / 2) - (int_Temp / 2), rect_Piece.Top, int_Temp, int_Temp);
            gp_Temp.LineTo(pts_Curve[0]);
            gp_Temp.LineTo(pts_Curve[1]);
            gp_Temp.LineTo(pts_Curve[2]);
            gp_Temp.Close();
            dc.DrawPath(gp_Temp, MainPaint);
            dc.DrawPath(gp_Temp, _borderPaint);
            dc.DrawOval(rect_Temp, _borderPaint);
            dc.DrawOval(rect_Temp, MainPaint);
            dc.DrawRect(rect_Base, MainPaint);
            dc.DrawRect(rect_Base, _borderPaint);
        }
        private readonly SKPaint _borderPaint;
        public PawnPiecesCP()
        {
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
    }
}