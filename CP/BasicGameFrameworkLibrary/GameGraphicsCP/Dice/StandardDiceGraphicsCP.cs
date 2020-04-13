using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.Interfaces;
using SkiaSharpGeneralLibrary.SKExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Dice
{
    public class StandardDiceGraphicsCP : ObservableObject
    {
        public IRepaintControl? PaintUI;
        public float ActualWidthHeight { get; set; }
        public float MinimumWidthHeight { get; set; }
        public StandardDiceGraphicsCP(IRepaintControl paintUI)
        {
            PaintUI = paintUI; //to make sure i don't forget.
            Load();
        }
        private void Load()
        {
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            PopulateDotPaint();
            PopulateFillPaint();
            MiscHelpers.DefaultFont = "Tahoma";
        }
        public StandardDiceGraphicsCP()
        {
            Load();
        }
        private string _FillColor = cs.White; //most of the time, its white
        public string FillColor
        {
            get { return _FillColor; }
            set
            {
                if (SetProperty(ref _FillColor, value))
                {
                    PopulateFillPaint();
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private string _DotColor = cs.Black;
        public string DotColor
        {
            get { return _DotColor; }
            set
            {
                if (SetProperty(ref _DotColor, value))
                {
                    PopulateDotPaint();
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 6 && Style == EnumStyle.Regular) //should have been regular.
                    throw new BasicBlankException("Only supports 1 to 6 for normal dice.  if the value is higher than 6, this should have not been used");
                if (SetProperty(ref _Value, value))
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        private EnumStyle _Style;
        public EnumStyle Style
        {
            get { return _Style; }
            set
            {
                if (SetProperty(ref _Style, value))
                {
                    if (value == EnumStyle.DrawWhiteNumber)
                        _selectPaint = BaseDeckGraphicsCP.GetDarkerSelectPaint();
                }
            }
        }
        public SKPoint Location { get; set; } = new SKPoint(0, 0); //because sometimes it needs to be drawn elsewhere.
        public double OriginalHeightWidth { get; set; }
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
                    PaintUI!.DoInvalidate();
            }
        }
        private bool _Hold;
        public bool Hold
        {
            get { return _Hold; }
            set
            {
                if (SetProperty(ref _Hold, value))
                    PaintUI!.DoInvalidate();
            }
        }
        public float NormalBorderWidth { get; set; } = 6;
        public float RoundedRadius { get; set; } = 8;
        public bool NeedsToClear { get; set; } = true; //sometimes it won't clear.
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
                    PaintUI!.DoInvalidate();
            }
        }
        public void UseSmallerBorders()
        {
            NormalBorderWidth = 2; // this is because its used inside of something else.
            if (ActualWidthHeight <= 60)
                RoundedRadius = 2;
            else
                RoundedRadius = 8;
        }
        private SKPaint? _dotPaint;
        private SKPaint? _selectPaint; //i may have to rethink (not sure).
        private SKPaint? _fillPaint;
        private SKPaint? _borderPaint;
        private void PopulateFillPaint()
        {
            _fillPaint = MiscHelpers.GetSolidPaint(FillColor);
        }
        private void PopulateDotPaint()
        {
            _dotPaint = MiscHelpers.GetSolidPaint(DotColor);
        }
        public void DrawDice(SKCanvas dc)
        {
            if (NeedsToClear == true)
                dc.Clear();
            if (Value == 0)
                return;  //because the dice value is 0.
            SKRect rect_Card;
            rect_Card = SKRect.Create(Location.X, Location.Y, ActualWidthHeight, ActualWidthHeight); //i think
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _fillPaint);
            if (IsSelected == true || Hold == true)
                dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _selectPaint);
            if (_borderPaint == null)
                _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, NormalBorderWidth);
            dc.DrawRoundRect(rect_Card, RoundedRadius, RoundedRadius, _borderPaint);
            FinishDrawing(dc, rect_Card);
        }
        private void DrawBorderText(SKCanvas canvas, SKRect rect_Card)
        {
            float tempFont;
            if (Value == 10)
                tempFont = rect_Card.Width / 1.3f;
            else
                tempFont = rect_Card.Width / 0.9f;
            using var FillPaint = MiscHelpers.GetTextPaint(SKColors.White, tempFont);
            using var StrokePaint = MiscHelpers.GetTextPaint(SKColors.Black, tempFont);
            canvas.DrawBorderText(Value.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, FillPaint, StrokePaint, rect_Card);
        }
        private void FinishDrawing(SKCanvas canvas, SKRect rect_Card)
        {
            if (Style == EnumStyle.DrawWhiteNumber)
            {
                DrawBorderText(canvas, rect_Card);
                return; // will try to do something else
            }
            var radius = rect_Card.Width / 10;
            switch (Value)
            {
                case 1:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2), radius, _dotPaint);
                        break;
                    }
                case 2:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        break;
                    }
                case 3:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        break;
                    }
                case 4:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        break;
                    }
                case 5:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        break;
                    }
                case 6:
                    {
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height * 3 / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height / 4), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 2), radius, _dotPaint);
                        canvas.DrawCircle(rect_Card.Left + (rect_Card.Width * 3 / 4), rect_Card.Top + (rect_Card.Height / 2), radius, _dotPaint);
                        break;
                    }
            }
        }
    }
}