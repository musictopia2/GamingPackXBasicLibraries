using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Tiles;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
using System.Reflection;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseMahjongTilesCP
{
    public class MahjongTilesGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        #region Basics
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public Base3DTilesCP? TileGraphics;
        private Assembly? _thisAssembly;
        private BaseMahjongGlobals? _thisGlobal; //this will not be used in unit testing.
        private SKPaint? _pDrewPaint;
        private SKPaint? _selectPaint;
        public void Init()
        {
            if (MainGraphics == null)
                throw new BasicBlankException("You never sent in the main graphics for helpers");
            if (TileGraphics == null)
                throw new BasicBlankException("You never sent in the tile graphics to help with drawing 3d tiles.");
            MainGraphics.ActualHeight = 39;
            MainGraphics.ActualWidth = 29;
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            GetFills(); //because you need the skpaint from main graphics.
            GetHatchPaints();
            GetPens();
            _thisGlobal = Resolve<BaseMahjongGlobals>();
        }
        #endregion
        #region Paint Processes
        private SKPaint? _largeDarkBluePen;
        private SKPaint? _largeRedPen;
        private SKPaint? _smallDarkBluePen;
        private SKPaint? _smallRedPen;
        private SKPaint? _smallDarkGreenPen;
        private SKPaint? _smallBlackPen;
        private SKPaint? _smallWhitePen;
        private SKPaint? _smallDarkRedPen;
        private SKPaint? _smallOrchidPen;
        private void GetPens()
        {
            _largeDarkBluePen = MiscHelpers.GetStrokePaint(SKColors.DarkBlue, 2);
            _largeRedPen = MiscHelpers.GetStrokePaint(SKColors.Red, 2);
            _smallDarkBluePen = MiscHelpers.GetStrokePaint(SKColors.DarkBlue, 1);
            _smallRedPen = MiscHelpers.GetStrokePaint(SKColors.Red, 1);
            _smallDarkGreenPen = MiscHelpers.GetStrokePaint(SKColors.DarkGreen, 1);
            _smallBlackPen = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _smallWhitePen = MiscHelpers.GetStrokePaint(SKColors.White, 1);
            _smallDarkRedPen = MiscHelpers.GetStrokePaint(SKColors.DarkRed, 1);
            _smallOrchidPen = MiscHelpers.GetStrokePaint(SKColors.Orchid, 1);
        }
        private SKPaint? _fillRed;
        private SKPaint? _fillDarkBlue;
        private SKPaint? _fillWhite;
        private SKPaint? _fillBlack;
        private SKPaint? _fillGreen;
        private SKPaint? _fillDarkRed;
        private SKPaint? _fillOrchid;
        private SKPaint? _fillYellow;
        private SKPaint? _fillLimeGreen;
        private SKPaint? _fillGoldenRod;
        private SKPaint? _fillSalmon;
        private SKPaint? _fillSteelBlue;
        private void GetFills()
        {
            _fillRed = MiscHelpers.GetSolidPaint(SKColors.Red);
            _fillDarkBlue = MiscHelpers.GetSolidPaint(SKColors.DarkBlue);
            _fillWhite = MiscHelpers.GetSolidPaint(SKColors.White);
            _fillBlack = MiscHelpers.GetSolidPaint(SKColors.Black);
            _fillGreen = MiscHelpers.GetSolidPaint(SKColors.Green);
            _fillDarkRed = MiscHelpers.GetSolidPaint(SKColors.DarkRed);
            _fillOrchid = MiscHelpers.GetSolidPaint(SKColors.Orchid);
            _fillYellow = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _fillLimeGreen = MiscHelpers.GetSolidPaint(SKColors.LimeGreen);
            _fillGoldenRod = MiscHelpers.GetSolidPaint(SKColors.Goldenrod);
            _fillSalmon = MiscHelpers.GetSolidPaint(SKColors.Salmon);
            _fillSteelBlue = MiscHelpers.GetSolidPaint(SKColors.SteelBlue);
        }
        private SKPaint? _hatchDarkBlueWhite30;
        private SKPaint? _hatchDarkGreenGreen30;
        private SKPaint? _hatchGreenTransparent75;
        private SKPaint? _hatchRedTransparrent20;
        private SKPaint? _hatchRedWhite50;
        private SKPaint? _hatchWhiteDarkBlue50;
        private SKPaint? _hatchWhiteRed50;
        private SKPaint? _hatchYellowTransparent40;
        private SKPaint GetPaint(string path)
        {
            var thisPaint = MiscHelpers.GetBitmapPaint();
            thisPaint.Shader = ImageExtensions.GetSkShader(_thisAssembly!, path);
            return thisPaint;
        }
        private void GetHatchPaints()
        {
            _hatchDarkBlueWhite30 = GetPaint("darkbluewhite30.png");
            _hatchDarkGreenGreen30 = GetPaint("darkgreengreen30.png");
            _hatchGreenTransparent75 = GetPaint("greentransparent75.png");
            _hatchRedTransparrent20 = GetPaint("redtransparent20.png");
            _hatchRedWhite50 = GetPaint("redwhite50.png");
            _hatchWhiteDarkBlue50 = GetPaint("whitedarkblue50.png");
            _hatchWhiteRed50 = GetPaint("whitered50.png");
            _hatchYellowTransparent40 = GetPaint("yellowtransparent40.png");
        }
        #endregion
        #region Properties
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                    MainGraphics!.PaintUI?.DoInvalidate();
            }
        }
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                    MainGraphics!.PaintUI?.DoInvalidate();
            }
        }
        private bool _is3D;
        public bool Is3D
        {
            get { return _is3D; }
            set
            {
                if (SetProperty(ref _is3D, value)) { }
            }
        }
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (SetProperty(ref _index, value) == true)
                    MainGraphics!.PaintUI?.DoInvalidate(); //just piggy back on that one.
            }
        }
        #endregion
        #region Inherited Stuff
        public bool NeedsToDrawBacks => false; //i think this one has no backs to draw.
        public bool CanStartDrawing()
        {
            return _thisGlobal!.CanShowTiles;
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
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            throw new BasicBlankException("You should have called the NeedsToDrawBacks to show there is no backs to draw");
        }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetMainRect(); //i think.  if i am wrong, rethink.
        }
        public void DrawImage(SKCanvas thisCanvas, SKRect bounds)
        {
            int int_Value;
            int_Value = Index;
            SKRect rect1;
            SKRect rect2;
            SKRect rect3;
            switch (int_Value)
            {
                case 1:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 9), bounds.Top + (bounds.Height / 2) - (bounds.Width * 7 / 18), bounds.Width * 7 / 9, bounds.Width * 7 / 9);
                        thisCanvas.DrawOval(rect1, _hatchDarkBlueWhite30);
                        thisCanvas.DrawOval(rect1, _largeDarkBluePen);
                        rect2 = SKRect.Create(bounds.Left + (bounds.Width / 4), bounds.Top + (bounds.Height / 2) - (bounds.Width / 4), bounds.Width / 2, bounds.Width / 2);
                        thisCanvas.DrawOval(rect2, _largeDarkBluePen);
                        rect3 = SKRect.Create(rect2.Left + 2, rect2.Top + 2, rect2.Width - 4, rect2.Height - 4);
                        thisCanvas.DrawOval(rect3, _hatchRedWhite50);
                        thisCanvas.DrawOval(rect3, _largeRedPen);
                        break;
                    }

                case 2:
                    {

                        rect1 = SKRect.Create(bounds.Left + (float)(bounds.Width * 0.3), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.45f), bounds.Width * 0.4f, bounds.Width * 0.4f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (float)(bounds.Width * 0.3), bounds.Top + (bounds.Height / 2) + (bounds.Width * 0.05f), bounds.Width * 0.4f, bounds.Width * 0.4f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        break;
                    }

                case 3:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.1f), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.19f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.19f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width) - (bounds.Width * 0.1f) - (bounds.Width * 0.38f), bounds.Top + (bounds.Height) - (bounds.Width * 0.05f) - (bounds.Width * 0.38f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        break;
                    }

                case 4:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.05f), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.55f), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.05f), bounds.Top + bounds.Height - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.55f), bounds.Top + bounds.Height - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        break;
                    }

                case 5:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.05f), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.55f), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.05f), bounds.Top + bounds.Height - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.55f), bounds.Top + bounds.Height - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.19f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.19f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        break;
                    }

                case 6:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - bounds.Width * 0.38f, bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Width * 0.05f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - bounds.Width * 0.38f, bounds.Top + (bounds.Height) - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height) - (bounds.Width * 0.43f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - bounds.Width * 0.38f, bounds.Top + (bounds.Height) - (bounds.Width * 0.4f) - (bounds.Width * 0.38f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height) - (bounds.Width * 0.43f) - (bounds.Width * 0.38f), bounds.Width * 0.38f, bounds.Width * 0.38f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        break;
                    }

                case 7:
                    {
                        rect1 = SKRect.Create(bounds.Left + bounds.Width * 0.05f, bounds.Top, bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.17f), bounds.Top + (bounds.Width * 0.1f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.95f) - (bounds.Width * 0.34f), bounds.Top + (bounds.Width * 0.2f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);

                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - bounds.Width * 0.34f, bounds.Top + (bounds.Height) - (bounds.Width * 0.39f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height) - (bounds.Width * 0.39f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - bounds.Width * 0.34f, bounds.Top + (bounds.Height) - (bounds.Width * 0.39f) - (bounds.Width * 0.34f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height) - (bounds.Width * 0.39f) - (bounds.Width * 0.34f), bounds.Width * 0.34f, bounds.Width * 0.34f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        break;
                    }

                case 8:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.18f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.62f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.52f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.62f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.18f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.31f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.52f), bounds.Top + (bounds.Height / 2) - (bounds.Width * 0.31f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.18f), bounds.Top + (bounds.Height / 2) + (bounds.Width * 0.01f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.52f), bounds.Top + (bounds.Height / 2) + (bounds.Width * 0.01f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.18f), bounds.Top + (bounds.Height / 2) + (bounds.Width * 0.32f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.52f), bounds.Top + (bounds.Height / 2) + (bounds.Width * 0.32f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        break;
                    }

                case 9:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.02f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.68f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);

                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.02f), bounds.Top + (bounds.Height * 0.5f) - (bounds.Width * 0.15f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.5f) - (bounds.Width * 0.15f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.68f), bounds.Top + (bounds.Height * 0.5f) - (bounds.Width * 0.15f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallRedPen!, _hatchWhiteRed50!);

                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.02f), bounds.Top + (bounds.Height * 0.9f) - (bounds.Width * 0.3f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.9f) - (bounds.Width * 0.3f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width * 0.68f), bounds.Top + (bounds.Height * 0.9f) - (bounds.Width * 0.3f), bounds.Width * 0.3f, bounds.Width * 0.3f);
                        DrawSmallCircle(thisCanvas, rect1, _smallDarkBluePen!, _hatchWhiteDarkBlue50!);
                        break;
                    }

                case 10:
                    {
                        DrawOwl(thisCanvas, bounds);
                        break;
                    }

                case 11:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 12:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 13:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 14:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height / 2) - (bounds.Height * 0.15f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillRed!, _smallRedPen!);
                        break;
                    }

                case 15:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.6f), bounds.Width * 0.2f, bounds.Height * 0.3f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 16:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillRed!, _smallRedPen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 17:
                    {
                        Draw17Bamboo(thisCanvas, bounds);
                        break;
                    }

                case 18:
                    {
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillRed!, _smallRedPen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillRed!, _smallRedPen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.37f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) - (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillRed!, _smallRedPen!);
                        rect1 = SKRect.Create(bounds.Left + (bounds.Width / 2) + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.7f), bounds.Width * 0.2f, bounds.Height * 0.25f);
                        DrawBamboo(thisCanvas, rect1, _fillDarkBlue!, _smallDarkBluePen!);
                        break;
                    }

                case 19:
                    {
                        DrawNumberTile(thisCanvas, bounds, 1);
                        break;
                    }

                case 20:
                    {
                        DrawNumberTile(thisCanvas, bounds, 2);
                        break;
                    }

                case 21:
                    {
                        DrawNumberTile(thisCanvas, bounds, 3);
                        break;
                    }

                case 22:
                    {
                        DrawNumberTile(thisCanvas, bounds, 4);
                        break;
                    }

                case 23:
                    {
                        DrawNumberTile(thisCanvas, bounds, 5);
                        break;
                    }

                case 24:
                    {
                        DrawNumberTile(thisCanvas, bounds, 6);
                        break;
                    }

                case 25:
                    {
                        DrawNumberTile(thisCanvas, bounds, 7);
                        break;
                    }

                case 26:
                    {
                        DrawNumberTile(thisCanvas, bounds, 8);
                        break;
                    }

                case 27:
                    {
                        DrawNumberTile(thisCanvas, bounds, 9);
                        break;
                    }

                case 28:
                    {
                        DrawDirectionTile(thisCanvas, bounds, "E");
                        break;
                    }

                case 29:
                    {
                        DrawDirectionTile(thisCanvas, bounds, "S");
                        break;
                    }

                case 30:
                    {
                        DrawDirectionTile(thisCanvas, bounds, "W");
                        break;
                    }

                case 31:
                    {
                        DrawDirectionTile(thisCanvas, bounds, "N");
                        break;
                    }

                case 32:
                    {
                        DrawTile32(thisCanvas, bounds);
                        break;
                    }

                case 33:
                    {
                        DrawTile33(thisCanvas, bounds);
                        break;
                    }

                case 34:
                    {
                        DrawTile34(thisCanvas, bounds);
                        break;
                    }

                case 35:
                    {
                        DrawSeasonTile(thisCanvas, bounds, "SPR");
                        break;
                    }

                case 36:
                    {
                        DrawSeasonTile(thisCanvas, bounds, "SUM");
                        break;
                    }

                case 37:
                    {
                        DrawSeasonTile(thisCanvas, bounds, "AUT");
                        break;
                    }

                case 38:
                    {
                        DrawSeasonTile(thisCanvas, bounds, "WIN");
                        break;
                    }

                case 39:
                    {
                        DrawFlowerTile(thisCanvas, bounds, 1);
                        break;
                    }

                case 40:
                    {
                        DrawFlowerTile(thisCanvas, bounds, 2);
                        break;
                    }

                case 41:
                    {
                        DrawFlowerTile(thisCanvas, bounds, 3);
                        break;
                    }

                case 42:
                    {
                        DrawFlowerTile(thisCanvas, bounds, 4);
                        break;
                    }
            }
            if (Is3D == true)
                TileGraphics!.Draw3D(thisCanvas); //i think
        }
        #endregion
        #region Specialized Drawings
        public void DrawNumberTile(SKCanvas thisCanvas, SKRect bounds, int number)
        {
            SKPoint[] pts = new SKPoint[85];

            pts[0] = new SKPoint(bounds.Left + (0.447587354409318f * bounds.Width), bounds.Top + (0.504716981132076f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.497504159733777f * bounds.Width), bounds.Top + (0.483490566037736f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.519134775374376f * bounds.Width), bounds.Top + (0.450471698113208f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.559068219633943f * bounds.Width), bounds.Top + (0.488207547169811f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.587354409317804f * bounds.Width), bounds.Top + (0.492924528301887f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.53910149750416f * bounds.Width), bounds.Top + (0.537735849056604f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.487520798668885f * bounds.Width), bounds.Top + (0.566037735849057f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.447587354409318f * bounds.Width), bounds.Top + (0.570754716981132f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.445923460898502f * bounds.Width), bounds.Top + (0.596698113207547f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.382695507487521f * bounds.Width), bounds.Top + (0.582547169811321f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.377703826955075f * bounds.Width), bounds.Top + (0.547169811320755f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.342762063227953f * bounds.Width), bounds.Top + (0.535377358490566f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.344425956738769f * bounds.Width), bounds.Top + (0.504716981132076f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.344425956738769f * bounds.Width), bounds.Top + (0.533018867924528f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.377703826955075f * bounds.Width), bounds.Top + (0.540094339622642f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.37936772046589f * bounds.Width), bounds.Top + (0.587264150943396f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.302828618968386f * bounds.Width), bounds.Top + (0.591981132075472f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.384359400998336f * bounds.Width), bounds.Top + (0.587264150943396f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.447587354409318f * bounds.Width), bounds.Top + (0.613207547169811f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.371048252911814f * bounds.Width), bounds.Top + (0.643867924528302f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.440931780366057f * bounds.Width), bounds.Top + (0.643867924528302f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.449251247920133f * bounds.Width), bounds.Top + (0.672169811320755f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.405990016638935f * bounds.Width), bounds.Top + (0.693396226415094f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.450915141430948f * bounds.Width), bounds.Top + (0.700471698113208f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.482529118136439f * bounds.Width), bounds.Top + (0.721698113207547f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.470881863560732f * bounds.Width), bounds.Top + (0.754716981132076f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.40765391014975f * bounds.Width), bounds.Top + (0.754716981132076f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.307820299500832f * bounds.Width), bounds.Top + (0.643867924528302f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.37936772046589f * bounds.Width), bounds.Top + (0.764150943396226f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.366056572379368f * bounds.Width), bounds.Top + (0.794811320754717f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.267886855241265f * bounds.Width), bounds.Top + (0.799528301886792f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.247920133111481f * bounds.Width), bounds.Top + (0.775943396226415f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.194675540765391f * bounds.Width), bounds.Top + (0.775943396226415f * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.232945091514143f * bounds.Width), bounds.Top + (0.778301886792453f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.24459234608985f * bounds.Width), bounds.Top + (0.794811320754717f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0.226289517470882f * bounds.Width), bounds.Top + (0.820754716981132f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.123128119800333f * bounds.Width), bounds.Top + (0.820754716981132f * bounds.Height));
            pts[37] = new SKPoint(bounds.Left + (0.166389351081531f * bounds.Width), bounds.Top + (0.830188679245283f * bounds.Height));
            pts[38] = new SKPoint(bounds.Left + (0.171381031613977f * bounds.Width), bounds.Top + (0.856132075471698f * bounds.Height));
            pts[39] = new SKPoint(bounds.Left + (0.176372712146423f * bounds.Width), bounds.Top + (0.818396226415094f * bounds.Height));
            pts[40] = new SKPoint(bounds.Left + (0.242928452579035f * bounds.Width), bounds.Top + (0.837264150943396f * bounds.Height));
            pts[41] = new SKPoint(bounds.Left + (0.239600665557404f * bounds.Width), bounds.Top + (0.886792452830189f * bounds.Height));
            pts[42] = new SKPoint(bounds.Left + (0.256239600665557f * bounds.Width), bounds.Top + (0.844339622641509f * bounds.Height));
            pts[43] = new SKPoint(bounds.Left + (0.334442595673877f * bounds.Width), bounds.Top + (0.85377358490566f * bounds.Height));
            pts[44] = new SKPoint(bounds.Left + (0.356073211314476f * bounds.Width), bounds.Top + (0.877358490566038f * bounds.Height));
            pts[45] = new SKPoint(bounds.Left + (0.37603993344426f * bounds.Width), bounds.Top + (0.875f * bounds.Height));
            pts[46] = new SKPoint(bounds.Left + (0.384359400998336f * bounds.Width), bounds.Top + (0.858490566037736f * bounds.Height));
            pts[47] = new SKPoint(bounds.Left + (0.550748752079867f * bounds.Width), bounds.Top + (0.858490566037736f * bounds.Height));
            pts[48] = new SKPoint(bounds.Left + (0.555740432612313f * bounds.Width), bounds.Top + (0.834905660377358f * bounds.Height));
            pts[49] = new SKPoint(bounds.Left + (0.615640599001664f * bounds.Width), bounds.Top + (0.825471698113208f * bounds.Height));
            pts[50] = new SKPoint(bounds.Left + (0.625623960066556f * bounds.Width), bounds.Top + (0.816037735849057f * bounds.Height));
            pts[51] = new SKPoint(bounds.Left + (0.617304492512479f * bounds.Width), bounds.Top + (0.89622641509434f * bounds.Height));
            pts[52] = new SKPoint(bounds.Left + (0.552412645590682f * bounds.Width), bounds.Top + (0.903301886792453f * bounds.Height));
            pts[53] = new SKPoint(bounds.Left + (0.625623960066556f * bounds.Width), bounds.Top + (0.912735849056604f * bounds.Height));
            pts[54] = new SKPoint(bounds.Left + (0.642262895174709f * bounds.Width), bounds.Top + (0.936320754716981f * bounds.Height));
            pts[55] = new SKPoint(bounds.Left + (0.73044925124792f * bounds.Width), bounds.Top + (0.92688679245283f * bounds.Height));
            pts[56] = new SKPoint(bounds.Left + (0.797004991680532f * bounds.Width), bounds.Top + (0.860849056603774f * bounds.Height));
            pts[57] = new SKPoint(bounds.Left + (0.690515806988353f * bounds.Width), bounds.Top + (0.754716981132076f * bounds.Height));
            pts[58] = new SKPoint(bounds.Left + (0.605657237936772f * bounds.Width), bounds.Top + (0.747641509433962f * bounds.Height));
            pts[59] = new SKPoint(bounds.Left + (0.59234608985025f * bounds.Width), bounds.Top + (0.780660377358491f * bounds.Height));
            pts[60] = new SKPoint(bounds.Left + (0.532445923460898f * bounds.Width), bounds.Top + (0.778301886792453f * bounds.Height));
            pts[61] = new SKPoint(bounds.Left + (0.512479201331115f * bounds.Width), bounds.Top + (0.806603773584906f * bounds.Height));
            pts[62] = new SKPoint(bounds.Left + (0.386023294509151f * bounds.Width), bounds.Top + (0.797169811320755f * bounds.Height));
            pts[63] = new SKPoint(bounds.Left + (0.439267886855241f * bounds.Width), bounds.Top + (0.797169811320755f * bounds.Height));
            pts[64] = new SKPoint(bounds.Left + (0.500831946755408f * bounds.Width), bounds.Top + (0.731132075471698f * bounds.Height));
            pts[65] = new SKPoint(bounds.Left + (0.56738768718802f * bounds.Width), bounds.Top + (0.719339622641509f * bounds.Height));
            pts[66] = new SKPoint(bounds.Left + (0.618968386023295f * bounds.Width), bounds.Top + (0.683962264150943f * bounds.Height));
            pts[67] = new SKPoint(bounds.Left + (0.610648918469218f * bounds.Width), bounds.Top + (0.634433962264151f * bounds.Height));
            pts[68] = new SKPoint(bounds.Left + (0.530782029950083f * bounds.Width), bounds.Top + (0.613207547169811f * bounds.Height));
            pts[69] = new SKPoint(bounds.Left + (0.480865224625624f * bounds.Width), bounds.Top + (0.629716981132076f * bounds.Height));
            pts[70] = new SKPoint(bounds.Left + (0.482529118136439f * bounds.Width), bounds.Top + (0.665094339622642f * bounds.Height));
            pts[71] = new SKPoint(bounds.Left + (0.557404326123128f * bounds.Width), bounds.Top + (0.672169811320755f * bounds.Height));
            pts[72] = new SKPoint(bounds.Left + (0.570715474209651f * bounds.Width), bounds.Top + (0.700471698113208f * bounds.Height));
            pts[73] = new SKPoint(bounds.Left + (0.494176372712146f * bounds.Width), bounds.Top + (0.716981132075472f * bounds.Height));
            pts[74] = new SKPoint(bounds.Left + (0.482529118136439f * bounds.Width), bounds.Top + (0.674528301886792f * bounds.Height));
            pts[75] = new SKPoint(bounds.Left + (0.489184692179701f * bounds.Width), bounds.Top + (0.721698113207547f * bounds.Height));
            pts[76] = new SKPoint(bounds.Left + (0.482529118136439f * bounds.Width), bounds.Top + (0.841981132075472f * bounds.Height));
            pts[77] = new SKPoint(bounds.Left + (0.484193011647255f * bounds.Width), bounds.Top + (0.719339622641509f * bounds.Height));
            pts[78] = new SKPoint(bounds.Left + (0.465890183028286f * bounds.Width), bounds.Top + (0.636792452830189f * bounds.Height));
            pts[79] = new SKPoint(bounds.Left + (0.450915141430948f * bounds.Width), bounds.Top + (0.570754716981132f * bounds.Height));
            pts[80] = new SKPoint(bounds.Left + (0.51414309484193f * bounds.Width), bounds.Top + (0.568396226415094f * bounds.Height));
            pts[81] = new SKPoint(bounds.Left + (0.522462562396007f * bounds.Width), bounds.Top + (0.60377358490566f * bounds.Height));
            pts[82] = new SKPoint(bounds.Left + (0.524126455906822f * bounds.Width), bounds.Top + (0.575471698113208f * bounds.Height));
            pts[83] = new SKPoint(bounds.Left + (0.645590682196339f * bounds.Width), bounds.Top + (0.566037735849057f * bounds.Height));
            pts[84] = new SKPoint(bounds.Left + (0.557404326123128f * bounds.Width), bounds.Top + (0.549528301886792f * bounds.Height));
            SKPath thisPath;
            thisPath = new SKPath();
            int x;
            thisPath.MoveTo(pts[0]);
            foreach (var thisPoint in pts)
                thisPath.LineTo(thisPoint);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _smallRedPen);
            pts = new SKPoint[7];
            pts[0] = new SKPoint(bounds.Left + (0.640599001663894f * bounds.Width), bounds.Top + (0.754716981132076f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.712146422628952f * bounds.Width), bounds.Top + (0.80188679245283f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.738768718801997f * bounds.Width), bounds.Top + (0.863207547169811f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.680532445923461f * bounds.Width), bounds.Top + (0.92688679245283f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.815307820299501f * bounds.Width), bounds.Top + (0.89622641509434f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.843594009983361f * bounds.Width), bounds.Top + (0.827830188679245f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.707154742096506f * bounds.Width), bounds.Top + (0.747641509433962f * bounds.Height));
            thisPath = new SKPath();
            thisPath.MoveTo(pts[0]);
            for (x = 1; x <= 6; x++)
                thisPath.LineTo(pts[x]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _fillRed);
            DrawNumberCharacter(thisCanvas, bounds, number);
            DrawNumber(thisCanvas, bounds, number);
        }
        private void DrawSkCurves(SKCanvas thisCanvas, SKPoint[] pts, bool alsoPen = true, SKPaint? fillChoice = null)
        {
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(pts[0]);
            int x;
            if (fillChoice == null == true)
                fillChoice = _fillBlack;
            var loopTo = pts.Count() - 1;
            for (x = 1; x <= loopTo; x++)
                thisPath.LineTo(pts[x]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, fillChoice);
            if (alsoPen == true)
                thisCanvas.DrawPath(thisPath, _smallBlackPen);
        }
        private void DrawSKPenCurves(SKCanvas thisCanvas, SKPoint[] pts, SKPaint tenChoice)
        {
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(pts[0]);
            int x;
            var loopTo = pts.Count() - 1;
            for (x = 1; x <= loopTo; x++)
                thisPath.LineTo(pts[x]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, tenChoice);
        }
        public void DrawDirectionTile(SKCanvas thisCanvas, SKRect bounds, string direction)
        {
            SKPoint[] pts;
            switch (direction)
            {
                case "E":
                    {
                        pts = new SKPoint[46];
                        pts[0] = new SKPoint(bounds.Left + (0.148148148148148f * bounds.Width), bounds.Top + (0.714285714285714f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.213804713804714f * bounds.Width), bounds.Top + (0.692857142857143f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.395622895622896f * bounds.Width), bounds.Top + (0.571428571428571f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.260942760942761f * bounds.Width), bounds.Top + (0.471428571428571f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.250841750841751f * bounds.Width), bounds.Top + (0.373809523809524f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.331649831649832f * bounds.Width), bounds.Top + (0.371428571428571f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.333333333333333f * bounds.Width), bounds.Top + (0.39047619047619f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.466329966329966f * bounds.Width), bounds.Top + (0.326190476190476f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.365319865319865f * bounds.Width), bounds.Top + (0.314285714285714f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.296296296296296f * bounds.Width), bounds.Top + (0.273809523809524f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.451178451178451f * bounds.Width), bounds.Top + (0.25952380952381f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.45959595959596f * bounds.Width), bounds.Top + (0.138095238095238f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.425925925925926f * bounds.Width), bounds.Top + (0.130952380952381f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.434343434343434f * bounds.Width), bounds.Top + (0.107142857142857f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.526936026936027f * bounds.Width), bounds.Top + (0.102380952380952f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.567340067340067f * bounds.Width), bounds.Top + (0.147619047619048f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.525252525252525f * bounds.Width), bounds.Top + (0.183333333333333f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.528619528619529f * bounds.Width), bounds.Top + (0.235714285714286f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.562289562289562f * bounds.Width), bounds.Top + (0.233333333333333f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.584175084175084f * bounds.Width), bounds.Top + (0.20952380952381f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.636363636363636f * bounds.Width), bounds.Top + (0.211904761904762f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.64983164983165f * bounds.Width), bounds.Top + (0.245238095238095f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.675084175084175f * bounds.Width), bounds.Top + (0.25f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.654882154882155f * bounds.Width), bounds.Top + (0.280952380952381f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.53030303030303f * bounds.Width), bounds.Top + (0.290476190476191f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.53030303030303f * bounds.Width), bounds.Top + (0.328571428571429f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.562289562289562f * bounds.Width), bounds.Top + (0.345238095238095f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.580808080808081f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.671717171717172f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.774410774410774f * bounds.Width), bounds.Top + (0.404761904761905f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.749158249158249f * bounds.Width), bounds.Top + (0.49047619047619f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.648148148148148f * bounds.Width), bounds.Top + (0.554761904761905f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.594276094276094f * bounds.Width), bounds.Top + (0.528571428571429f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.537037037037037f * bounds.Width), bounds.Top + (0.528571428571429f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.525252525252525f * bounds.Width), bounds.Top + (0.557142857142857f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.599326599326599f * bounds.Width), bounds.Top + (0.569047619047619f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.749158249158249f * bounds.Width), bounds.Top + (0.685714285714286f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.851851851851852f * bounds.Width), bounds.Top + (0.716666666666667f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.698653198653199f * bounds.Width), bounds.Top + (0.738095238095238f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.648148148148148f * bounds.Width), bounds.Top + (0.692857142857143f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.538720538720539f * bounds.Width), bounds.Top + (0.583333333333333f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.531986531986532f * bounds.Width), bounds.Top + (0.802380952380952f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.466329966329966f * bounds.Width), bounds.Top + (0.85f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.457912457912458f * bounds.Width), bounds.Top + (0.797619047619048f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.343434343434343f * bounds.Width), bounds.Top + (0.711904761904762f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.203703703703704f * bounds.Width), bounds.Top + (0.730952380952381f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.363636363636364f * bounds.Width), bounds.Top + (0.44047619047619f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.393939393939394f * bounds.Width), bounds.Top + (0.407142857142857f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.45959595959596f * bounds.Width), bounds.Top + (0.4f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.430976430976431f * bounds.Width), bounds.Top + (0.442857142857143f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts[0] = new SKPoint(bounds.Left + (0.531986531986532f * bounds.Width), bounds.Top + (0.402380952380952f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.664983164983165f * bounds.Width), bounds.Top + (0.404761904761905f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.654882154882155f * bounds.Width), bounds.Top + (0.452380952380952f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.616161616161616f * bounds.Width), bounds.Top + (0.428571428571429f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts[0] = new SKPoint(bounds.Left + (0.533670033670034f * bounds.Width), bounds.Top + (0.488095238095238f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.643097643097643f * bounds.Width), bounds.Top + (0.473809523809524f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.632996632996633f * bounds.Width), bounds.Top + (0.497619047619048f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.545454545454545f * bounds.Width), bounds.Top + (0.495238095238095f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts[0] = new SKPoint(bounds.Left + (0.388888888888889f * bounds.Width), bounds.Top + (0.476190476190476f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.461279461279461f * bounds.Width), bounds.Top + (0.476190476190476f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.45959595959596f * bounds.Width), bounds.Top + (0.5f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.402356902356902f * bounds.Width), bounds.Top + (0.5f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts[0] = new SKPoint(bounds.Left + (0.427609427609428f * bounds.Width), bounds.Top + (0.535714285714286f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.471380471380471f * bounds.Width), bounds.Top + (0.523809523809524f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.464646464646465f * bounds.Width), bounds.Top + (0.547619047619048f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.439393939393939f * bounds.Width), bounds.Top + (0.547619047619048f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts[0] = new SKPoint(bounds.Left + (0.353535353535354f * bounds.Width), bounds.Top + (0.661904761904762f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.45959595959596f * bounds.Width), bounds.Top + (0.611904761904762f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.45959595959596f * bounds.Width), bounds.Top + (0.697619047619048f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.404040404040404f * bounds.Width), bounds.Top + (0.70952380952381f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        break;
                    }

                case "S":
                    {
                        pts = new SKPoint[69];
                        pts[0] = new SKPoint(bounds.Left + (0.366386554621849f * bounds.Width), bounds.Top + (0.129186602870813f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.442016806722689f * bounds.Width), bounds.Top + (0.141148325358852f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.504201680672269f * bounds.Width), bounds.Top + (0.19377990430622f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.45546218487395f * bounds.Width), bounds.Top + (0.222488038277512f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.452100840336134f * bounds.Width), bounds.Top + (0.287081339712919f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.576470588235294f * bounds.Width), bounds.Top + (0.291866028708134f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.576470588235294f * bounds.Width), bounds.Top + (0.342105263157895f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.431932773109244f * bounds.Width), bounds.Top + (0.342105263157895f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.388235294117647f * bounds.Width), bounds.Top + (0.425837320574163f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.423529411764706f * bounds.Width), bounds.Top + (0.454545454545455f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.435294117647059f * bounds.Width), bounds.Top + (0.435406698564593f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.490756302521008f * bounds.Width), bounds.Top + (0.41866028708134f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.515966386554622f * bounds.Width), bounds.Top + (0.375598086124402f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.579831932773109f * bounds.Width), bounds.Top + (0.413875598086124f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.719327731092437f * bounds.Width), bounds.Top + (0.423444976076555f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.801680672268908f * bounds.Width), bounds.Top + (0.504784688995215f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.736134453781513f * bounds.Width), bounds.Top + (0.739234449760766f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.643697478991597f * bounds.Width), bounds.Top + (0.822966507177033f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.59327731092437f * bounds.Width), bounds.Top + (0.763157894736842f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.489075630252101f * bounds.Width), bounds.Top + (0.700956937799043f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.534453781512605f * bounds.Width), bounds.Top + (0.69377990430622f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.648739495798319f * bounds.Width), bounds.Top + (0.748803827751196f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.704201680672269f * bounds.Width), bounds.Top + (0.58133971291866f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.640336134453781f * bounds.Width), bounds.Top + (0.464114832535885f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.546218487394958f * bounds.Width), bounds.Top + (0.447368421052632f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.521008403361345f * bounds.Width), bounds.Top + (0.473684210526316f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.505882352941176f * bounds.Width), bounds.Top + (0.480861244019139f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.568067226890756f * bounds.Width), bounds.Top + (0.504784688995215f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.564705882352941f * bounds.Width), bounds.Top + (0.526315789473684f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.482352941176471f * bounds.Width), bounds.Top + (0.528708133971292f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.46890756302521f * bounds.Width), bounds.Top + (0.569377990430622f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.596638655462185f * bounds.Width), bounds.Top + (0.578947368421053f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.6f * bounds.Width), bounds.Top + (0.614832535885168f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.472268907563025f * bounds.Width), bounds.Top + (0.626794258373206f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.480672268907563f * bounds.Width), bounds.Top + (0.669856459330144f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.46218487394958f * bounds.Width), bounds.Top + (0.748803827751196f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.433613445378151f * bounds.Width), bounds.Top + (0.755980861244019f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.421848739495798f * bounds.Width), bounds.Top + (0.643540669856459f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.327731092436975f * bounds.Width), bounds.Top + (0.655502392344498f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.359663865546218f * bounds.Width), bounds.Top + (0.619617224880383f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.423529411764706f * bounds.Width), bounds.Top + (0.58133971291866f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.418487394957983f * bounds.Width), bounds.Top + (0.552631578947368f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.336134453781513f * bounds.Width), bounds.Top + (0.559808612440191f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.376470588235294f * bounds.Width), bounds.Top + (0.533492822966507f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.435294117647059f * bounds.Width), bounds.Top + (0.509569377990431f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.460504201680672f * bounds.Width), bounds.Top + (0.476076555023923f * bounds.Height));
                        pts[46] = new SKPoint(bounds.Left + (0.450420168067227f * bounds.Width), bounds.Top + (0.45933014354067f * bounds.Height));
                        pts[47] = new SKPoint(bounds.Left + (0.420168067226891f * bounds.Width), bounds.Top + (0.478468899521531f * bounds.Height));
                        pts[48] = new SKPoint(bounds.Left + (0.359663865546218f * bounds.Width), bounds.Top + (0.483253588516746f * bounds.Height));
                        pts[49] = new SKPoint(bounds.Left + (0.312605042016807f * bounds.Width), bounds.Top + (0.516746411483254f * bounds.Height));
                        pts[50] = new SKPoint(bounds.Left + (0.278991596638655f * bounds.Width), bounds.Top + (0.497607655502392f * bounds.Height));
                        pts[51] = new SKPoint(bounds.Left + (0.248739495798319f * bounds.Width), bounds.Top + (0.5f * bounds.Height));
                        pts[52] = new SKPoint(bounds.Left + (0.179831932773109f * bounds.Width), bounds.Top + (0.5311004784689f * bounds.Height));
                        pts[53] = new SKPoint(bounds.Left + (0.228571428571429f * bounds.Width), bounds.Top + (0.595693779904306f * bounds.Height));
                        pts[54] = new SKPoint(bounds.Left + (0.265546218487395f * bounds.Width), bounds.Top + (0.736842105263158f * bounds.Height));
                        pts[55] = new SKPoint(bounds.Left + (0.20672268907563f * bounds.Width), bounds.Top + (0.77511961722488f * bounds.Height));
                        pts[56] = new SKPoint(bounds.Left + (0.142857142857143f * bounds.Width), bounds.Top + (0.717703349282297f * bounds.Height));
                        pts[57] = new SKPoint(bounds.Left + (0.14453781512605f * bounds.Width), bounds.Top + (0.564593301435407f * bounds.Height));
                        pts[58] = new SKPoint(bounds.Left + (0.173109243697479f * bounds.Width), bounds.Top + (0.538277511961722f * bounds.Height));
                        pts[59] = new SKPoint(bounds.Left + (0.215126050420168f * bounds.Width), bounds.Top + (0.488038277511962f * bounds.Height));
                        pts[60] = new SKPoint(bounds.Left + (0.263865546218487f * bounds.Width), bounds.Top + (0.41866028708134f * bounds.Height));
                        pts[61] = new SKPoint(bounds.Left + (0.305882352941176f * bounds.Width), bounds.Top + (0.442583732057416f * bounds.Height));
                        pts[62] = new SKPoint(bounds.Left + (0.361344537815126f * bounds.Width), bounds.Top + (0.425837320574163f * bounds.Height));
                        pts[63] = new SKPoint(bounds.Left + (0.361344537815126f * bounds.Width), bounds.Top + (0.380382775119617f * bounds.Height));
                        pts[64] = new SKPoint(bounds.Left + (0.250420168067227f * bounds.Width), bounds.Top + (0.354066985645933f * bounds.Height));
                        pts[65] = new SKPoint(bounds.Left + (0.263865546218487f * bounds.Width), bounds.Top + (0.327751196172249f * bounds.Height));
                        pts[66] = new SKPoint(bounds.Left + (0.38655462184874f * bounds.Width), bounds.Top + (0.299043062200957f * bounds.Height));
                        pts[67] = new SKPoint(bounds.Left + (0.389915966386555f * bounds.Width), bounds.Top + (0.191387559808612f * bounds.Height));
                        pts[68] = new SKPoint(bounds.Left + (0.369747899159664f * bounds.Width), bounds.Top + (0.188995215311005f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        break;
                    }

                case "W":
                    {
                        pts = new SKPoint[44];
                        pts[0] = new SKPoint(bounds.Left + (0.118243243243243f * bounds.Width), bounds.Top + (0.310262529832936f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.464527027027027f * bounds.Width), bounds.Top + (0.257756563245823f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.616554054054054f * bounds.Width), bounds.Top + (0.250596658711217f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.673986486486487f * bounds.Width), bounds.Top + (0.210023866348449f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.756756756756757f * bounds.Width), bounds.Top + (0.262529832935561f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.839527027027027f * bounds.Width), bounds.Top + (0.284009546539379f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.847972972972973f * bounds.Width), bounds.Top + (0.336515513126492f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.6875f * bounds.Width), bounds.Top + (0.33890214797136f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.611486486486487f * bounds.Width), bounds.Top + (0.398568019093079f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.601351351351351f * bounds.Width), bounds.Top + (0.465393794749403f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.744932432432432f * bounds.Width), bounds.Top + (0.496420047732697f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.814189189189189f * bounds.Width), bounds.Top + (0.615751789976134f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.663851351351351f * bounds.Width), bounds.Top + (0.732696897374702f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.574324324324324f * bounds.Width), bounds.Top + (0.73508353221957f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.567567567567568f * bounds.Width), bounds.Top + (0.675417661097852f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.427364864864865f * bounds.Width), bounds.Top + (0.675417661097852f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.341216216216216f * bounds.Width), bounds.Top + (0.739856801909308f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.146959459459459f * bounds.Width), bounds.Top + (0.618138424821002f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.140202702702703f * bounds.Width), bounds.Top + (0.556085918854415f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.233108108108108f * bounds.Width), bounds.Top + (0.548926014319809f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.314189189189189f * bounds.Width), bounds.Top + (0.489260143198091f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.282094594594595f * bounds.Width), bounds.Top + (0.422434367541766f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.239864864864865f * bounds.Width), bounds.Top + (0.403341288782816f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.305743243243243f * bounds.Width), bounds.Top + (0.393794749403341f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.347972972972973f * bounds.Width), bounds.Top + (0.436754176610979f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.39527027027027f * bounds.Width), bounds.Top + (0.491646778042959f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.466216216216216f * bounds.Width), bounds.Top + (0.496420047732697f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.462837837837838f * bounds.Width), bounds.Top + (0.443914081145585f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.488175675675676f * bounds.Width), bounds.Top + (0.443914081145585f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.5f * bounds.Width), bounds.Top + (0.477326968973747f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.543918918918919f * bounds.Width), bounds.Top + (0.446300715990453f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.501689189189189f * bounds.Width), bounds.Top + (0.427207637231504f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.498310810810811f * bounds.Width), bounds.Top + (0.391408114558473f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.538851351351351f * bounds.Width), bounds.Top + (0.384248210023866f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.54222972972973f * bounds.Width), bounds.Top + (0.343675417661098f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.493243243243243f * bounds.Width), bounds.Top + (0.336515513126492f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.515202702702703f * bounds.Width), bounds.Top + (0.31980906921241f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.592905405405405f * bounds.Width), bounds.Top + (0.322195704057279f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.616554054054054f * bounds.Width), bounds.Top + (0.33890214797136f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.672297297297297f * bounds.Width), bounds.Top + (0.300715990453461f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.505067567567568f * bounds.Width), bounds.Top + (0.291169451073986f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.33277027027027f * bounds.Width), bounds.Top + (0.315035799522673f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.226351351351351f * bounds.Width), bounds.Top + (0.336515513126492f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.160472972972973f * bounds.Width), bounds.Top + (0.367541766109785f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.251689189189189f * bounds.Width), bounds.Top + (0.610978520286396f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.302364864864865f * bounds.Width), bounds.Top + (0.544152744630072f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.390202702702703f * bounds.Width), bounds.Top + (0.644391408114558f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.295608108108108f * bounds.Width), bounds.Top + (0.656324582338902f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.391891891891892f * bounds.Width), bounds.Top + (0.532219570405728f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.405405405405405f * bounds.Width), bounds.Top + (0.579952267303103f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.427364864864865f * bounds.Width), bounds.Top + (0.568019093078759f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.425675675675676f * bounds.Width), bounds.Top + (0.52744630071599f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.462837837837838f * bounds.Width), bounds.Top + (0.534606205250597f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.478040540540541f * bounds.Width), bounds.Top + (0.630071599045346f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.535472972972973f * bounds.Width), bounds.Top + (0.594272076372315f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.527027027027027f * bounds.Width), bounds.Top + (0.525059665871122f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts = new SKPoint[5];
                        pts[0] = new SKPoint(bounds.Left + (0.608108108108108f * bounds.Width), bounds.Top + (0.508353221957041f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.60472972972973f * bounds.Width), bounds.Top + (0.572792362768496f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.567567567567568f * bounds.Width), bounds.Top + (0.615751789976134f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.712837837837838f * bounds.Width), bounds.Top + (0.587112171837709f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.665540540540541f * bounds.Width), bounds.Top + (0.529832935560859f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        break;
                    }

                case "N":
                    {
                        pts = new SKPoint[59];
                        pts[0] = new SKPoint(bounds.Left + (0.535353535353535f * bounds.Width), bounds.Top + (0.598574821852732f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.538720538720539f * bounds.Width), bounds.Top + (0.641330166270784f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.65993265993266f * bounds.Width), bounds.Top + (0.714964370546318f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.804713804713805f * bounds.Width), bounds.Top + (0.71021377672209f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.856902356902357f * bounds.Width), bounds.Top + (0.679334916864608f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.85016835016835f * bounds.Width), bounds.Top + (0.631828978622328f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.654882154882155f * bounds.Width), bounds.Top + (0.629453681710214f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.597643097643098f * bounds.Width), bounds.Top + (0.600950118764846f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.604377104377104f * bounds.Width), bounds.Top + (0.529691211401425f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.664983164983165f * bounds.Width), bounds.Top + (0.522565320665083f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.855218855218855f * bounds.Width), bounds.Top + (0.470308788598575f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.777777777777778f * bounds.Width), bounds.Top + (0.399049881235154f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.686868686868687f * bounds.Width), bounds.Top + (0.399049881235154f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.675084175084175f * bounds.Width), bounds.Top + (0.444180522565321f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.614478114478115f * bounds.Width), bounds.Top + (0.475059382422803f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.607744107744108f * bounds.Width), bounds.Top + (0.403800475059382f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.671717171717172f * bounds.Width), bounds.Top + (0.351543942992874f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.676767676767677f * bounds.Width), bounds.Top + (0.294536817102138f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.607744107744108f * bounds.Width), bounds.Top + (0.294536817102138f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.604377104377104f * bounds.Width), bounds.Top + (0.263657957244656f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.501683501683502f * bounds.Width), bounds.Top + (0.26603325415677f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.5f * bounds.Width), bounds.Top + (0.308788598574822f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.538720538720539f * bounds.Width), bounds.Top + (0.327790973871734f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.533670033670034f * bounds.Width), bounds.Top + (0.581947743467934f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.483164983164983f * bounds.Width), bounds.Top + (0.584323040380047f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.456228956228956f * bounds.Width), bounds.Top + (0.608076009501188f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.425925925925926f * bounds.Width), bounds.Top + (0.598574821852732f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.419191919191919f * bounds.Width), bounds.Top + (0.456057007125891f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.457912457912458f * bounds.Width), bounds.Top + (0.444180522565321f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.461279461279461f * bounds.Width), bounds.Top + (0.394299287410926f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.388888888888889f * bounds.Width), bounds.Top + (0.342042755344418f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.356902356902357f * bounds.Width), bounds.Top + (0.370546318289786f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.286195286195286f * bounds.Width), bounds.Top + (0.365795724465558f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.286195286195286f * bounds.Width), bounds.Top + (0.418052256532067f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.343434343434343f * bounds.Width), bounds.Top + (0.458432304038005f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.321548821548822f * bounds.Width), bounds.Top + (0.479809976247031f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.15993265993266f * bounds.Width), bounds.Top + (0.479809976247031f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.146464646464646f * bounds.Width), bounds.Top + (0.515439429928741f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.198653198653199f * bounds.Width), bounds.Top + (0.548693586698337f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.282828282828283f * bounds.Width), bounds.Top + (0.551068883610451f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.308080808080808f * bounds.Width), bounds.Top + (0.524940617577197f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.326599326599327f * bounds.Width), bounds.Top + (0.484560570071259f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.351851851851852f * bounds.Width), bounds.Top + (0.472684085510689f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.36026936026936f * bounds.Width), bounds.Top + (0.612826603325416f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.318181818181818f * bounds.Width), bounds.Top + (0.643705463182898f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.225589225589226f * bounds.Width), bounds.Top + (0.66270783847981f * bounds.Height));
                        pts[46] = new SKPoint(bounds.Left + (0.176767676767677f * bounds.Width), bounds.Top + (0.610451306413302f * bounds.Height));
                        pts[47] = new SKPoint(bounds.Left + (0.143097643097643f * bounds.Width), bounds.Top + (0.679334916864608f * bounds.Height));
                        pts[48] = new SKPoint(bounds.Left + (0.107744107744108f * bounds.Width), bounds.Top + (0.700712589073634f * bounds.Height));
                        pts[49] = new SKPoint(bounds.Left + (0.15993265993266f * bounds.Width), bounds.Top + (0.741092636579572f * bounds.Height));
                        pts[50] = new SKPoint(bounds.Left + (0.245791245791246f * bounds.Width), bounds.Top + (0.743467933491686f * bounds.Height));
                        pts[51] = new SKPoint(bounds.Left + (0.267676767676768f * bounds.Width), bounds.Top + (0.714964370546318f * bounds.Height));
                        pts[52] = new SKPoint(bounds.Left + (0.318181818181818f * bounds.Width), bounds.Top + (0.717339667458432f * bounds.Height));
                        pts[53] = new SKPoint(bounds.Left + (0.353535353535354f * bounds.Width), bounds.Top + (0.68646080760095f * bounds.Height));
                        pts[54] = new SKPoint(bounds.Left + (0.36026936026936f * bounds.Width), bounds.Top + (0.738717339667458f * bounds.Height));
                        pts[55] = new SKPoint(bounds.Left + (0.392255892255892f * bounds.Width), bounds.Top + (0.738717339667458f * bounds.Height));
                        pts[56] = new SKPoint(bounds.Left + (0.400673400673401f * bounds.Width), bounds.Top + (0.693586698337292f * bounds.Height));
                        pts[57] = new SKPoint(bounds.Left + (0.430976430976431f * bounds.Width), bounds.Top + (0.679334916864608f * bounds.Height));
                        pts[58] = new SKPoint(bounds.Left + (0.432659932659933f * bounds.Width), bounds.Top + (0.634204275534442f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        break;
                    }
            }
            DrawDirection(thisCanvas, bounds, direction);
        }
        private void DrawDirection(SKCanvas thisCanvas, SKRect bounds, string direction)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Blue, bounds.Height / 2.5f);
            MiscHelpers.MostlyBold = true;
            MiscHelpers.DefaultFont = "Arial";
            bounds.Left += 2;
            bounds.Top += 2;
            thisCanvas.DrawCustomText(direction, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, bounds, out _);
        }
        private void DrawSeasonText(SKCanvas thisCanvas, SKRect bounds, string season)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height / 4);
            MiscHelpers.MostlyBold = true;
            MiscHelpers.DefaultFont = "Arial";
            bounds.Top += 2;
            thisCanvas.DrawCustomText(season, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, bounds, out _);
        }
        private void DrawNumber(SKCanvas thisCanvas, SKRect bounds, int number)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Purple, bounds.Height / 2.5f);
            MiscHelpers.MostlyBold = true;
            MiscHelpers.DefaultFont = "Arial";
            bounds.Left += 2;
            bounds.Top += 2;
            thisCanvas.DrawCustomText(number.ToString(), TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, bounds, out _);
        }
        private void DrawNumberCharacter(SKCanvas thisCanvas, SKRect bounds, int number)
        {
            _ = new SKPoint[2];
            SKPoint[] pts;
            switch (number)
            {
                case 1:
                    {
                        pts = new SKPoint[9];
                        pts[0] = new SKPoint(bounds.Left + (0.138047138047138f * bounds.Width), bounds.Top + (0.282296650717703f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.476430976430976f * bounds.Width), bounds.Top + (0.217703349282297f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.735690235690236f * bounds.Width), bounds.Top + (0.184210526315789f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.872053872053872f * bounds.Width), bounds.Top + (0.251196172248804f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.806397306397306f * bounds.Width), bounds.Top + (0.330143540669856f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.745791245791246f * bounds.Width), bounds.Top + (0.260765550239234f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.488215488215488f * bounds.Width), bounds.Top + (0.241626794258373f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.419191919191919f * bounds.Width), bounds.Top + (0.26555023923445f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.227272727272727f * bounds.Width), bounds.Top + (0.320574162679426f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }

                case 2:
                    {
                        pts = new SKPoint[9];
                        pts[0] = new SKPoint(bounds.Left + (0.164154103852596f * bounds.Width), bounds.Top + (0.334123222748815f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.386934673366834f * bounds.Width), bounds.Top + (0.315165876777251f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.654941373534338f * bounds.Width), bounds.Top + (0.267772511848341f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.78894472361809f * bounds.Width), bounds.Top + (0.305687203791469f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.770519262981575f * bounds.Width), bounds.Top + (0.398104265402844f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.701842546063652f * bounds.Width), bounds.Top + (0.317535545023697f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.477386934673367f * bounds.Width), bounds.Top + (0.33175355450237f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.326633165829146f * bounds.Width), bounds.Top + (0.350710900473934f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.23785594639866f * bounds.Width), bounds.Top + (0.400473933649289f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);

                        pts = new SKPoint[8];
                        pts[0] = new SKPoint(bounds.Left + (0.263513513513513f * bounds.Width), bounds.Top + (0.180288461538462f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.425675675675676f * bounds.Width), bounds.Top + (0.137019230769231f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.587837837837838f * bounds.Width), bounds.Top + (0.122596153846154f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.652027027027027f * bounds.Width), bounds.Top + (0.163461538461538f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.52027027027027f * bounds.Width), bounds.Top + (0.158653846153846f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.402027027027027f * bounds.Width), bounds.Top + (0.192307692307692f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.324324324324324f * bounds.Width), bounds.Top + (0.225961538461538f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.295608108108108f * bounds.Width), bounds.Top + (0.199519230769231f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }

                case 3:
                    {
                        pts = new SKPoint[5];
                        pts[0] = new SKPoint(bounds.Left + (0.267226890756303f * bounds.Width), bounds.Top + (0.186320754716981f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.569747899159664f * bounds.Width), bounds.Top + (0.108490566037736f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.680672268907563f * bounds.Width), bounds.Top + (0.148584905660377f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.626890756302521f * bounds.Width), bounds.Top + (0.169811320754717f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.566386554621849f * bounds.Width), bounds.Top + (0.143867924528302f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[9];
                        pts[0] = new SKPoint(bounds.Left + (0.32998324958124f * bounds.Width), bounds.Top + (0.287735849056604f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.398659966499162f * bounds.Width), bounds.Top + (0.242924528301887f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.520938023450586f * bounds.Width), bounds.Top + (0.219339622641509f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.596314907872697f * bounds.Width), bounds.Top + (0.221698113207547f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.634840871021776f * bounds.Width), bounds.Top + (0.259433962264151f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.582914572864322f * bounds.Width), bounds.Top + (0.283018867924528f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.552763819095477f * bounds.Width), bounds.Top + (0.247641509433962f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.442211055276382f * bounds.Width), bounds.Top + (0.254716981132075f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.340033500837521f * bounds.Width), bounds.Top + (0.306603773584906f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[10];
                        pts[0] = new SKPoint(bounds.Left + (0.194304857621441f * bounds.Width), bounds.Top + (0.383529411764706f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.371859296482412f * bounds.Width), bounds.Top + (0.352941176470588f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.569514237855946f * bounds.Width), bounds.Top + (0.32f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.666666666666667f * bounds.Width), bounds.Top + (0.329411764705882f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.775544388609715f * bounds.Width), bounds.Top + (0.388235294117647f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.73534338358459f * bounds.Width), bounds.Top + (0.421176470588235f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.673366834170854f * bounds.Width), bounds.Top + (0.364705882352941f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.569514237855946f * bounds.Width), bounds.Top + (0.348235294117647f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.375209380234506f * bounds.Width), bounds.Top + (0.388235294117647f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.254606365159129f * bounds.Width), bounds.Top + (0.416470588235294f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }

                case 4:
                    {
                        pts = new SKPoint[10];
                        pts[0] = new SKPoint(bounds.Left + (0.15993265993266f * bounds.Width), bounds.Top + (0.252358490566038f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.304713804713805f * bounds.Width), bounds.Top + (0.375f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.397306397306397f * bounds.Width), bounds.Top + (0.316037735849057f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.634680134680135f * bounds.Width), bounds.Top + (0.32311320754717f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.673400673400673f * bounds.Width), bounds.Top + (0.344339622641509f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.792929292929293f * bounds.Width), bounds.Top + (0.25f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.796296296296296f * bounds.Width), bounds.Top + (0.191037735849057f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.727272727272727f * bounds.Width), bounds.Top + (0.134433962264151f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.441077441077441f * bounds.Width), bounds.Top + (0.134433962264151f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.21043771043771f * bounds.Width), bounds.Top + (0.174528301886792f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);

                        pts = new SKPoint[5];
                        pts[0] = new SKPoint(bounds.Left + (0.28956228956229f * bounds.Width), bounds.Top + (0.214622641509434f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.259259259259259f * bounds.Width), bounds.Top + (0.216981132075472f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.244107744107744f * bounds.Width), bounds.Top + (0.275943396226415f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.304713804713805f * bounds.Width), bounds.Top + (0.313679245283019f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.385521885521886f * bounds.Width), bounds.Top + (0.280660377358491f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts = new SKPoint[5];
                        pts[0] = new SKPoint(bounds.Left + (0.402356902356902f * bounds.Width), bounds.Top + (0.200471698113208f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.468013468013468f * bounds.Width), bounds.Top + (0.273584905660377f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.51010101010101f * bounds.Width), bounds.Top + (0.252358490566038f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.516835016835017f * bounds.Width), bounds.Top + (0.172169811320755f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.466329966329966f * bounds.Width), bounds.Top + (0.183962264150943f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        pts = new SKPoint[6];
                        pts[0] = new SKPoint(bounds.Left + (0.604377104377104f * bounds.Width), bounds.Top + (0.172169811320755f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.634680134680135f * bounds.Width), bounds.Top + (0.205188679245283f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.577441077441077f * bounds.Width), bounds.Top + (0.266509433962264f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.661616161616162f * bounds.Width), bounds.Top + (0.283018867924528f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.705387205387205f * bounds.Width), bounds.Top + (0.226415094339623f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.676767676767677f * bounds.Width), bounds.Top + (0.174528301886792f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillWhite);
                        break;
                    }

                case 5:
                    {
                        pts = new SKPoint[41];
                        pts[0] = new SKPoint(bounds.Left + (0.12436974789916f * bounds.Width), bounds.Top + (0.412322274881517f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.238655462184874f * bounds.Width), bounds.Top + (0.277251184834123f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.280672268907563f * bounds.Width), bounds.Top + (0.180094786729858f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.272268907563025f * bounds.Width), bounds.Top + (0.146919431279621f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.326050420168067f * bounds.Width), bounds.Top + (0.144549763033175f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.342857142857143f * bounds.Width), bounds.Top + (0.206161137440758f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.312605042016807f * bounds.Width), bounds.Top + (0.246445497630332f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.314285714285714f * bounds.Width), bounds.Top + (0.355450236966825f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.394957983193277f * bounds.Width), bounds.Top + (0.35781990521327f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.431932773109244f * bounds.Width), bounds.Top + (0.324644549763033f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.364705882352941f * bounds.Width), bounds.Top + (0.272511848341232f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.458823529411765f * bounds.Width), bounds.Top + (0.260663507109005f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.467226890756303f * bounds.Width), bounds.Top + (0.222748815165877f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.507563025210084f * bounds.Width), bounds.Top + (0.21563981042654f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.502521008403361f * bounds.Width), bounds.Top + (0.184834123222749f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.403361344537815f * bounds.Width), bounds.Top + (0.172985781990521f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.47563025210084f * bounds.Width), bounds.Top + (0.135071090047393f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.583193277310924f * bounds.Width), bounds.Top + (0.0781990521327014f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.705882352941177f * bounds.Width), bounds.Top + (0.14218009478673f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.636974789915966f * bounds.Width), bounds.Top + (0.149289099526066f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.564705882352941f * bounds.Width), bounds.Top + (0.149289099526066f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.557983193277311f * bounds.Width), bounds.Top + (0.191943127962085f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.527731092436975f * bounds.Width), bounds.Top + (0.21563981042654f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.563025210084034f * bounds.Width), bounds.Top + (0.23696682464455f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.598319327731092f * bounds.Width), bounds.Top + (0.210900473933649f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.699159663865546f * bounds.Width), bounds.Top + (0.274881516587678f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.630252100840336f * bounds.Width), bounds.Top + (0.281990521327014f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.60672268907563f * bounds.Width), bounds.Top + (0.322274881516588f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.65546218487395f * bounds.Width), bounds.Top + (0.312796208530806f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.734453781512605f * bounds.Width), bounds.Top + (0.312796208530806f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.811764705882353f * bounds.Width), bounds.Top + (0.353080568720379f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.813445378151261f * bounds.Width), bounds.Top + (0.419431279620853f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.715966386554622f * bounds.Width), bounds.Top + (0.364928909952607f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.495798319327731f * bounds.Width), bounds.Top + (0.360189573459716f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.448739495798319f * bounds.Width), bounds.Top + (0.381516587677725f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.317647058823529f * bounds.Width), bounds.Top + (0.398104265402844f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.312605042016807f * bounds.Width), bounds.Top + (0.447867298578199f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.263865546218487f * bounds.Width), bounds.Top + (0.407582938388626f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.258823529411765f * bounds.Width), bounds.Top + (0.372037914691943f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.284033613445378f * bounds.Width), bounds.Top + (0.372037914691943f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.27563025210084f * bounds.Width), bounds.Top + (0.31042654028436f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.463865546218487f * bounds.Width), bounds.Top + (0.327014218009479f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.499159663865546f * bounds.Width), bounds.Top + (0.298578199052133f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.568067226890756f * bounds.Width), bounds.Top + (0.289099526066351f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.539495798319328f * bounds.Width), bounds.Top + (0.322274881516588f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        break;
                    }

                case 6:
                    {
                        pts = new SKPoint[22];
                        pts[0] = new SKPoint(bounds.Left + (0.198996655518395f * bounds.Width), bounds.Top + (0.218009478672986f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.421404682274247f * bounds.Width), bounds.Top + (0.210900473933649f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.42809364548495f * bounds.Width), bounds.Top + (0.187203791469194f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.45819397993311f * bounds.Width), bounds.Top + (0.170616113744076f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.461538461538462f * bounds.Width), bounds.Top + (0.113744075829384f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.438127090301003f * bounds.Width), bounds.Top + (0.0876777251184834f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.489966555183946f * bounds.Width), bounds.Top + (0.0805687203791469f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.501672240802676f * bounds.Width), bounds.Top + (0.101895734597156f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.566889632107023f * bounds.Width), bounds.Top + (0.135071090047393f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.528428093645485f * bounds.Width), bounds.Top + (0.170616113744076f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.596989966555184f * bounds.Width), bounds.Top + (0.184834123222749f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.655518394648829f * bounds.Width), bounds.Top + (0.130331753554502f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.774247491638796f * bounds.Width), bounds.Top + (0.218009478672986f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.737458193979933f * bounds.Width), bounds.Top + (0.286729857819905f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.677257525083612f * bounds.Width), bounds.Top + (0.241706161137441f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.677257525083612f * bounds.Width), bounds.Top + (0.210900473933649f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.505016722408027f * bounds.Width), bounds.Top + (0.213270142180095f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.444816053511706f * bounds.Width), bounds.Top + (0.26303317535545f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.418060200668896f * bounds.Width), bounds.Top + (0.239336492890995f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.289297658862876f * bounds.Width), bounds.Top + (0.23696682464455f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.252508361204013f * bounds.Width), bounds.Top + (0.26303317535545f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.220735785953177f * bounds.Width), bounds.Top + (0.239336492890995f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[8];
                        pts[0] = new SKPoint(bounds.Left + (0.331103678929766f * bounds.Width), bounds.Top + (0.283687943262411f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.362876254180602f * bounds.Width), bounds.Top + (0.295508274231678f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.357859531772575f * bounds.Width), bounds.Top + (0.33806146572104f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.247491638795987f * bounds.Width), bounds.Top + (0.40661938534279f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.341137123745819f * bounds.Width), bounds.Top + (0.390070921985816f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.461538461538462f * bounds.Width), bounds.Top + (0.297872340425532f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.40133779264214f * bounds.Width), bounds.Top + (0.293144208037825f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.379598662207358f * bounds.Width), bounds.Top + (0.267139479905437f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[5];
                        pts[0] = new SKPoint(bounds.Left + (0.508361204013378f * bounds.Width), bounds.Top + (0.276595744680851f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.70066889632107f * bounds.Width), bounds.Top + (0.425531914893617f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.70066889632107f * bounds.Width), bounds.Top + (0.319148936170213f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.638795986622074f * bounds.Width), bounds.Top + (0.319148936170213f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.590301003344482f * bounds.Width), bounds.Top + (0.267139479905437f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false);
                        break;
                    }

                case 7:
                    {
                        pts = new SKPoint[25];
                        pts[0] = new SKPoint(bounds.Left + (0.148829431438127f * bounds.Width), bounds.Top + (0.400943396226415f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.249163879598662f * bounds.Width), bounds.Top + (0.379716981132075f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.359531772575251f * bounds.Width), bounds.Top + (0.318396226415094f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.387959866220736f * bounds.Width), bounds.Top + (0.308962264150943f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.389632107023411f * bounds.Width), bounds.Top + (0.247641509433962f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.357859531772575f * bounds.Width), bounds.Top + (0.226415094339623f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.347826086956522f * bounds.Width), bounds.Top + (0.186320754716981f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.326086956521739f * bounds.Width), bounds.Top + (0.136792452830189f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.37123745819398f * bounds.Width), bounds.Top + (0.153301886792453f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.426421404682274f * bounds.Width), bounds.Top + (0.162735849056604f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.421404682274247f * bounds.Width), bounds.Top + (0.287735849056604f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.635451505016722f * bounds.Width), bounds.Top + (0.136792452830189f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.819397993311037f * bounds.Width), bounds.Top + (0.143867924528302f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.764214046822743f * bounds.Width), bounds.Top + (0.172169811320755f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.642140468227425f * bounds.Width), bounds.Top + (0.179245283018868f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.471571906354515f * bounds.Width), bounds.Top + (0.30188679245283f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.526755852842809f * bounds.Width), bounds.Top + (0.344339622641509f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.576923076923077f * bounds.Width), bounds.Top + (0.32311320754717f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.704013377926421f * bounds.Width), bounds.Top + (0.379716981132075f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.652173913043478f * bounds.Width), bounds.Top + (0.417452830188679f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.510033444816054f * bounds.Width), bounds.Top + (0.410377358490566f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.419732441471572f * bounds.Width), bounds.Top + (0.341981132075472f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.344481605351171f * bounds.Width), bounds.Top + (0.386792452830189f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.259197324414716f * bounds.Width), bounds.Top + (0.415094339622642f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.207357859531773f * bounds.Width), bounds.Top + (0.44811320754717f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }

                case 8:
                    {
                        pts = new SKPoint[11];
                        pts[0] = new SKPoint(bounds.Left + (0.0836120401337793f * bounds.Width), bounds.Top + (0.374407582938389f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.247491638795987f * bounds.Width), bounds.Top + (0.329383886255924f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.319397993311037f * bounds.Width), bounds.Top + (0.253554502369668f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.292642140468227f * bounds.Width), bounds.Top + (0.213270142180095f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.334448160535117f * bounds.Width), bounds.Top + (0.232227488151659f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.384615384615385f * bounds.Width), bounds.Top + (0.232227488151659f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.426421404682274f * bounds.Width), bounds.Top + (0.265402843601896f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.411371237458194f * bounds.Width), bounds.Top + (0.317535545023697f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.377926421404682f * bounds.Width), bounds.Top + (0.293838862559242f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.297658862876254f * bounds.Width), bounds.Top + (0.348341232227488f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.180602006688963f * bounds.Width), bounds.Top + (0.386255924170616f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        pts = new SKPoint[12];
                        pts[0] = new SKPoint(bounds.Left + (0.401006711409396f * bounds.Width), bounds.Top + (0.133016627078385f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.458053691275168f * bounds.Width), bounds.Top + (0.128266033254157f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.48489932885906f * bounds.Width), bounds.Top + (0.104513064133017f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.531879194630873f * bounds.Width), bounds.Top + (0.128266033254157f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.493288590604027f * bounds.Width), bounds.Top + (0.156769596199525f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.523489932885906f * bounds.Width), bounds.Top + (0.194774346793349f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.771812080536913f * bounds.Width), bounds.Top + (0.30166270783848f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.895973154362416f * bounds.Width), bounds.Top + (0.315914489311164f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.808724832214765f * bounds.Width), bounds.Top + (0.363420427553444f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.681208053691275f * bounds.Width), bounds.Top + (0.356294536817102f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.486577181208054f * bounds.Width), bounds.Top + (0.180522565320665f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.424496644295302f * bounds.Width), bounds.Top + (0.156769596199525f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }

                case 9:
                    {
                        pts = new SKPoint[32];
                        pts[0] = new SKPoint(bounds.Left + (0.154103852596315f * bounds.Width), bounds.Top + (0.4f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.301507537688442f * bounds.Width), bounds.Top + (0.30952380952381f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.355108877721943f * bounds.Width), bounds.Top + (0.235714285714286f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.234505862646566f * bounds.Width), bounds.Top + (0.228571428571429f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.23785594639866f * bounds.Width), bounds.Top + (0.211904761904762f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.355108877721943f * bounds.Width), bounds.Top + (0.2f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.355108877721943f * bounds.Width), bounds.Top + (0.15f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.294807370184255f * bounds.Width), bounds.Top + (0.10952380952381f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.428810720268007f * bounds.Width), bounds.Top + (0.10952380952381f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.47571189279732f * bounds.Width), bounds.Top + (0.145238095238095f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.422110552763819f * bounds.Width), bounds.Top + (0.176190476190476f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.522613065326633f * bounds.Width), bounds.Top + (0.188095238095238f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.5678391959799f * bounds.Width), bounds.Top + (0.147619047619048f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.536013400335008f * bounds.Width), bounds.Top + (0.116666666666667f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.597989949748744f * bounds.Width), bounds.Top + (0.10952380952381f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.700167504187605f * bounds.Width), bounds.Top + (0.15f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.688442211055276f * bounds.Width), bounds.Top + (0.216666666666667f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.636515912897823f * bounds.Width), bounds.Top + (0.185714285714286f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.604690117252931f * bounds.Width), bounds.Top + (0.195238095238095f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.537688442211055f * bounds.Width), bounds.Top + (0.24047619047619f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.544388609715243f * bounds.Width), bounds.Top + (0.314285714285714f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.6214405360134f * bounds.Width), bounds.Top + (0.347619047619048f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.710217755443886f * bounds.Width), bounds.Top + (0.30952380952381f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.721943048576214f * bounds.Width), bounds.Top + (0.247619047619048f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.775544388609715f * bounds.Width), bounds.Top + (0.295238095238095f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.770519262981575f * bounds.Width), bounds.Top + (0.383333333333333f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.582914572864322f * bounds.Width), bounds.Top + (0.385714285714286f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.504187604690117f * bounds.Width), bounds.Top + (0.326190476190476f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.50251256281407f * bounds.Width), bounds.Top + (0.25f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.430485762144054f * bounds.Width), bounds.Top + (0.233333333333333f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.425460636515913f * bounds.Width), bounds.Top + (0.276190476190476f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.33500837520938f * bounds.Width), bounds.Top + (0.347619047619048f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts);
                        break;
                    }
            }
        }
        public void DrawSmallCircle(SKCanvas thisCanvas, SKRect bounds, SKPaint penPaint, SKPaint henPaint) // you have to send the paints.
        {
            thisCanvas.DrawOval(bounds, penPaint);
            SKRect rect;
            rect = SKRect.Create(bounds.Left + 2, bounds.Top + 2, bounds.Width - 4, bounds.Height - 4);
            thisCanvas.DrawOval(rect, henPaint);
            thisCanvas.DrawOval(rect, penPaint);
        }
        public void DrawBamboo(SKCanvas thisCanvas, SKRect bounds, SKPaint fillBrush, SKPaint penBrush)
        {
            SKPath gp;
            SKPath gp2;
            SKPoint[] pts;
            SKMatrix tmp_Matrix;
            var rect = SKRect.Create(bounds.Left + (bounds.Width * 0.3f), bounds.Top, bounds.Width * 0.4f, bounds.Height);
            pts = new SKPoint[6];
            gp = new SKPath();
            gp2 = new SKPath();
            pts[0] = new SKPoint(bounds.Left, bounds.Top + (bounds.Height * 0.15f));
            pts[1] = new SKPoint(rect.Left, bounds.Top);
            pts[2] = new SKPoint(rect.Left + rect.Width, bounds.Top);
            pts[3] = new SKPoint(bounds.Left + (bounds.Width), bounds.Top + (bounds.Height * 0.15f));
            pts[4] = new SKPoint(rect.Left + rect.Width, bounds.Top + (bounds.Height * 0.3f));
            pts[5] = new SKPoint(rect.Left, bounds.Top + (bounds.Height * 0.3f));
            gp.AddPoly(pts);
            gp2.AddPoly(pts);
            thisCanvas.DrawPath(gp, fillBrush);
            tmp_Matrix = SKMatrix.MakeTranslation(0, (bounds.Height / 3) + (bounds.Height * 0.1f));
            gp.Transform(tmp_Matrix);
            gp2.Transform(tmp_Matrix);
            thisCanvas.DrawPath(gp, fillBrush);
            gp.Transform(tmp_Matrix);
            thisCanvas.DrawPath(gp, fillBrush);
            thisCanvas.DrawRect(rect, _fillWhite);
            thisCanvas.DrawRect(rect, penBrush);
            thisCanvas.DrawPath(gp2, penBrush);
        }
        private void DrawOwl(SKCanvas thisCanvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[22];
            pts[0] = new SKPoint(bounds.Left + (0.631016042780749f * bounds.Width), bounds.Top + (0.1f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.545454545454545f * bounds.Width), bounds.Top + (0.174193548387097f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.401069518716578f * bounds.Width), bounds.Top + (0.219354838709677f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.521390374331551f * bounds.Width), bounds.Top + (0.229032258064516f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.580213903743316f * bounds.Width), bounds.Top + (0.27741935483871f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.526737967914439f * bounds.Width), bounds.Top + (0.358064516129032f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.344919786096257f * bounds.Width), bounds.Top + (0.319354838709677f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.299465240641711f * bounds.Width), bounds.Top + (0.37741935483871f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.363636363636364f * bounds.Width), bounds.Top + (0.519354838709677f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.497326203208556f * bounds.Width), bounds.Top + (0.535483870967742f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.636363636363636f * bounds.Width), bounds.Top + (0.464516129032258f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.6524064171123f * bounds.Width), bounds.Top + (0.364516129032258f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.593582887700535f * bounds.Width), bounds.Top + (0.319354838709677f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.580213903743316f * bounds.Width), bounds.Top + (0.438709677419355f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.574866310160428f * bounds.Width), bounds.Top + (0.354838709677419f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.516042780748663f * bounds.Width), bounds.Top + (0.370967741935484f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.510695187165775f * bounds.Width), bounds.Top + (0.467741935483871f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.494652406417112f * bounds.Width), bounds.Top + (0.37741935483871f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.457219251336898f * bounds.Width), bounds.Top + (0.37741935483871f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.459893048128342f * bounds.Width), bounds.Top + (0.448387096774194f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.438502673796791f * bounds.Width), bounds.Top + (0.370967741935484f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.36096256684492f * bounds.Width), bounds.Top + (0.345161290322581f * bounds.Height));
            DrawSKPenCurves(thisCanvas, pts, _smallRedPen!);
            pts = new SKPoint[7];
            pts[0] = new SKPoint(bounds.Left + (0.414438502673797f * bounds.Width), bounds.Top + (0.364516129032258f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.377005347593583f * bounds.Width), bounds.Top + (0.403225806451613f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.379679144385027f * bounds.Width), bounds.Top + (0.503225806451613f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.216577540106952f * bounds.Width), bounds.Top + (0.590322580645161f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.296791443850267f * bounds.Width), bounds.Top + (0.525806451612903f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.262032085561497f * bounds.Width), bounds.Top + (0.409677419354839f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.32620320855615f * bounds.Width), bounds.Top + (0.309677419354839f * bounds.Height));
            DrawSkCurves(thisCanvas, pts, false, _fillRed);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.171122994652406f * bounds.Width), bounds.Top + (0.196774193548387f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.270053475935829f * bounds.Width), bounds.Top + (0.212903225806452f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.350267379679144f * bounds.Width), bounds.Top + (0.261290322580645f * bounds.Height));
            DrawSKPenCurves(thisCanvas, pts, _smallRedPen!);
            thisCanvas.DrawOval(SKRect.Create(bounds.Left + (0.417112299465241f * bounds.Width), bounds.Top + (0.264516129032258f * bounds.Height), bounds.Width * 0.1f, bounds.Width * 0.1f), _fillRed);
            pts = new SKPoint[41];
            pts[0] = new SKPoint(bounds.Left + (0.858288770053476f * bounds.Width), bounds.Top + (0.429032258064516f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.938502673796791f * bounds.Width), bounds.Top + (0.425806451612903f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.903743315508021f * bounds.Width), bounds.Top + (0.361290322580645f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.807486631016043f * bounds.Width), bounds.Top + (0.403225806451613f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.684491978609626f * bounds.Width), bounds.Top + (0.519354838709677f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.655080213903743f * bounds.Width), bounds.Top + (0.554838709677419f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.655080213903743f * bounds.Width), bounds.Top + (0.632258064516129f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.633689839572192f * bounds.Width), bounds.Top + (0.680645161290323f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.644385026737968f * bounds.Width), bounds.Top + (0.635483870967742f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.641711229946524f * bounds.Width), bounds.Top + (0.551612903225806f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.593582887700535f * bounds.Width), bounds.Top + (0.574193548387097f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.601604278074866f * bounds.Width), bounds.Top + (0.629032258064516f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.56951871657754f * bounds.Width), bounds.Top + (0.57741935483871f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.526737967914439f * bounds.Width), bounds.Top + (0.593548387096774f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.540106951871658f * bounds.Width), bounds.Top + (0.619354838709677f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.572192513368984f * bounds.Width), bounds.Top + (0.670967741935484f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.540106951871658f * bounds.Width), bounds.Top + (0.641935483870968f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.510695187165775f * bounds.Width), bounds.Top + (0.593548387096774f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.46524064171123f * bounds.Width), bounds.Top + (0.609677419354839f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.486631016042781f * bounds.Width), bounds.Top + (0.651612903225806f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.558823529411765f * bounds.Width), bounds.Top + (0.725806451612903f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.491978609625668f * bounds.Width), bounds.Top + (0.670967741935484f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.446524064171123f * bounds.Width), bounds.Top + (0.603225806451613f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.398395721925134f * bounds.Width), bounds.Top + (0.616129032258064f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.459893048128342f * bounds.Width), bounds.Top + (0.745161290322581f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.582887700534759f * bounds.Width), bounds.Top + (0.832258064516129f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.695187165775401f * bounds.Width), bounds.Top + (0.890322580645161f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.577540106951872f * bounds.Width), bounds.Top + (0.835483870967742f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.454545454545455f * bounds.Width), bounds.Top + (0.738709677419355f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.39572192513369f * bounds.Width), bounds.Top + (0.632258064516129f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.323529411764706f * bounds.Width), bounds.Top + (0.638709677419355f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.347593582887701f * bounds.Width), bounds.Top + (0.7f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.294117647058824f * bounds.Width), bounds.Top + (0.641935483870968f * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.262032085561497f * bounds.Width), bounds.Top + (0.651612903225806f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.152406417112299f * bounds.Width), bounds.Top + (0.658064516129032f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0.053475935828877f * bounds.Width), bounds.Top + (0.72258064516129f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.133689839572193f * bounds.Width), bounds.Top + (0.687096774193548f * bounds.Height));
            pts[37] = new SKPoint(bounds.Left + (0.120320855614973f * bounds.Width), bounds.Top + (0.758064516129032f * bounds.Height));
            pts[38] = new SKPoint(bounds.Left + (0.17379679144385f * bounds.Width), bounds.Top + (0.703225806451613f * bounds.Height));
            pts[39] = new SKPoint(bounds.Left + (0.179144385026738f * bounds.Width), bounds.Top + (0.751612903225806f * bounds.Height));
            pts[40] = new SKPoint(bounds.Left + (0.24331550802139f * bounds.Width), bounds.Top + (0.674193548387097f * bounds.Height));
            DrawSKPenCurves(thisCanvas, pts, _smallDarkGreenPen!);
        }
        public void Draw17Bamboo(SKCanvas thisCanvas, SKRect bounds)
        {
            SKBitmap bmp_Temp = new SKBitmap((int)bounds.Width, (int)bounds.Height);
            var tempCanvas1 = new SKCanvas(bmp_Temp);
            SKBitmap bmp_Temp2 = new SKBitmap((int)bounds.Width, (int)bounds.Height);
            var tempCanvas2 = new SKCanvas(bmp_Temp2);
            tempCanvas1.Clear(SKColors.Transparent);
            tempCanvas2.Clear(SKColors.Transparent);
            SKMatrix tmp_Matrix = default;
            SKRect rect;
            rect = SKRect.Create((bounds.Width / 2) - (bounds.Width * 0.4f), (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Height * 0.3f);
            DrawBamboo(tempCanvas1, rect, _fillDarkBlue!, _smallDarkBluePen!);
            SKMatrix.RotateDegrees(ref tmp_Matrix, 45, rect.Left + (rect.Width / 2), rect.Top + (rect.Height));
            tempCanvas1.SetMatrix(tmp_Matrix);
            DrawBamboo(tempCanvas1, rect, _fillDarkBlue!, _smallDarkBluePen!);
            tempCanvas2.DrawBitmap(bmp_Temp, 0, 0); // i think
            tempCanvas1.ResetMatrix();
            tmp_Matrix = SKMatrix.MakeTranslation(bounds.Width, 0);
            tmp_Matrix.ScaleX = -1;
            tmp_Matrix.ScaleY = 1;
            tempCanvas2.SetMatrix(tmp_Matrix);
            tempCanvas2.DrawBitmap(bmp_Temp, 0, 0); // i think
            tempCanvas2.ResetMatrix();
            tmp_Matrix = SKMatrix.MakeTranslation(0, bounds.Height);
            tmp_Matrix.ScaleX = 1;
            tmp_Matrix.ScaleY = -1;
            tempCanvas2.SetMatrix(tmp_Matrix);
            tempCanvas2.DrawBitmap(bmp_Temp, 0, 0); // i think
            tempCanvas2.ResetMatrix();
            tmp_Matrix = SKMatrix.MakeTranslation(bounds.Width, bounds.Height);
            tmp_Matrix.ScaleX = -1;
            tmp_Matrix.ScaleY = -1;
            tempCanvas2.SetMatrix(tmp_Matrix);
            tempCanvas2.DrawBitmap(bmp_Temp, 0, 0);
            tempCanvas2.ResetMatrix();
            thisCanvas.DrawBitmap(bmp_Temp2, 0, 0);
            bmp_Temp.Dispose();
            bmp_Temp2.Dispose();
            tempCanvas1.Dispose();
            tempCanvas2.Dispose();
        }
        public void DrawTile34(SKCanvas thisCanvas, SKRect bounds)
        {
            SKPoint[] pts;
            pts = new SKPoint[54];
            pts[0] = new SKPoint(bounds.Left + (0.260303687635575f * bounds.Width), bounds.Top + (0.706214689265537f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.206073752711497f * bounds.Width), bounds.Top + (0.748587570621469f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.238611713665944f * bounds.Width), bounds.Top + (0.836158192090395f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.501084598698482f * bounds.Width), bounds.Top + (0.909604519774011f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.79175704989154f * bounds.Width), bounds.Top + (0.765536723163842f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.668112798264642f * bounds.Width), bounds.Top + (0.449152542372881f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.806941431670282f * bounds.Width), bounds.Top + (0.38135593220339f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.859002169197397f * bounds.Width), bounds.Top + (0.180790960451977f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.702819956616052f * bounds.Width), bounds.Top + (0.104519774011299f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.544468546637744f * bounds.Width), bounds.Top + (0.104519774011299f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.542299349240781f * bounds.Width), bounds.Top + (0.129943502824859f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.481561822125813f * bounds.Width), bounds.Top + (0.129943502824859f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.475054229934924f * bounds.Width), bounds.Top + (0.107344632768362f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.527114967462039f * bounds.Width), bounds.Top + (0.107344632768362f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.364425162689805f * bounds.Width), bounds.Top + (0.0819209039548023f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.314533622559653f * bounds.Width), bounds.Top + (0.0677966101694915f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.314533622559653f * bounds.Width), bounds.Top + (0.129943502824859f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.38177874186551f * bounds.Width), bounds.Top + (0.141242937853107f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.351409978308026f * bounds.Width), bounds.Top + (0.175141242937853f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.221258134490239f * bounds.Width), bounds.Top + (0.166666666666667f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.171366594360087f * bounds.Width), bounds.Top + (0.115819209039548f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.199566160520607f * bounds.Width), bounds.Top + (0.169491525423729f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.355748373101952f * bounds.Width), bounds.Top + (0.180790960451977f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.3058568329718f * bounds.Width), bounds.Top + (0.203389830508475f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.433839479392625f * bounds.Width), bounds.Top + (0.18361581920904f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.511930585683297f * bounds.Width), bounds.Top + (0.200564971751412f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.670281995661605f * bounds.Width), bounds.Top + (0.18361581920904f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.741865509761388f * bounds.Width), bounds.Top + (0.23728813559322f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.668112798264642f * bounds.Width), bounds.Top + (0.31638418079096f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.557483731019523f * bounds.Width), bounds.Top + (0.34180790960452f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.522776572668113f * bounds.Width), bounds.Top + (0.310734463276836f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.490238611713666f * bounds.Width), bounds.Top + (0.310734463276836f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.40997830802603f * bounds.Width), bounds.Top + (0.333333333333333f * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.507592190889371f * bounds.Width), bounds.Top + (0.327683615819209f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.516268980477223f * bounds.Width), bounds.Top + (0.344632768361582f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0.390455531453362f * bounds.Width), bounds.Top + (0.353107344632768f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.314533622559653f * bounds.Width), bounds.Top + (0.406779661016949f * bounds.Height));
            pts[37] = new SKPoint(bounds.Left + (0.34056399132321f * bounds.Width), bounds.Top + (0.483050847457627f * bounds.Height));
            pts[38] = new SKPoint(bounds.Left + (0.529284164859002f * bounds.Width), bounds.Top + (0.615819209039548f * bounds.Height));
            pts[39] = new SKPoint(bounds.Left + (0.631236442516269f * bounds.Width), bounds.Top + (0.672316384180791f * bounds.Height));
            pts[40] = new SKPoint(bounds.Left + (0.626898047722343f * bounds.Width), bounds.Top + (0.711864406779661f * bounds.Height));
            pts[41] = new SKPoint(bounds.Left + (0.68763557483731f * bounds.Width), bounds.Top + (0.765536723163842f * bounds.Height));
            pts[42] = new SKPoint(bounds.Left + (0.607375271149675f * bounds.Width), bounds.Top + (0.700564971751412f * bounds.Height));
            pts[43] = new SKPoint(bounds.Left + (0.492407809110629f * bounds.Width), bounds.Top + (0.711864406779661f * bounds.Height));
            pts[44] = new SKPoint(bounds.Left + (0.440347071583514f * bounds.Width), bounds.Top + (0.711864406779661f * bounds.Height));
            pts[45] = new SKPoint(bounds.Left + (0.422993492407809f * bounds.Width), bounds.Top + (0.686440677966102f * bounds.Height));
            pts[46] = new SKPoint(bounds.Left + (0.344902386117137f * bounds.Width), bounds.Top + (0.714689265536723f * bounds.Height));
            pts[47] = new SKPoint(bounds.Left + (0.383947939262473f * bounds.Width), bounds.Top + (0.706214689265537f * bounds.Height));
            pts[48] = new SKPoint(bounds.Left + (0.403470715835141f * bounds.Width), bounds.Top + (0.782485875706215f * bounds.Height));
            pts[49] = new SKPoint(bounds.Left + (0.509761388286334f * bounds.Width), bounds.Top + (0.745762711864407f * bounds.Height));
            pts[50] = new SKPoint(bounds.Left + (0.568329718004338f * bounds.Width), bounds.Top + (0.759887005649718f * bounds.Height));
            pts[51] = new SKPoint(bounds.Left + (0.566160520607375f * bounds.Width), bounds.Top + (0.807909604519774f * bounds.Height));
            pts[52] = new SKPoint(bounds.Left + (0.427331887201735f * bounds.Width), bounds.Top + (0.838983050847458f * bounds.Height));
            pts[53] = new SKPoint(bounds.Left + (0.273318872017354f * bounds.Width), bounds.Top + (0.76271186440678f * bounds.Height));
            DrawSKPenCurves(thisCanvas, pts, _smallBlackPen!);
            pts = new SKPoint[16];
            pts[0] = new SKPoint(bounds.Left + (0.662921348314607f * bounds.Width), bounds.Top + (0.461318051575931f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.498876404494382f * bounds.Width), bounds.Top + (0.429799426934097f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.350561797752809f * bounds.Width), bounds.Top + (0.458452722063037f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.328089887640449f * bounds.Width), bounds.Top + (0.558739255014327f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.235955056179775f * bounds.Width), bounds.Top + (0.492836676217765f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.244943820224719f * bounds.Width), bounds.Top + (0.461318051575931f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.193258426966292f * bounds.Width), bounds.Top + (0.446991404011461f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.112359550561798f * bounds.Width), bounds.Top + (0.449856733524355f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.166292134831461f * bounds.Width), bounds.Top + (0.469914040114613f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.0898876404494382f * bounds.Width), bounds.Top + (0.498567335243553f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.175280898876404f * bounds.Width), bounds.Top + (0.532951289398281f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.278651685393258f * bounds.Width), bounds.Top + (0.613180515759312f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.357303370786517f * bounds.Width), bounds.Top + (0.627507163323782f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.391011235955056f * bounds.Width), bounds.Top + (0.64756446991404f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.402247191011236f * bounds.Width), bounds.Top + (0.527220630372493f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.519101123595506f * bounds.Width), bounds.Top + (0.521489971346705f * bounds.Height));
            DrawSkCurves(thisCanvas, pts, true, _fillWhite); // well see how this works.
        }
        public void DrawTile33(SKCanvas thisCanvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[76];
            pts[0] = new SKPoint(bounds.Left + (0.302170283806344f * bounds.Width), bounds.Top + (0.287410926365796f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.387312186978297f * bounds.Width), bounds.Top + (0.216152019002375f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.440734557595993f * bounds.Width), bounds.Top + (0.280285035629454f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.298831385642738f * bounds.Width), bounds.Top + (0.432304038004751f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.425709515859766f * bounds.Width), bounds.Top + (0.434679334916865f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.454090150250417f * bounds.Width), bounds.Top + (0.39667458432304f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.490818030050083f * bounds.Width), bounds.Top + (0.427553444180523f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.495826377295493f * bounds.Width), bounds.Top + (0.551068883610451f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.335559265442404f * bounds.Width), bounds.Top + (0.551068883610451f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.307178631051753f * bounds.Width), bounds.Top + (0.589073634204276f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.515859766277129f * bounds.Width), bounds.Top + (0.59144893111639f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.530884808013356f * bounds.Width), bounds.Top + (0.532066508313539f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.569282136894825f * bounds.Width), bounds.Top + (0.508313539192399f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.562604340567613f * bounds.Width), bounds.Top + (0.460807600950119f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.534223706176962f * bounds.Width), bounds.Top + (0.446555819477435f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.529215358931553f * bounds.Width), bounds.Top + (0.399049881235154f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.559265442404007f * bounds.Width), bounds.Top + (0.380047505938242f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.492487479131886f * bounds.Width), bounds.Top + (0.36104513064133f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.492487479131886f * bounds.Width), bounds.Top + (0.318289786223278f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.449081803005008f * bounds.Width), bounds.Top + (0.318289786223278f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.457429048414023f * bounds.Width), bounds.Top + (0.263657957244656f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.519198664440735f * bounds.Width), bounds.Top + (0.247030878859857f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.56093489148581f * bounds.Width), bounds.Top + (0.232779097387173f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.585976627712855f * bounds.Width), bounds.Top + (0.180522565320665f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.639398998330551f * bounds.Width), bounds.Top + (0.230403800475059f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.741235392320534f * bounds.Width), bounds.Top + (0.275534441805226f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.602671118530885f * bounds.Width), bounds.Top + (0.351543942992874f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.669449081803005f * bounds.Width), bounds.Top + (0.387173396674584f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.74457429048414f * bounds.Width), bounds.Top + (0.465558194774347f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.846410684474123f * bounds.Width), bounds.Top + (0.470308788598575f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.891485809682805f * bounds.Width), bounds.Top + (0.517814726840855f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.799666110183639f * bounds.Width), bounds.Top + (0.532066508313539f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.72787979966611f * bounds.Width), bounds.Top + (0.562945368171021f * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.696160267111853f * bounds.Width), bounds.Top + (0.501187648456057f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.624373956594324f * bounds.Width), bounds.Top + (0.425178147268409f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0.609348914858097f * bounds.Width), bounds.Top + (0.510688836104513f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.674457429048414f * bounds.Width), bounds.Top + (0.558194774346793f * bounds.Height));
            pts[37] = new SKPoint(bounds.Left + (0.587646076794658f * bounds.Width), bounds.Top + (0.66270783847981f * bounds.Height));
            pts[38] = new SKPoint(bounds.Left + (0.676126878130217f * bounds.Width), bounds.Top + (0.717339667458432f * bounds.Height));
            pts[39] = new SKPoint(bounds.Left + (0.74457429048414f * bounds.Width), bounds.Top + (0.731591448931116f * bounds.Height));
            pts[40] = new SKPoint(bounds.Left + (0.762938230383973f * bounds.Width), bounds.Top + (0.809976247030879f * bounds.Height));
            pts[41] = new SKPoint(bounds.Left + (0.671118530884808f * bounds.Width), bounds.Top + (0.812351543942993f * bounds.Height));
            pts[42] = new SKPoint(bounds.Left + (0.552587646076795f * bounds.Width), bounds.Top + (0.705463182897862f * bounds.Height));
            pts[43] = new SKPoint(bounds.Left + (0.509181969949916f * bounds.Width), bounds.Top + (0.724465558194774f * bounds.Height));
            pts[44] = new SKPoint(bounds.Left + (0.445742904841402f * bounds.Width), bounds.Top + (0.757719714964371f * bounds.Height));
            pts[45] = new SKPoint(bounds.Left + (0.40567612687813f * bounds.Width), bounds.Top + (0.750593824228028f * bounds.Height));
            pts[46] = new SKPoint(bounds.Left + (0.420701168614357f * bounds.Width), bounds.Top + (0.712589073634204f * bounds.Height));
            pts[47] = new SKPoint(bounds.Left + (0.504173622704508f * bounds.Width), bounds.Top + (0.657957244655582f * bounds.Height));
            pts[48] = new SKPoint(bounds.Left + (0.549248747913189f * bounds.Width), bounds.Top + (0.648456057007126f * bounds.Height));
            pts[49] = new SKPoint(bounds.Left + (0.547579298831386f * bounds.Width), bounds.Top + (0.57957244655582f * bounds.Height));
            pts[50] = new SKPoint(bounds.Left + (0.534223706176962f * bounds.Width), bounds.Top + (0.634204275534442f * bounds.Height));
            pts[51] = new SKPoint(bounds.Left + (0.460767946577629f * bounds.Width), bounds.Top + (0.63895486935867f * bounds.Height));
            pts[52] = new SKPoint(bounds.Left + (0.419031719532554f * bounds.Width), bounds.Top + (0.672209026128266f * bounds.Height));
            pts[53] = new SKPoint(bounds.Left + (0.33889816360601f * bounds.Width), bounds.Top + (0.752969121140142f * bounds.Height));
            pts[54] = new SKPoint(bounds.Left + (0.32220367278798f * bounds.Width), bounds.Top + (0.788598574821853f * bounds.Height));
            pts[55] = new SKPoint(bounds.Left + (0.193656093489149f * bounds.Width), bounds.Top + (0.783847980997625f * bounds.Height));
            pts[56] = new SKPoint(bounds.Left + (0.348914858096828f * bounds.Width), bounds.Top + (0.679334916864608f * bounds.Height));
            pts[57] = new SKPoint(bounds.Left + (0.358931552587646f * bounds.Width), bounds.Top + (0.6270783847981f * bounds.Height));
            pts[58] = new SKPoint(bounds.Left + (0.337228714524207f * bounds.Width), bounds.Top + (0.6270783847981f * bounds.Height));
            pts[59] = new SKPoint(bounds.Left + (0.275459098497496f * bounds.Width), bounds.Top + (0.684085510688836f * bounds.Height));
            pts[60] = new SKPoint(bounds.Left + (0.200333889816361f * bounds.Width), bounds.Top + (0.648456057007126f * bounds.Height));
            pts[61] = new SKPoint(bounds.Left + (0.250417362270451f * bounds.Width), bounds.Top + (0.581947743467934f * bounds.Height));
            pts[62] = new SKPoint(bounds.Left + (0.215358931552588f * bounds.Width), bounds.Top + (0.553444180522565f * bounds.Height));
            pts[63] = new SKPoint(bounds.Left + (0.295492487479132f * bounds.Width), bounds.Top + (0.546318289786223f * bounds.Height));
            pts[64] = new SKPoint(bounds.Left + (0.320534223706177f * bounds.Width), bounds.Top + (0.501187648456057f * bounds.Height));
            pts[65] = new SKPoint(bounds.Left + (0.442404006677796f * bounds.Width), bounds.Top + (0.496437054631829f * bounds.Height));
            pts[66] = new SKPoint(bounds.Left + (0.402337228714524f * bounds.Width), bounds.Top + (0.475059382422803f * bounds.Height));
            pts[67] = new SKPoint(bounds.Left + (0.267111853088481f * bounds.Width), bounds.Top + (0.482185273159145f * bounds.Height));
            pts[68] = new SKPoint(bounds.Left + (0.13355592654424f * bounds.Width), bounds.Top + (0.584323040380047f * bounds.Height));
            pts[69] = new SKPoint(bounds.Left + (0.143572621035058f * bounds.Width), bounds.Top + (0.501187648456057f * bounds.Height));
            pts[70] = new SKPoint(bounds.Left + (0.242070116861436f * bounds.Width), bounds.Top + (0.444180522565321f * bounds.Height));
            pts[71] = new SKPoint(bounds.Left + (0.245409015025042f * bounds.Width), bounds.Top + (0.375296912114014f * bounds.Height));
            pts[72] = new SKPoint(bounds.Left + (0.212020033388982f * bounds.Width), bounds.Top + (0.315914489311164f * bounds.Height));
            pts[73] = new SKPoint(bounds.Left + (0.262103505843072f * bounds.Width), bounds.Top + (0.306413301662708f * bounds.Height));
            pts[74] = new SKPoint(bounds.Left + (0.292153589315526f * bounds.Width), bounds.Top + (0.351543942992874f * bounds.Height));
            pts[75] = new SKPoint(bounds.Left + (0.368948247078464f * bounds.Width), bounds.Top + (0.306413301662708f * bounds.Height));
            DrawSkCurves(thisCanvas, pts, false, _fillGreen);
        }
        public void DrawTile32(SKCanvas thisCanvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[9];
            pts[0] = new SKPoint(bounds.Left + (0.465546218487395f * bounds.Width), bounds.Top + (0.290476190476191f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.46890756302521f * bounds.Width), bounds.Top + (0.861904761904762f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.534453781512605f * bounds.Width), bounds.Top + (0.80952380952381f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.539495798319328f * bounds.Width), bounds.Top + (0.252380952380952f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.584873949579832f * bounds.Width), bounds.Top + (0.204761904761905f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.494117647058824f * bounds.Width), bounds.Top + (0.119047619047619f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.458823529411765f * bounds.Width), bounds.Top + (0.152380952380952f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.391596638655462f * bounds.Width), bounds.Top + (0.164285714285714f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.460504201680672f * bounds.Width), bounds.Top + (0.219047619047619f * bounds.Height));
            DrawSkCurves(thisCanvas, pts, false, _fillRed);
            pts = new SKPoint[24];
            pts[0] = new SKPoint(bounds.Left + (0.116357504215852f * bounds.Width), bounds.Top + (0.333333333333333f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.225969645868465f * bounds.Width), bounds.Top + (0.478571428571429f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.322091062394604f * bounds.Width), bounds.Top + (0.55952380952381f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.360876897133221f * bounds.Width), bounds.Top + (0.483333333333333f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.323777403035413f * bounds.Width), bounds.Top + (0.435714285714286f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.327150084317032f * bounds.Width), bounds.Top + (0.404761904761905f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.45531197301855f * bounds.Width), bounds.Top + (0.378571428571429f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.699831365935919f * bounds.Width), bounds.Top + (0.342857142857143f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.711635750421585f * bounds.Width), bounds.Top + (0.411904761904762f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.639123102866779f * bounds.Width), bounds.Top + (0.473809523809524f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.372681281618887f * bounds.Width), bounds.Top + (0.478571428571429f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.36424957841484f * bounds.Width), bounds.Top + (0.492857142857143f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.598650927487352f * bounds.Width), bounds.Top + (0.507142857142857f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.662731871838111f * bounds.Width), bounds.Top + (0.554761904761905f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.763912310286678f * bounds.Width), bounds.Top + (0.457142857142857f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.807757166947723f * bounds.Width), bounds.Top + (0.478571428571429f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.860033726812816f * bounds.Width), bounds.Top + (0.44047619047619f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.854974704890388f * bounds.Width), bounds.Top + (0.352380952380952f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.723440134907251f * bounds.Width), bounds.Top + (0.280952380952381f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.671163575042159f * bounds.Width), bounds.Top + (0.314285714285714f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.445193929173693f * bounds.Width), bounds.Top + (0.326190476190476f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.340640809443508f * bounds.Width), bounds.Top + (0.366666666666667f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.27318718381113f * bounds.Width), bounds.Top + (0.397619047619048f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.172006745362563f * bounds.Width), bounds.Top + (0.316666666666667f * bounds.Height));
            DrawSkCurves(thisCanvas, pts, false, _fillRed);
        }
        public void DrawSeasonTile(SKCanvas thisCanvas, SKRect bounds, string season)
        {
            SKRect rect;
            SKMatrix tmp_Matrix = new SKMatrix();
            int int_Count;
            using SKPath gp = new SKPath();
            using SKPath gp2 = new SKPath();
            SKPoint[] pts = new SKPoint[3];
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.15f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.7f, bounds.Width * 0.7f);
            switch (season)
            {
                case "SPR":
                    {
                        gp.AddArc(SKRect.Create(rect.Left - rect.Width, rect.Top, rect.Width * 2, rect.Height), -18, 36);
                        gp.LineTo(new SKPoint(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2)));
                        gp.Close();
                        SKMatrix.RotateDegrees(ref tmp_Matrix, 36, rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
                        for (int_Count = 0; int_Count <= 10; int_Count++)
                        {
                            gp.Transform(tmp_Matrix);
                            thisCanvas.DrawPath(gp, _fillGreen);
                        }
                        for (int_Count = 0; int_Count <= 10; int_Count++)
                        {
                            gp.Transform(tmp_Matrix);
                            thisCanvas.DrawPath(gp, _smallWhitePen);
                        }
                        rect = SKRect.Create(rect.Left + (rect.Width / 4), rect.Top + (rect.Height / 4), rect.Width / 2, rect.Height / 2);
                        thisCanvas.DrawOval(rect, _fillGreen);
                        thisCanvas.DrawOval(rect, _smallWhitePen);
                        break;
                    }

                case "SUM":
                    {
                        pts[0] = new SKPoint(rect.Left + (rect.Width / 2), rect.Top);
                        pts[1] = new SKPoint(rect.Left + (rect.Width * 0.7f), rect.Top + (rect.Height / 2));
                        pts[2] = new SKPoint(rect.Left + (rect.Width * 0.3f), rect.Top + (rect.Height / 2));
                        SKMatrix.RotateDegrees(ref tmp_Matrix, 40, rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
                        gp.AddPoly(pts, true);
                        for (int_Count = 0; int_Count <= 8; int_Count++)
                        {
                            gp.Transform(tmp_Matrix);
                            thisCanvas.DrawPath(gp, _fillDarkRed);
                        }
                        rect = SKRect.Create(rect.Left + (rect.Width / 4), rect.Top + (rect.Height / 4), rect.Width / 2, rect.Height / 2);
                        thisCanvas.DrawOval(rect, _fillWhite);
                        thisCanvas.DrawOval(rect, _smallDarkRedPen);
                        break;
                    }

                case "AUT":
                    {
                        gp.AddArc(SKRect.Create(rect.Left, rect.Top + (rect.Height / 4), rect.Width, rect.Height * 3 / 4), -30, 240);
                        gp.LineTo(new SKPoint(rect.Left + (rect.Width / 2), rect.Top));
                        gp.Close();
                        thisCanvas.DrawPath(gp, _hatchGreenTransparent75);
                        thisCanvas.DrawPath(gp, _hatchYellowTransparent40);
                        thisCanvas.DrawPath(gp, _hatchRedTransparrent20);
                        break;
                    }

                case "WIN":
                    {
                        pts[0] = new SKPoint(rect.Left + (rect.Width * 0.35f), rect.Top + (rect.Height * 0.1f));
                        pts[1] = new SKPoint(rect.Left + (rect.Width * 0.5f), rect.Top + (rect.Height * 0.25f));
                        pts[2] = new SKPoint(rect.Left + (rect.Width * 0.65f), rect.Top + (rect.Height * 0.1f));
                        gp2.MoveTo(pts[0]);
                        gp2.LineTo(pts[1]);
                        gp2.LineTo(pts[2]);
                        gp.MoveTo(new SKPoint(rect.Left + (rect.Width / 2), rect.Top));
                        gp.LineTo(new SKPoint(rect.Left + (rect.Width / 2), rect.Top + rect.Height));
                        SKMatrix.RotateDegrees(ref tmp_Matrix, 60, rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
                        for (int_Count = 1; int_Count <= 6; int_Count++)
                        {
                            gp.Transform(tmp_Matrix);
                            gp2.Transform(tmp_Matrix);
                            thisCanvas.DrawPath(gp, _smallRedPen);
                            thisCanvas.DrawPath(gp2, _smallRedPen);
                        }
                        break;
                    }
            }
            DrawSeasonText(thisCanvas, bounds, season);
        }
        public void DrawFlowerTile(SKCanvas thisCanvas, SKRect bounds, int number)
        {
            SKPoint[] pts;
            switch (number)
            {
                case 1:
                    {
                        pts = new SKPoint[61];
                        pts[0] = new SKPoint(bounds.Left + (0.106960950764007f * bounds.Width), bounds.Top + (0.471153846153846f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.225806451612903f * bounds.Width), bounds.Top + (0.451923076923077f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.276740237691002f * bounds.Width), bounds.Top + (0.471153846153846f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.269949066213922f * bounds.Width), bounds.Top + (0.598557692307692f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.387096774193548f * bounds.Width), bounds.Top + (0.514423076923077f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.385398981324278f * bounds.Width), bounds.Top + (0.329326923076923f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.33446519524618f * bounds.Width), bounds.Top + (0.314903846153846f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.33616298811545f * bounds.Width), bounds.Top + (0.252403846153846f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.375212224108659f * bounds.Width), bounds.Top + (0.216346153846154f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.404074702886248f * bounds.Width), bounds.Top + (0.262019230769231f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.412563667232598f * bounds.Width), bounds.Top + (0.288461538461538f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.431239388794567f * bounds.Width), bounds.Top + (0.254807692307692f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.453310696095076f * bounds.Width), bounds.Top + (0.189903846153846f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.387096774193548f * bounds.Width), bounds.Top + (0.185096153846154f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.390492359932088f * bounds.Width), bounds.Top + (0.15625f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.483870967741936f * bounds.Width), bounds.Top + (0.173076923076923f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.488964346349745f * bounds.Width), bounds.Top + (0.262019230769231f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.443123938879457f * bounds.Width), bounds.Top + (0.295673076923077f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.426146010186757f * bounds.Width), bounds.Top + (0.454326923076923f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.492359932088285f * bounds.Width), bounds.Top + (0.478365384615385f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.570458404074703f * bounds.Width), bounds.Top + (0.377403846153846f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.643463497453311f * bounds.Width), bounds.Top + (0.432692307692308f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.735144312393888f * bounds.Width), bounds.Top + (0.401442307692308f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.826825127334465f * bounds.Width), bounds.Top + (0.447115384615385f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.66553480475382f * bounds.Width), bounds.Top + (0.483173076923077f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.855687606112054f * bounds.Width), bounds.Top + (0.533653846153846f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.857385398981324f * bounds.Width), bounds.Top + (0.574519230769231f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.67911714770798f * bounds.Width), bounds.Top + (0.581730769230769f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.870967741935484f * bounds.Width), bounds.Top + (0.658653846153846f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.66723259762309f * bounds.Width), bounds.Top + (0.651442307692308f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.859083191850594f * bounds.Width), bounds.Top + (0.788461538461538f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.847198641765705f * bounds.Width), bounds.Top + (0.879807692307692f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.67911714770798f * bounds.Width), bounds.Top + (0.889423076923077f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.730050933786078f * bounds.Width), bounds.Top + (0.822115384615385f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.808149405772496f * bounds.Width), bounds.Top + (0.822115384615385f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.787775891341256f * bounds.Width), bounds.Top + (0.769230769230769f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.697792869269949f * bounds.Width), bounds.Top + (0.725961538461538f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.6553480475382f * bounds.Width), bounds.Top + (0.757211538461538f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.597623089983022f * bounds.Width), bounds.Top + (0.697115384615385f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.568760611205433f * bounds.Width), bounds.Top + (0.745192307692308f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.539898132427844f * bounds.Width), bounds.Top + (0.701923076923077f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.502546689303905f * bounds.Width), bounds.Top + (0.673076923076923f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.492359932088285f * bounds.Width), bounds.Top + (0.605769230769231f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.455008488964346f * bounds.Width), bounds.Top + (0.584134615384615f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.453310696095076f * bounds.Width), bounds.Top + (0.65625f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.511035653650255f * bounds.Width), bounds.Top + (0.766826923076923f * bounds.Height));
                        pts[46] = new SKPoint(bounds.Left + (0.582342954159593f * bounds.Width), bounds.Top + (0.788461538461538f * bounds.Height));
                        pts[47] = new SKPoint(bounds.Left + (0.641765704584041f * bounds.Width), bounds.Top + (0.786057692307692f * bounds.Height));
                        pts[48] = new SKPoint(bounds.Left + (0.611205432937182f * bounds.Width), bounds.Top + (0.831730769230769f * bounds.Height));
                        pts[49] = new SKPoint(bounds.Left + (0.606112054329372f * bounds.Width), bounds.Top + (0.887019230769231f * bounds.Height));
                        pts[50] = new SKPoint(bounds.Left + (0.444821731748727f * bounds.Width), bounds.Top + (0.882211538461538f * bounds.Height));
                        pts[51] = new SKPoint(bounds.Left + (0.509337860780985f * bounds.Width), bounds.Top + (0.853365384615385f * bounds.Height));
                        pts[52] = new SKPoint(bounds.Left + (0.483870967741936f * bounds.Width), bounds.Top + (0.805288461538462f * bounds.Height));
                        pts[53] = new SKPoint(bounds.Left + (0.453310696095076f * bounds.Width), bounds.Top + (0.805288461538462f * bounds.Height));
                        pts[54] = new SKPoint(bounds.Left + (0.426146010186757f * bounds.Width), bounds.Top + (0.891826923076923f * bounds.Height));
                        pts[55] = new SKPoint(bounds.Left + (0.32258064516129f * bounds.Width), bounds.Top + (0.879807692307692f * bounds.Height));
                        pts[56] = new SKPoint(bounds.Left + (0.3276740237691f * bounds.Width), bounds.Top + (0.819711538461538f * bounds.Height));
                        pts[57] = new SKPoint(bounds.Left + (0.378607809847199f * bounds.Width), bounds.Top + (0.834134615384615f * bounds.Height));
                        pts[58] = new SKPoint(bounds.Left + (0.400679117147708f * bounds.Width), bounds.Top + (0.730769230769231f * bounds.Height));
                        pts[59] = new SKPoint(bounds.Left + (0.307300509337861f * bounds.Width), bounds.Top + (0.783653846153846f * bounds.Height));
                        pts[60] = new SKPoint(bounds.Left + (0.129032258064516f * bounds.Width), bounds.Top + (0.783653846153846f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _hatchDarkGreenGreen30);
                        pts = new SKPoint[8];
                        pts[0] = new SKPoint(bounds.Left + (0.340640809443508f * bounds.Width), bounds.Top + (0.25952380952381f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.269814502529511f * bounds.Width), bounds.Top + (0.254761904761905f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.244519392917369f * bounds.Width), bounds.Top + (0.30952380952381f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.197301854974705f * bounds.Width), bounds.Top + (0.323809523809524f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.284991568296796f * bounds.Width), bounds.Top + (0.383333333333333f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.306913996627319f * bounds.Width), bounds.Top + (0.423809523809524f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.320404721753794f * bounds.Width), bounds.Top + (0.364285714285714f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.347386172006745f * bounds.Width), bounds.Top + (0.352380952380952f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillOrchid);
                        DrawSKPenCurves(thisCanvas, pts, _smallOrchidPen!);
                        pts = new SKPoint[13];
                        pts[0] = new SKPoint(bounds.Left + (0.591289782244556f * bounds.Width), bounds.Top + (0.478468899521531f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.529313232830821f * bounds.Width), bounds.Top + (0.385167464114833f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.494137353433836f * bounds.Width), bounds.Top + (0.447368421052632f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.571189279731993f * bounds.Width), bounds.Top + (0.519138755980861f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.530988274706868f * bounds.Width), bounds.Top + (0.552631578947368f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.554438860971524f * bounds.Width), bounds.Top + (0.578947368421053f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.596314907872697f * bounds.Width), bounds.Top + (0.519138755980861f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.613065326633166f * bounds.Width), bounds.Top + (0.566985645933014f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.710217755443886f * bounds.Width), bounds.Top + (0.507177033492823f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.695142378559464f * bounds.Width), bounds.Top + (0.461722488038278f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.641541038525963f * bounds.Width), bounds.Top + (0.490430622009569f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.700167504187605f * bounds.Width), bounds.Top + (0.41866028708134f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.659966499162479f * bounds.Width), bounds.Top + (0.315789473684211f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillOrchid);
                        DrawSKPenCurves(thisCanvas, pts, _smallOrchidPen!);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.631490787269682f * bounds.Width), bounds.Top + (0.471291866028708f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.57286432160804f * bounds.Width), bounds.Top + (0.471291866028708f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.574539363484087f * bounds.Width), bounds.Top + (0.523923444976077f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.604690117252931f * bounds.Width), bounds.Top + (0.5f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillYellow);
                        break;
                    }

                case 2:
                    {
                        pts = new SKPoint[22];
                        pts[0] = new SKPoint(bounds.Left + (0.235690235690236f * bounds.Width), bounds.Top + (0.470449172576832f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.131313131313131f * bounds.Width), bounds.Top + (0.364066193853428f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.225589225589226f * bounds.Width), bounds.Top + (0.390070921985816f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.242424242424242f * bounds.Width), bounds.Top + (0.319148936170213f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.297979797979798f * bounds.Width), bounds.Top + (0.326241134751773f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.36026936026936f * bounds.Width), bounds.Top + (0.42080378250591f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.563973063973064f * bounds.Width), bounds.Top + (0.288416075650118f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.651515151515151f * bounds.Width), bounds.Top + (0.328605200945626f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.641414141414141f * bounds.Width), bounds.Top + (0.390070921985816f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.664983164983165f * bounds.Width), bounds.Top + (0.505910165484634f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.786195286195286f * bounds.Width), bounds.Top + (0.557919621749409f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.666666666666667f * bounds.Width), bounds.Top + (0.591016548463357f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.643097643097643f * bounds.Width), bounds.Top + (0.728132387706856f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.838383838383838f * bounds.Width), bounds.Top + (0.650118203309693f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.836700336700337f * bounds.Width), bounds.Top + (0.855791962174941f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.218855218855219f * bounds.Width), bounds.Top + (0.846335697399527f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.286195286195286f * bounds.Width), bounds.Top + (0.806146572104019f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.141414141414141f * bounds.Width), bounds.Top + (0.773049645390071f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.348484848484849f * bounds.Width), bounds.Top + (0.742316784869976f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.136363636363636f * bounds.Width), bounds.Top + (0.654846335697399f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.271043771043771f * bounds.Width), bounds.Top + (0.567375886524823f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.12962962962963f * bounds.Width), bounds.Top + (0.49645390070922f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillLimeGreen);
                        pts = new SKPoint[33];
                        pts[0] = new SKPoint(bounds.Left + (0.365319865319865f * bounds.Width), bounds.Top + (0.469047619047619f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.276094276094276f * bounds.Width), bounds.Top + (0.469047619047619f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.271043771043771f * bounds.Width), bounds.Top + (0.423809523809524f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.203703703703704f * bounds.Width), bounds.Top + (0.430952380952381f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.186868686868687f * bounds.Width), bounds.Top + (0.485714285714286f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.222222222222222f * bounds.Width), bounds.Top + (0.521428571428571f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.195286195286195f * bounds.Width), bounds.Top + (0.59047619047619f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.237373737373737f * bounds.Width), bounds.Top + (0.597619047619048f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.291245791245791f * bounds.Width), bounds.Top + (0.545238095238095f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.378787878787879f * bounds.Width), bounds.Top + (0.547619047619048f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.31986531986532f * bounds.Width), bounds.Top + (0.588095238095238f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.294612794612795f * bounds.Width), bounds.Top + (0.626190476190476f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.335016835016835f * bounds.Width), bounds.Top + (0.7f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.619528619528619f * bounds.Width), bounds.Top + (0.695238095238095f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.734006734006734f * bounds.Width), bounds.Top + (0.60952380952381f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.654882154882155f * bounds.Width), bounds.Top + (0.54047619047619f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.521885521885522f * bounds.Width), bounds.Top + (0.526190476190476f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.602693602693603f * bounds.Width), bounds.Top + (0.488095238095238f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.693602693602694f * bounds.Width), bounds.Top + (0.516666666666667f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.739057239057239f * bounds.Width), bounds.Top + (0.542857142857143f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.732323232323232f * bounds.Width), bounds.Top + (0.471428571428571f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.698653198653199f * bounds.Width), bounds.Top + (0.452380952380952f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.718855218855219f * bounds.Width), bounds.Top + (0.392857142857143f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.612794612794613f * bounds.Width), bounds.Top + (0.392857142857143f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.607744107744108f * bounds.Width), bounds.Top + (0.435714285714286f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.543771043771044f * bounds.Width), bounds.Top + (0.466666666666667f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.523569023569024f * bounds.Width), bounds.Top + (0.445238095238095f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.552188552188552f * bounds.Width), bounds.Top + (0.311904761904762f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.511784511784512f * bounds.Width), bounds.Top + (0.278571428571429f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.47979797979798f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.336700336700337f * bounds.Width), bounds.Top + (0.292857142857143f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.356902356902357f * bounds.Width), bounds.Top + (0.357142857142857f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.314814814814815f * bounds.Width), bounds.Top + (0.361904761904762f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillYellow);
                        pts = new SKPoint[6];
                        pts[0] = new SKPoint(bounds.Left + (0.471380471380471f * bounds.Width), bounds.Top + (0.433333333333333f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.367003367003367f * bounds.Width), bounds.Top + (0.464285714285714f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.361952861952862f * bounds.Width), bounds.Top + (0.523809523809524f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.446127946127946f * bounds.Width), bounds.Top + (0.578571428571429f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.547138047138047f * bounds.Width), bounds.Top + (0.576190476190476f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.54040404040404f * bounds.Width), bounds.Top + (0.478571428571429f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillGoldenRod);
                        break;
                    }

                case 3:
                    {
                        pts = new SKPoint[25];
                        pts[0] = new SKPoint(bounds.Left + (0.208828522920204f * bounds.Width), bounds.Top + (0.854368932038835f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.607809847198642f * bounds.Width), bounds.Top + (0.866504854368932f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.752122241086587f * bounds.Width), bounds.Top + (0.781553398058252f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.570458404074703f * bounds.Width), bounds.Top + (0.79368932038835f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.536502546689304f * bounds.Width), bounds.Top + (0.402912621359223f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.66553480475382f * bounds.Width), bounds.Top + (0.402912621359223f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.831918505942275f * bounds.Width), bounds.Top + (0.509708737864078f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.852292020373514f * bounds.Width), bounds.Top + (0.434466019417476f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.791171477079796f * bounds.Width), bounds.Top + (0.364077669902913f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.617996604414261f * bounds.Width), bounds.Top + (0.347087378640777f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.713073005093379f * bounds.Width), bounds.Top + (0.264563106796117f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.617996604414261f * bounds.Width), bounds.Top + (0.20631067961165f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.585738539898132f * bounds.Width), bounds.Top + (0.228155339805825f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.529711375212224f * bounds.Width), bounds.Top + (0.182038834951456f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.492359932088285f * bounds.Width), bounds.Top + (0.245145631067961f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.500848896434635f * bounds.Width), bounds.Top + (0.303398058252427f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.242784380305603f * bounds.Width), bounds.Top + (0.247572815533981f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.298811544991511f * bounds.Width), bounds.Top + (0.349514563106796f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.398981324278438f * bounds.Width), bounds.Top + (0.480582524271845f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.222410865874363f * bounds.Width), bounds.Top + (0.468446601941748f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.171477079796265f * bounds.Width), bounds.Top + (0.502427184466019f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.164685908319185f * bounds.Width), bounds.Top + (0.611650485436893f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.33955857385399f * bounds.Width), bounds.Top + (0.655339805825243f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.302207130730051f * bounds.Width), bounds.Top + (0.730582524271845f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.205432937181664f * bounds.Width), bounds.Top + (0.740291262135922f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillGreen);
                        pts = new SKPoint[18];
                        pts[0] = new SKPoint(bounds.Left + (0.272727272727273f * bounds.Width), bounds.Top + (0.463414634146341f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.391080617495712f * bounds.Width), bounds.Top + (0.541463414634146f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.313893653516295f * bounds.Width), bounds.Top + (0.568292682926829f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.240137221269297f * bounds.Width), bounds.Top + (0.621951219512195f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.264150943396226f * bounds.Width), bounds.Top + (0.719512195121951f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.451114922813036f * bounds.Width), bounds.Top + (0.763414634146341f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.521440823327616f * bounds.Width), bounds.Top + (0.758536585365854f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.588336192109777f * bounds.Width), bounds.Top + (0.819512195121951f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.643224699828473f * bounds.Width), bounds.Top + (0.773170731707317f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.641509433962264f * bounds.Width), bounds.Top + (0.731707317073171f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.708404802744425f * bounds.Width), bounds.Top + (0.719512195121951f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.840480274442539f * bounds.Width), bounds.Top + (0.597560975609756f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.756432246998285f * bounds.Width), bounds.Top + (0.529268292682927f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.701543739279588f * bounds.Width), bounds.Top + (0.55609756097561f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.514579759862779f * bounds.Width), bounds.Top + (0.431707317073171f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.461406518010292f * bounds.Width), bounds.Top + (0.439024390243902f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.428816466552316f * bounds.Width), bounds.Top + (0.390243902439024f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.384219554030875f * bounds.Width), bounds.Top + (0.446341463414634f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillSalmon);
                        pts = new SKPoint[6];
                        pts[0] = new SKPoint(bounds.Left + (0.475128644939966f * bounds.Width), bounds.Top + (0.248780487804878f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.452830188679245f * bounds.Width), bounds.Top + (0.24390243902439f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.456260720411664f * bounds.Width), bounds.Top + (0.185365853658537f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.526586620926244f * bounds.Width), bounds.Top + (0.178048780487805f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.519725557461406f * bounds.Width), bounds.Top + (0.197560975609756f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.490566037735849f * bounds.Width), bounds.Top + (0.2f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillSalmon);
                        pts[0] = new SKPoint(bounds.Left + (0.632933104631218f * bounds.Width), bounds.Top + (0.182926829268293f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.632933104631218f * bounds.Width), bounds.Top + (0.219512195121951f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.662092624356775f * bounds.Width), bounds.Top + (0.231707317073171f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.680960548885077f * bounds.Width), bounds.Top + (0.214634146341463f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.679245283018868f * bounds.Width), bounds.Top + (0.190243902439024f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.660377358490566f * bounds.Width), bounds.Top + (0.180487804878049f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillSalmon);
                        pts = new SKPoint[21];
                        pts[0] = new SKPoint(bounds.Left + (0.512820512820513f * bounds.Width), bounds.Top + (0.536585365853659f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.526495726495727f * bounds.Width), bounds.Top + (0.582926829268293f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.446153846153846f * bounds.Width), bounds.Top + (0.597560975609756f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.504273504273504f * bounds.Width), bounds.Top + (0.636585365853659f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.541880341880342f * bounds.Width), bounds.Top + (0.690243902439024f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.44957264957265f * bounds.Width), bounds.Top + (0.714634146341463f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.454700854700855f * bounds.Width), bounds.Top + (0.753658536585366f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.528205128205128f * bounds.Width), bounds.Top + (0.758536585365854f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.608547008547008f * bounds.Width), bounds.Top + (0.663414634146342f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.558974358974359f * bounds.Width), bounds.Top + (0.631707317073171f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.622222222222222f * bounds.Width), bounds.Top + (0.55609756097561f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.697435897435897f * bounds.Width), bounds.Top + (0.570731707317073f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.67008547008547f * bounds.Width), bounds.Top + (0.665853658536585f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.731623931623932f * bounds.Width), bounds.Top + (0.690243902439024f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.811965811965812f * bounds.Width), bounds.Top + (0.592682926829268f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.760683760683761f * bounds.Width), bounds.Top + (0.529268292682927f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.647863247863248f * bounds.Width), bounds.Top + (0.507317073170732f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.676923076923077f * bounds.Width), bounds.Top + (0.482926829268293f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.67008547008547f * bounds.Width), bounds.Top + (0.431707317073171f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.618803418803419f * bounds.Width), bounds.Top + (0.414634146341463f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.623931623931624f * bounds.Width), bounds.Top + (0.473170731707317f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillRed);
                        pts = new SKPoint[4];
                        pts[0] = new SKPoint(bounds.Left + (0.48034188034188f * bounds.Width), bounds.Top + (0.531707317073171f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.447863247863248f * bounds.Width), bounds.Top + (0.55609756097561f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.482051282051282f * bounds.Width), bounds.Top + (0.570731707317073f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.495726495726496f * bounds.Width), bounds.Top + (0.553658536585366f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillGreen);
                        break;
                    }

                case 4:
                    {
                        pts = new SKPoint[47];
                        pts[0] = new SKPoint(bounds.Left + (0.214527027027027f * bounds.Width), bounds.Top + (0.834123222748815f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.297297297297297f * bounds.Width), bounds.Top + (0.815165876777251f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.329391891891892f * bounds.Width), bounds.Top + (0.791469194312796f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.285472972972973f * bounds.Width), bounds.Top + (0.781990521327014f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.282094594594595f * bounds.Width), bounds.Top + (0.682464454976303f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.380067567567568f * bounds.Width), bounds.Top + (0.744075829383886f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.388513513513513f * bounds.Width), bounds.Top + (0.639810426540284f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.432432432432432f * bounds.Width), bounds.Top + (0.644549763033175f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.435810810810811f * bounds.Width), bounds.Top + (0.682464454976303f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.491554054054054f * bounds.Width), bounds.Top + (0.687203791469194f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.501689189189189f * bounds.Width), bounds.Top + (0.725118483412322f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.462837837837838f * bounds.Width), bounds.Top + (0.748815165876777f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.503378378378378f * bounds.Width), bounds.Top + (0.758293838862559f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.506756756756757f * bounds.Width), bounds.Top + (0.786729857819905f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.452702702702703f * bounds.Width), bounds.Top + (0.791469194312796f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.456081081081081f * bounds.Width), bounds.Top + (0.819905213270142f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.506756756756757f * bounds.Width), bounds.Top + (0.815165876777251f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.516891891891892f * bounds.Width), bounds.Top + (0.838862559241706f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.606418918918919f * bounds.Width), bounds.Top + (0.765402843601896f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.564189189189189f * bounds.Width), bounds.Top + (0.748815165876777f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.581081081081081f * bounds.Width), bounds.Top + (0.682464454976303f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.677364864864865f * bounds.Width), bounds.Top + (0.677725118483412f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.680743243243243f * bounds.Width), bounds.Top + (0.781990521327014f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.719594594594595f * bounds.Width), bounds.Top + (0.781990521327014f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.712837837837838f * bounds.Width), bounds.Top + (0.73696682464455f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.809121621621622f * bounds.Width), bounds.Top + (0.672985781990521f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.85472972972973f * bounds.Width), bounds.Top + (0.701421800947867f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.795608108108108f * bounds.Width), bounds.Top + (0.765402843601896f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.866554054054054f * bounds.Width), bounds.Top + (0.829383886255924f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.814189189189189f * bounds.Width), bounds.Top + (0.872037914691943f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.731418918918919f * bounds.Width), bounds.Top + (0.808056872037915f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.709459459459459f * bounds.Width), bounds.Top + (0.819905213270142f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.716216216216216f * bounds.Width), bounds.Top + (0.862559241706161f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.673986486486487f * bounds.Width), bounds.Top + (0.862559241706161f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.66722972972973f * bounds.Width), bounds.Top + (0.779620853080569f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.559121621621622f * bounds.Width), bounds.Top + (0.864928909952607f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.427364864864865f * bounds.Width), bounds.Top + (0.869668246445498f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.425675675675676f * bounds.Width), bounds.Top + (0.815165876777251f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.445945945945946f * bounds.Width), bounds.Top + (0.812796208530806f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.4375f * bounds.Width), bounds.Top + (0.786729857819905f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.425675675675676f * bounds.Width), bounds.Top + (0.781990521327014f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.420608108108108f * bounds.Width), bounds.Top + (0.73696682464455f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.405405405405405f * bounds.Width), bounds.Top + (0.748815165876777f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.336148648648649f * bounds.Width), bounds.Top + (0.791469194312796f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.347972972972973f * bounds.Width), bounds.Top + (0.81042654028436f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.287162162162162f * bounds.Width), bounds.Top + (0.85781990521327f * bounds.Height));
                        pts[46] = new SKPoint(bounds.Left + (0.222972972972973f * bounds.Width), bounds.Top + (0.85781990521327f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillGreen);
                        pts = new SKPoint[77];
                        pts[0] = new SKPoint(bounds.Left + (0.309882747068677f * bounds.Width), bounds.Top + (0.664285714285714f * bounds.Height));
                        pts[1] = new SKPoint(bounds.Left + (0.350083752093802f * bounds.Width), bounds.Top + (0.642857142857143f * bounds.Height));
                        pts[2] = new SKPoint(bounds.Left + (0.340033500837521f * bounds.Width), bounds.Top + (0.59047619047619f * bounds.Height));
                        pts[3] = new SKPoint(bounds.Left + (0.294807370184255f * bounds.Width), bounds.Top + (0.616666666666667f * bounds.Height));
                        pts[4] = new SKPoint(bounds.Left + (0.268006700167504f * bounds.Width), bounds.Top + (0.652380952380952f * bounds.Height));
                        pts[5] = new SKPoint(bounds.Left + (0.236180904522613f * bounds.Width), bounds.Top + (0.626190476190476f * bounds.Height));
                        pts[6] = new SKPoint(bounds.Left + (0.281407035175879f * bounds.Width), bounds.Top + (0.592857142857143f * bounds.Height));
                        pts[7] = new SKPoint(bounds.Left + (0.249581239530988f * bounds.Width), bounds.Top + (0.573809523809524f * bounds.Height));
                        pts[8] = new SKPoint(bounds.Left + (0.249581239530988f * bounds.Width), bounds.Top + (0.530952380952381f * bounds.Height));
                        pts[9] = new SKPoint(bounds.Left + (0.288107202680067f * bounds.Width), bounds.Top + (0.511904761904762f * bounds.Height));
                        pts[10] = new SKPoint(bounds.Left + (0.247906197654941f * bounds.Width), bounds.Top + (0.492857142857143f * bounds.Height));
                        pts[11] = new SKPoint(bounds.Left + (0.26465661641541f * bounds.Width), bounds.Top + (0.473809523809524f * bounds.Height));
                        pts[12] = new SKPoint(bounds.Left + (0.306532663316583f * bounds.Width), bounds.Top + (0.464285714285714f * bounds.Height));
                        pts[13] = new SKPoint(bounds.Left + (0.313232830820771f * bounds.Width), bounds.Top + (0.428571428571429f * bounds.Height));
                        pts[14] = new SKPoint(bounds.Left + (0.291457286432161f * bounds.Width), bounds.Top + (0.383333333333333f * bounds.Height));
                        pts[15] = new SKPoint(bounds.Left + (0.244556113902848f * bounds.Width), bounds.Top + (0.352380952380952f * bounds.Height));
                        pts[16] = new SKPoint(bounds.Left + (0.254606365159129f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[17] = new SKPoint(bounds.Left + (0.328308207705193f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[18] = new SKPoint(bounds.Left + (0.32998324958124f * bounds.Width), bounds.Top + (0.342857142857143f * bounds.Height));
                        pts[19] = new SKPoint(bounds.Left + (0.353433835845896f * bounds.Width), bounds.Top + (0.30952380952381f * bounds.Height));
                        pts[20] = new SKPoint(bounds.Left + (0.321608040201005f * bounds.Width), bounds.Top + (0.25f * bounds.Height));
                        pts[21] = new SKPoint(bounds.Left + (0.284757118927973f * bounds.Width), bounds.Top + (0.183333333333333f * bounds.Height));
                        pts[22] = new SKPoint(bounds.Left + (0.333333333333333f * bounds.Width), bounds.Top + (0.188095238095238f * bounds.Height));
                        pts[23] = new SKPoint(bounds.Left + (0.333333333333333f * bounds.Width), bounds.Top + (0.223809523809524f * bounds.Height));
                        pts[24] = new SKPoint(bounds.Left + (0.351758793969849f * bounds.Width), bounds.Top + (0.221428571428571f * bounds.Height));
                        pts[25] = new SKPoint(bounds.Left + (0.358458961474037f * bounds.Width), bounds.Top + (0.166666666666667f * bounds.Height));
                        pts[26] = new SKPoint(bounds.Left + (0.40536013400335f * bounds.Width), bounds.Top + (0.178571428571429f * bounds.Height));
                        pts[27] = new SKPoint(bounds.Left + (0.403685092127303f * bounds.Width), bounds.Top + (0.223809523809524f * bounds.Height));
                        pts[28] = new SKPoint(bounds.Left + (0.423785594639866f * bounds.Width), bounds.Top + (0.238095238095238f * bounds.Height));
                        pts[29] = new SKPoint(bounds.Left + (0.428810720268007f * bounds.Width), bounds.Top + (0.283333333333333f * bounds.Height));
                        pts[30] = new SKPoint(bounds.Left + (0.400335008375209f * bounds.Width), bounds.Top + (0.316666666666667f * bounds.Height));
                        pts[31] = new SKPoint(bounds.Left + (0.395309882747069f * bounds.Width), bounds.Top + (0.380952380952381f * bounds.Height));
                        pts[32] = new SKPoint(bounds.Left + (0.380234505862647f * bounds.Width), bounds.Top + (0.411904761904762f * bounds.Height));
                        pts[33] = new SKPoint(bounds.Left + (0.323283082077052f * bounds.Width), bounds.Top + (0.426190476190476f * bounds.Height));
                        pts[34] = new SKPoint(bounds.Left + (0.324958123953099f * bounds.Width), bounds.Top + (0.435714285714286f * bounds.Height));
                        pts[35] = new SKPoint(bounds.Left + (0.417085427135678f * bounds.Width), bounds.Top + (0.438095238095238f * bounds.Height));
                        pts[36] = new SKPoint(bounds.Left + (0.420435510887772f * bounds.Width), bounds.Top + (0.464285714285714f * bounds.Height));
                        pts[37] = new SKPoint(bounds.Left + (0.333333333333333f * bounds.Width), bounds.Top + (0.464285714285714f * bounds.Height));
                        pts[38] = new SKPoint(bounds.Left + (0.313232830820771f * bounds.Width), bounds.Top + (0.495238095238095f * bounds.Height));
                        pts[39] = new SKPoint(bounds.Left + (0.328308207705193f * bounds.Width), bounds.Top + (0.528571428571429f * bounds.Height));
                        pts[40] = new SKPoint(bounds.Left + (0.370184254606365f * bounds.Width), bounds.Top + (0.507142857142857f * bounds.Height));
                        pts[41] = new SKPoint(bounds.Left + (0.417085427135678f * bounds.Width), bounds.Top + (0.488095238095238f * bounds.Height));
                        pts[42] = new SKPoint(bounds.Left + (0.460636515912898f * bounds.Width), bounds.Top + (0.488095238095238f * bounds.Height));
                        pts[43] = new SKPoint(bounds.Left + (0.495812395309883f * bounds.Width), bounds.Top + (0.457142857142857f * bounds.Height));
                        pts[44] = new SKPoint(bounds.Left + (0.485762144053601f * bounds.Width), bounds.Top + (0.364285714285714f * bounds.Height));
                        pts[45] = new SKPoint(bounds.Left + (0.428810720268007f * bounds.Width), bounds.Top + (0.354761904761905f * bounds.Height));
                        pts[46] = new SKPoint(bounds.Left + (0.428810720268007f * bounds.Width), bounds.Top + (0.319047619047619f * bounds.Height));
                        pts[47] = new SKPoint(bounds.Left + (0.477386934673367f * bounds.Width), bounds.Top + (0.323809523809524f * bounds.Height));
                        pts[48] = new SKPoint(bounds.Left + (0.494137353433836f * bounds.Width), bounds.Top + (0.330952380952381f * bounds.Height));
                        pts[49] = new SKPoint(bounds.Left + (0.470686767169179f * bounds.Width), bounds.Top + (0.292857142857143f * bounds.Height));
                        pts[50] = new SKPoint(bounds.Left + (0.507537688442211f * bounds.Width), bounds.Top + (0.276190476190476f * bounds.Height));
                        pts[51] = new SKPoint(bounds.Left + (0.509212730318258f * bounds.Width), bounds.Top + (0.245238095238095f * bounds.Height));
                        pts[52] = new SKPoint(bounds.Left + (0.530988274706868f * bounds.Width), bounds.Top + (0.273809523809524f * bounds.Height));
                        pts[53] = new SKPoint(bounds.Left + (0.571189279731993f * bounds.Width), bounds.Top + (0.288095238095238f * bounds.Height));
                        pts[54] = new SKPoint(bounds.Left + (0.530988274706868f * bounds.Width), bounds.Top + (0.316666666666667f * bounds.Height));
                        pts[55] = new SKPoint(bounds.Left + (0.532663316582915f * bounds.Width), bounds.Top + (0.345238095238095f * bounds.Height));
                        pts[56] = new SKPoint(bounds.Left + (0.609715242881072f * bounds.Width), bounds.Top + (0.369047619047619f * bounds.Height));
                        pts[57] = new SKPoint(bounds.Left + (0.597989949748744f * bounds.Width), bounds.Top + (0.404761904761905f * bounds.Height));
                        pts[58] = new SKPoint(bounds.Left + (0.557788944723618f * bounds.Width), bounds.Top + (0.392857142857143f * bounds.Height));
                        pts[59] = new SKPoint(bounds.Left + (0.525963149078727f * bounds.Width), bounds.Top + (0.392857142857143f * bounds.Height));
                        pts[60] = new SKPoint(bounds.Left + (0.532663316582915f * bounds.Width), bounds.Top + (0.416666666666667f * bounds.Height));
                        pts[61] = new SKPoint(bounds.Left + (0.566164154103853f * bounds.Width), bounds.Top + (0.435714285714286f * bounds.Height));
                        pts[62] = new SKPoint(bounds.Left + (0.561139028475712f * bounds.Width), bounds.Top + (0.457142857142857f * bounds.Height));
                        pts[63] = new SKPoint(bounds.Left + (0.536013400335008f * bounds.Width), bounds.Top + (0.466666666666667f * bounds.Height));
                        pts[64] = new SKPoint(bounds.Left + (0.577889447236181f * bounds.Width), bounds.Top + (0.466666666666667f * bounds.Height));
                        pts[65] = new SKPoint(bounds.Left + (0.577889447236181f * bounds.Width), bounds.Top + (0.504761904761905f * bounds.Height));
                        pts[66] = new SKPoint(bounds.Left + (0.530988274706868f * bounds.Width), bounds.Top + (0.523809523809524f * bounds.Height));
                        pts[67] = new SKPoint(bounds.Left + (0.529313232830821f * bounds.Width), bounds.Top + (0.552380952380952f * bounds.Height));
                        pts[68] = new SKPoint(bounds.Left + (0.484087102177554f * bounds.Width), bounds.Top + (0.526190476190476f * bounds.Height));
                        pts[69] = new SKPoint(bounds.Left + (0.395309882747069f * bounds.Width), bounds.Top + (0.519047619047619f * bounds.Height));
                        pts[70] = new SKPoint(bounds.Left + (0.351758793969849f * bounds.Width), bounds.Top + (0.545238095238095f * bounds.Height));
                        pts[71] = new SKPoint(bounds.Left + (0.380234505862647f * bounds.Width), bounds.Top + (0.569047619047619f * bounds.Height));
                        pts[72] = new SKPoint(bounds.Left + (0.442211055276382f * bounds.Width), bounds.Top + (0.554761904761905f * bounds.Height));
                        pts[73] = new SKPoint(bounds.Left + (0.509212730318258f * bounds.Width), bounds.Top + (0.576190476190476f * bounds.Height));
                        pts[74] = new SKPoint(bounds.Left + (0.536013400335008f * bounds.Width), bounds.Top + (0.6f * bounds.Height));
                        pts[75] = new SKPoint(bounds.Left + (0.477386934673367f * bounds.Width), bounds.Top + (0.611904761904762f * bounds.Height));
                        pts[76] = new SKPoint(bounds.Left + (0.442211055276382f * bounds.Width), bounds.Top + (0.661904761904762f * bounds.Height));
                        DrawSkCurves(thisCanvas, pts, false, _fillSteelBlue);
                        break;
                    }
            }
        }
        #endregion
    }
}