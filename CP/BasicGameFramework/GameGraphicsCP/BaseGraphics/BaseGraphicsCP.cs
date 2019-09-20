using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFramework.GameGraphicsCP.BaseGraphics
{
    public abstract class BaseGraphicsCP : ObservableObject
    {
        public IRepaintControl? PaintUI; //has to be public now.
        public double ActualHeight { get; set; }
        public double ActualWidth { get; set; }
        public double MinimumHeight { get; set; }
        public double MinimumWidth { get; set; }
        private string _MainColor = cs.Transparent;
        public string MainColor //here too.
        {
            get { return _MainColor; }
            set
            {
                if (SetProperty(ref _MainColor, value))
                {
                    UpdateMainPaint(); //this could still be done i think
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        public bool NeedsToClear { get; set; } = true;
        private bool _NeedsHighlighting = false; // has to be here.  otherwise, games like checkers won't do this.  the color control knows nothing of checkers.
        public bool NeedsHighlighting
        {
            get
            {
                return _NeedsHighlighting;
            }
            set
            {
                if (SetProperty(ref _NeedsHighlighting, value) == true)
                    UsedSelected = value;
            }
        }
        protected bool HighlightTransparent = false;
        private readonly SKPaint _limePaint;
        private readonly SKPaint _aquaPaint;
        private readonly SKPaint _disablePaint;
        public BaseGraphicsCP()
        {
            MinimumHeight = 20;
            MinimumWidth = 20;
            MainPaint = new SKPaint();
            MainPaint.Style = SKPaintStyle.Fill;
            MainPaint.IsAntialias = true;
            _limePaint = MiscHelpers.GetSolidPaint(SKColors.LimeGreen);
            _aquaPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _disablePaint = MiscHelpers.GetSolidPaint(SKColors.LightGray); // i think
            UpdateMainPaint();
        }
        public SKPoint Location { get; set; } = new SKPoint(0, 0); // for board games (especially for mobile), this has to be done unfortunately.
        public SKSize OriginalSize { get; set; } // hopefully no repainting for this one.  this needs to be everywhere
        public SKRect GetActualRectangle(int x, int y, int width, int height)
        {
            return GetActualRectangle(SKRect.Create(x, y, width, height));
        }
        public SKRect GetActualRectangle(SKRect originalRectangle)
        {
            SKPoint oldPoint;
            oldPoint = new SKPoint(originalRectangle.Left, originalRectangle.Top);
            SKSize oldSize;
            oldSize = new SKSize(originalRectangle.Width, originalRectangle.Height);
            SKPoint newPoint;
            SKSize newSize;
            newPoint = GetActualPoint(oldPoint);
            newSize = GetActualSize(oldSize);
            return SKRect.Create(newPoint, newSize);
        }
        public SKPoint GetActualPoint(SKPoint pt_Current)
        {
            int int_X;
            int int_Y;
            var tempX = ((ActualWidth / OriginalSize.Width) * pt_Current.X);
            var tempY = ((ActualHeight / OriginalSize.Height) * pt_Current.Y);
            int_X = (int)tempX;
            int_Y = (int)tempY;
            return new SKPoint(int_X, int_Y);
        }
        public SKSize GetActualSize(SKSize size_Current)
        {
            int int_Width;
            int int_Height;
            var tempWidth = ((ActualWidth / OriginalSize.Width) * size_Current.Width);
            var tempHeight = ((ActualHeight / OriginalSize.Height) * size_Current.Height);
            int_Width = (int)tempWidth;
            int_Height = (int)tempHeight;
            return new SKSize(int_Width, int_Height);
        }
        public SKSize GetActualSize(float width, float height)
        {
            SKSize thisSize = new SKSize(width, height);
            return GetActualSize(thisSize);
        }
        public float GetFontSize(float originalFontSize)
        {
            var diffs = ActualHeight / OriginalSize.Height;
            var temps = originalFontSize * diffs;
            return (float)temps;
        }
        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (SetProperty(ref _IsSelected, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    if (UsedSelected == true)
                        PaintUI.DoInvalidate();
                }
            }
        }
        private bool _IsEnabled; // this will hint about whether you can even click.  some pieces require this.
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (SetProperty(ref _IsEnabled, value) == true)
                {
                    if (PaintUI == null)
                        return; //this will solve one of the problems
                    PaintUI.DoInvalidate();
                }
            }
        }
        public bool UsedSelected = false; // the checker piece will not use that part.  if that changes, can do.
        private void UpdateMainPaint() // the skpaint is where i would do the extensions.
        {
            MainPaint.Color = SKColor.Parse(MainColor);
        }
        protected SKPaint MainPaint; //this could still be okay (?)
        public SKRect GetMainRect() // needs to be public now so the position helpers can do what it needs to.
        {
            return SKRect.Create(Location.X, Location.Y, (float)ActualWidth, (float)ActualHeight); // now has to use .create since its not 0, 0 anymore.
        }
        protected void DrawSelector(SKCanvas dc) //iffy
        {
            if (NeedsHighlighting == false)
                return;
            var thisRect = GetMainRect();
            if (IsSelected == false)
            {
                if (IsEnabled == false)
                {
                    dc.DrawRect(thisRect, _disablePaint);
                    return;
                }
                if (HighlightTransparent == true)
                    return;
                dc.DrawRect(thisRect, GetFillPaint());
                return; // because did not select
            }
            dc.DrawRect(thisRect, _limePaint);
        }
        protected virtual SKPaint GetFillPaint() // so games like countdown, can have a different color to show hints and use a different color to symbolize the hints.  even if that is not the case, has the option for a different color so something can inherit from this one.
        {
            return _aquaPaint; // they will inherit not from this one but from the numberpiece
        }
        public abstract void DrawImage(SKCanvas dc); // this is where all the drawings take place
    }
}