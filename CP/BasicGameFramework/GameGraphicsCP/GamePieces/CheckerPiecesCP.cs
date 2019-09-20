using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFramework.GameGraphicsCP.GamePieces
{
    public class CheckerPiecesCP : BaseGraphicsCP
    {
        public bool IsCrowned { get; set; }
        public bool FlatPiece { get; set; }
        private bool _HasImage = true;
        public bool HasImage
        {
            get
            {
                return _HasImage;
            }
            set
            {
                if (SetProperty(ref _HasImage, value) == true)
                    PaintUI!.DoInvalidate();
            }
        }
        private string _BlankColor = cs.White;
        public string BlankColor
        {
            get { return _BlankColor; }
            set
            {
                if (SetProperty(ref _BlankColor, value))
                {
                    _noImageM = MiscHelpers.GetSolidPaint(value.ToSKColor());
                    PaintUI!.DoInvalidate();
                }
            }
        }
        private void DrawPiece(SKCanvas dc, SKRect rect_Piece) // part 1 is okay.
        {
            SKRect rect_Bottom = new SKRect();
            SKRect rect_Top = new SKRect();
            SKSize TempSize = new SKSize(rect_Piece.Width, rect_Piece.Height * 9 / 10);
            rect_Bottom.Left = rect_Piece.Left;
            rect_Top.Left = rect_Piece.Left;
            rect_Top.Top = rect_Piece.Top;
            if (FlatPiece == false)
            {
                rect_Bottom.Top = rect_Piece.Top + (rect_Piece.Height / 10);
            }
            else
            {
                TempSize = new SKSize(rect_Piece.Width, rect_Piece.Height / 2);
                rect_Bottom.Top = rect_Piece.Top + (rect_Piece.Height / 2);
            }
            rect_Top.Size = TempSize;
            rect_Bottom.Size = TempSize;
            SKPoint firstPoint;
            SKPoint secondPoint;
            firstPoint = new SKPoint(rect_Bottom.Left, rect_Bottom.Bottom);
            secondPoint = new SKPoint(rect_Bottom.Right, rect_Bottom.Bottom);
            SKColor firstColor;
            SKColor secondColor;
            firstColor = new SKColor(255, 255, 255, 100); // 60 instead of 100 seems to do the trick
            secondColor = new SKColor(0, 0, 0, 150);
            SKPaint firstLine = new SKPaint();
            {
                var withBlock = firstLine;
                withBlock.Style = SKPaintStyle.Stroke;
                withBlock.Color = new SKColor(0, 0, 0, 255);
                withBlock.IsStroke = true;
                withBlock.StrokeWidth = rect_Bottom.Width / 30;
                withBlock.IsAntialias = true;
            }
            SKColor[] colors;
            colors = new SKColor[2];
            colors[0] = firstColor;
            colors[1] = secondColor;
            SKPaint thisPaint = new SKPaint();
            thisPaint.Style = SKPaintStyle.Fill;
            thisPaint.IsAntialias = true; // i think
            thisPaint.Shader = SKShader.CreateLinearGradient(firstPoint, secondPoint, colors, null, SKShaderTileMode.Clamp);
            SKPoint newPoint;
            newPoint = new SKPoint(rect_Bottom.Left + (rect_Bottom.Width / 2), rect_Bottom.Top + (rect_Bottom.Height / 2));
            int x;
            var temps = rect_Bottom.Width / 2;
            x = (int)temps;
            int y;
            temps = rect_Bottom.Height / 2;
            y = (int)temps;
            dc.DrawOval(newPoint.X, newPoint.Y, x, y, MainPaint);
            dc.DrawOval(newPoint.X, newPoint.Y, x, y, thisPaint); // can't get exact.  hopefully close enough when we combine everything.
            dc.DrawOval(newPoint.X, newPoint.Y, x - 1, y - 1, firstLine);
            SKRect tempRect = new SKRect();
            tempRect.Left = rect_Top.Left;
            tempRect.Top = rect_Top.Top + (rect_Top.Height / 2);
            TempSize = new SKSize(rect_Top.Width, rect_Bottom.Top - rect_Top.Top);
            tempRect.Size = TempSize;
            dc.DrawRect(tempRect, MainPaint);
            dc.DrawRect(tempRect, thisPaint);
            dc.DrawLine(rect_Top.Left, rect_Top.Top + (rect_Top.Height / 2), rect_Top.Left, rect_Bottom.Top + (rect_Bottom.Height / 2), firstLine);
            dc.DrawLine(rect_Top.Left + rect_Top.Width, rect_Top.Top + (rect_Top.Height / 2), rect_Top.Left + rect_Top.Width, rect_Bottom.Top + (rect_Bottom.Height / 2), firstLine);
            SKPoint finalPoint;
            finalPoint = new SKPoint(rect_Top.Left + (rect_Top.Width / 2), rect_Top.Top + (rect_Top.Height / 2));
            temps = rect_Top.Width / 2;
            x = (int)temps;
            temps = rect_Top.Height / 2;
            y = (int)temps;
            dc.DrawOval(finalPoint.X, finalPoint.Y, x, y, MainPaint);
            dc.DrawOval(finalPoint.X, finalPoint.Y, x, y, firstLine);
        }
        private SKPaint _noImageM;
        public override void DrawImage(SKCanvas dc)
        {
            int int_Height = 0;
            if (NeedsToClear == true)
                dc.Clear();
            var rect_Piece = GetMainRect();
            DrawSelector(dc);
            if (HasImage == true)
            {
                SKRect rect_Bottom = new SKRect();
                SKSize thisSize = new SKSize(rect_Piece.Width, int_Height);
                rect_Bottom.Left = rect_Piece.Left;
                SKRect rect_Top = new SKRect();
                rect_Top.Left = rect_Piece.Left;
                rect_Top.Top = rect_Piece.Top;
                if (FlatPiece == true)
                {
                    var temps = ActualHeight / 2 + (ActualHeight / 8);
                    int_Height = (int)temps;
                    temps = rect_Piece.Top + (ActualHeight * 3 / 8);
                    rect_Bottom.Top = (int)temps;
                    thisSize.Height = int_Height;
                }
                else
                {
                    var temps = ActualHeight * 9 / 10;
                    int_Height = (int)temps;
                    thisSize.Height = int_Height;
                    rect_Bottom.Top = rect_Piece.Top + (rect_Piece.Height / 10);
                }
                rect_Bottom.Size = thisSize;
                rect_Top.Size = thisSize;
                DrawPiece(dc, rect_Bottom);
                if (IsCrowned == true)
                    DrawPiece(dc, rect_Top);
            }
            else
            {
                dc.DrawCircle(rect_Piece.MidX, rect_Piece.MidY, rect_Piece.MidX, _noImageM);
            }
        }
        public CheckerPiecesCP()
        {
            HighlightTransparent = true;
            _noImageM = MiscHelpers.GetSolidPaint(SKColors.White); // defaults that.
        }
    }
}