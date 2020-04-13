using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using static SkiaSharpGeneralLibrary.SKExtensions.MiscHelpers;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics
{
    public sealed class BaseDeckGraphicsCP : BaseGraphicsCP
    {
        public SKPaint BitPaint;

        public IDeckGraphicsCP? ThisGraphics; //so this would actually use the functions needed.

        private EnumRotateCategory _angle;
        public EnumRotateCategory Angle
        {
            get
            {
                return _angle;
            }

            set
            {
                if (SetProperty(ref _angle, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate(); //so for games like mexican train dominos where it does not have to immediately redraw, will work.
                }
            }
        }

        private bool _isUnknown;
        public bool IsUnknown
        {
            get
            {
                return _isUnknown;
            }

            set
            {
                if (SetProperty(ref _isUnknown, value) == true)
                    PaintUI!.DoInvalidate();
            }
        }
        public SKRect GetDefaultRect()
        {
            if (Angle == EnumRotateCategory.None || Angle == EnumRotateCategory.FlipOnly180)
                return SKRect.Create(Location.X, Location.Y, (float)ActualWidth, (float)ActualHeight);
            else
                return SKRect.Create(Location.X, Location.Y, (float)ActualHeight, (float)ActualWidth); //maybe this is how it has to be in the new (?)
        }
        public SKRect GetDrawingRectangle()
        {
            return GetDefaultRect();
        }
        public float RoundedRadius = 6;
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint ThisPaint)
        {
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, ThisPaint);
        }
        private string _backgroundColor = cs.White;
        public string BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if (SetProperty(ref _backgroundColor, value) == true)
                    PaintUI!.DoInvalidate();
            }
        }
        public SKColor GetFillColor() // the color cards may override this.
        {
            SKColor fillColor;
            if (BackgroundColor == null)
                fillColor = SKColors.White;
            else
                fillColor = BackgroundColor.ToSKColor();
            return fillColor;
        }
        public SKPaint GetStandardDrewPaint()
        {
            SKColor thisColor = SKColors.Lime; //we can change as needed.
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 30);
            return GetSolidPaint(otherColor);
        }
        public SKPaint GetDisablePaint()
        {
            SKColor thisColor = SKColors.Black; //some transparency.
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 30);
            return GetSolidPaint(otherColor);
        }
        public static SKPaint GetStandardSelectPaint()
        {
            SKColor thisColor = SKColors.Red;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 30); //can experiment as needed.
            return GetSolidPaint(otherColor);
        }
        public static SKPaint GetDarkerSelectPaint()
        {
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            return GetSolidPaint(otherColor);
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (NeedsToClear == true)
                dc.Clear();
            if (ThisGraphics == null)
                throw new BasicBlankException("There is no interface to handle drawing.  Can't use dependency injection because there could be several implementations on games like think twice");
            if (ThisGraphics.CanStartDrawing() == false)
                return;
            SKRect rect_Card = ThisGraphics.GetDrawingRectangle();
            if (Angle != EnumRotateCategory.None)
            {
                dc.Save();
                dc.RotateDrawing(Angle, rect_Card);
            }
            SKColor fillColor = ThisGraphics.GetFillColor();
            SKPaint thisPaint = GetSolidPaint(fillColor);
            ThisGraphics.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            ThisGraphics.DrawBorders(dc, rect_Card); //this is responsible for figuring out the border widths, etc.
            if (IsUnknown == true && ThisGraphics.NeedsToDrawBacks == true)
            {
                ThisGraphics.DrawBacks(dc, rect_Card);
                if (IsSelected == true)
                    ThisGraphics.DrawBorders(dc, rect_Card); //needs to redo so it can do the highlighting.  hopefully that works.
                dc.Restore();
                return;
            }
            ThisGraphics.DrawImage(dc, rect_Card);
            dc.Restore();
        }
        public BaseDeckGraphicsCP()
        {
            DefaultFont = "Tahoma";
            BitPaint = GetBitmapPaint();
            NeedsHighlighting = true;
            UsedSelected = true;
        }
    }
}