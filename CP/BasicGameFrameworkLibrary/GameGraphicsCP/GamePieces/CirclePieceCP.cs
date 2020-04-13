using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces
{
    public class CirclePieceCP<E> : BaseGraphicsCP, IEnumPiece<E> where E : Enum
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
        //because of setback, can't do autoimplement interfaces for quite a while since some mobile does not support .net standard 2.1.

        private bool _NeedsWhiteBorders;
        public bool NeedsWhiteBorders
        {
            get { return _NeedsWhiteBorders; }
            set
            {
                if (SetProperty(ref _NeedsWhiteBorders, value))
                {
                    PaintUI!.DoInvalidate();
                }
            }
        }
        private readonly SKPaint _borderPaint;
        private readonly SKPaint _whitePaint;
        private readonly SKPaint _thickBorder;
        public CirclePieceCP()
        {
            NeedsHighlighting = false;
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); // i think 3 is fine.
            _thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 5);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
        }
        public override void DrawImage(SKCanvas dc)
        {
            var thisRect = GetMainRect();
            if (NeedsWhiteBorders == true)
            {
                dc.DrawRect(thisRect, _whitePaint);
                dc.DrawRect(thisRect, _thickBorder);
                thisRect = SKRect.Create(thisRect.Location.X + 5, thisRect.Location.Y + 5, thisRect.Width - 10, thisRect.Width - 10);
            }
            else
            {
                thisRect = SKRect.Create(thisRect.Location.X + 2, thisRect.Location.Y + 2, thisRect.Width - 4, thisRect.Height - 4);
            }
            if (thisRect.Width != thisRect.Height)
                throw new BasicBlankException("Rethink because width and height is not the same so its not a true circle.  Values are " + thisRect.Width + ", " + thisRect.Height + ".  Actual Was " + ActualWidth + ", " + ActualHeight + ".  Needs White Borders Is " + NeedsWhiteBorders);
            if (MainColor.Equals(cs.Transparent) == true)
            {
                if (NeedsWhiteBorders == false)
                    throw new Exception("Will cause blank.  If we want that, rethink");
                return;
            }
            dc.DrawOval(thisRect, MainPaint);
            dc.DrawOval(thisRect, _borderPaint);
        }
    }
}