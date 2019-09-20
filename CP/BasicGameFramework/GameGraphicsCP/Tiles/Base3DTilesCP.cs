using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFramework.GameGraphicsCP.Tiles
{
    public sealed class Base3DTilesCP : ObservableObject
    {
        public IRepaintControl? PaintUI;
        public BaseDeckGraphicsCP? ThisGraphics;
        private bool _Is3D; // needs to be here even though the bindings won't be automatic.  that way if 3d, then the base class won't fill everything.
        public bool Is3D
        {
            get
            {
                return _Is3D;
            }
            set
            {
                if (SetProperty(ref _Is3D, value) == true)
                    PaintUI?.DoInvalidate();
            }
        }
        public float HeightFor3D = 5; // default to 10 but inherited classes can change it
        public float WidthFor3D = 5; // again, default is 10 but can be adjusted  10 seemed too large
        private bool _NeedsLeft;
        public bool NeedsLeft
        {
            get
            {
                return _NeedsLeft;
            }

            set
            {
                if (SetProperty(ref _NeedsLeft, value) == true)
                    PaintUI?.DoInvalidate();
            }
        }
        private bool _NeedsRight;
        public bool NeedsRight
        {
            get
            {
                return _NeedsRight;
            }
            set
            {
                if (SetProperty(ref _NeedsRight, value) == true)
                    PaintUI?.DoInvalidate();
            }
        }
        private bool _NeedsTop;
        public bool NeedsTop
        {
            get
            {
                return _NeedsTop;
            }
            set
            {
                if (SetProperty(ref _NeedsTop, value) == true)
                    PaintUI?.DoInvalidate();
            }
        }
        private bool _NeedsBottom;
        public bool NeedsBottom
        {
            get
            {
                return _NeedsBottom;
            }
            set
            {
                if (SetProperty(ref _NeedsBottom, value) == true)
                    PaintUI?.DoInvalidate();
            }
        }

        public SKPaint FillSilver; //so the one that needs to draw can use this to help draw.
        public Base3DTilesCP()
        {
            FillSilver = MiscHelpers.GetSolidPaint(SKColors.Silver);
        }
        public float Margins3D = 5; // default is 5 all around.
        public SKRect Get3DBottomRect()
        {
            var firstRect = ThisGraphics!.GetDefaultRect();
            return SKRect.Create(firstRect.Left, firstRect.Bottom - HeightFor3D, firstRect.Width, HeightFor3D);
        }
        public SKRect Get3DRightRect()
        {
            var firstRect = ThisGraphics!.GetDefaultRect();
            return SKRect.Create(firstRect.Right - WidthFor3D, firstRect.Top, WidthFor3D, firstRect.Height);
        }
        public SKRect Get3DTopRect()
        {
            var firstRect = ThisGraphics!.GetDefaultRect();
            return SKRect.Create(firstRect.Left, firstRect.Top, firstRect.Width, HeightFor3D);
        }
        public SKRect Get3DLeftRect()
        {
            var firstRect = ThisGraphics!.GetDefaultRect();
            return SKRect.Create(firstRect.Left, firstRect.Top, WidthFor3D, firstRect.Height);
        }
        public SKRect GetDrawingRectangle()
        {
            if (Is3D == false)
                return ThisGraphics!.GetDrawingRectangle();
            if (NeedsBottom == false && NeedsLeft == false && NeedsTop == false && NeedsRight == false)
                return ThisGraphics!.GetDrawingRectangle();// because its also not 3d this time.
            var startRect = ThisGraphics!.GetDefaultRect();
            var location = startRect.Location;
            var size = startRect.Size;
            if (NeedsTop == true)
            {
                location.Y += Margins3D;
                size.Height -= Margins3D;
            }
            if (NeedsBottom == true)
                size.Height -= Margins3D;
            if (NeedsLeft == true)
            {
                location.X += Margins3D;
                size.Width -= Margins3D;
            }
            if (NeedsRight == true)
                size.Width -= Margins3D;
            return SKRect.Create(location, size);
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            if (Is3D == false)
            {
                ThisGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
                return;
            }
            if (NeedsBottom == false && NeedsLeft == false && NeedsTop == false && NeedsRight == false)
            {
                ThisGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
                return;
            }
            dc.DrawRect(rect_Card, thisPaint);
        }
        public void Draw3D(SKCanvas thisCanvas)
        {
            if (NeedsBottom == true)
            {
                var BottomRect = Get3DBottomRect();
                thisCanvas.DrawRect(BottomRect, FillSilver);
            }
            if (NeedsRight == true)
            {
                var RightRect = Get3DRightRect();
                thisCanvas.DrawRect(RightRect, FillSilver);
            }
            if (NeedsTop == true)
            {
                var TopRect = Get3DTopRect();
                thisCanvas.DrawRect(TopRect, FillSilver);
            }
            if (NeedsLeft == true)
            {
                var LeftRect = Get3DLeftRect();
                thisCanvas.DrawRect(LeftRect, FillSilver);
            }
        }
    }
}