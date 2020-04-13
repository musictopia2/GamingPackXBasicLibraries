using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Linq;
using static SkiaSharpGeneralLibrary.SKExtensions.MiscHelpers;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Cards
{
    public abstract class BaseColorCardsCP : ObservableObject, IDeckGraphicsCP
    {
        public static string TagUsed => "colorcard"; //standard enough.
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        protected abstract SKColor BackColor { get; }
        protected abstract SKColor BackFontColor { get; }
        protected abstract string BackText { get; }
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }
        private EnumColorTypes _color;
        public EnumColorTypes Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value))
                    MainGraphics!.PaintUI!.DoInvalidate(); //i think here too.
            }
        }
        public bool NeedsToDrawBacks => true; //for sure yes on this one.

        private string _value = "";
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (SetProperty(ref _value, value) == true)
                    // code to run
                    MainGraphics!.PaintUI!.DoInvalidate();
            }
        }// this requires binding.
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {

            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            if (BackText == "")
                throw new BasicBlankException("Must have the back text");
            var thisList = BackText.Split(" ").ToList();
            if (thisList.Count > 2)
                throw new Exception("Only 2 lines are supported currently");
            var thisFill = GetFillColor();
            var thisPaint = GetSolidPaint(thisFill);
            DrawDefaultRectangles(canvas, rect_Card, thisPaint);
            SKPaint textPaint;
            CustomBasicList<SKRect> rectList = new CustomBasicList<SKRect>();
            if (thisList.Count == 1)
            {
                rectList.Add(rect_Card);
            }
            else
            {
                SKRect firstRect;
                firstRect = SKRect.Create(8, 8, rect_Card.Width - 6, rect_Card.Height / 2.1f);
                var secondRect = SKRect.Create(8, rect_Card.Height / 2, rect_Card.Width - 6, rect_Card.Height / 2.1f);
                rectList.Add(firstRect);
                rectList.Add(secondRect);
            }
            if (rectList.Count != thisList.Count)
                throw new BasicBlankException("Numbers Don't Match For Drawing Backs");
            int x = 0;
            foreach (var thisRect in rectList)
            {
                float fontSize;
                var ThisText = thisList[x];
                if (ThisText.Length == 5)
                    fontSize = rect_Card.Height / 4.2f;
                else
                    fontSize = rect_Card.Height / 3.6f;

                textPaint = MiscHelpers.GetTextPaint(BackFontColor, fontSize);
                canvas.DrawBorderText(ThisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder!, thisRect);
                x++;
            }
        }
        public virtual SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return BackColor;
            if (Color == EnumColorTypes.None || Color == EnumColorTypes.ZOther)
                throw new BasicBlankException("Not Supported");
            return PrivateColor();
        }
        protected virtual SKColor PrivateColor()
        {
            return Color.ToColor().ToSKColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetMainRect();
        }
        private SKPaint? _textBorder;
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        public void Init()
        {
            if (MainGraphics == null)
                throw new BasicBlankException("You never sent in the main graphics for helpers");
            _textBorder = GetStrokePaint(SKColors.Black, 2);
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            _selectPaint = GetSolidPaint(otherColor);
            thisColor = SKColors.White;
            otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 150);
            _pDrewPaint = GetSolidPaint(otherColor);
        }
        public abstract bool CanStartDrawing();
        protected void DrawValueCard(SKCanvas canvas, SKRect rect_Card, string valueNeeded) // to be more flexible
        {
            float fontSize;
            fontSize = (rect_Card.Height * 0.5f); // has to experiment
            var textPaint = GetTextPaint(SKColors.White, fontSize);
            canvas.DrawBorderText(valueNeeded, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder!, rect_Card);
        }
        public abstract void DrawImage(SKCanvas canvas, SKRect rect_Card);
    }
}