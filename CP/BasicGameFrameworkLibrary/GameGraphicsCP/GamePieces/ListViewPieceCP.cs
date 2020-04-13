using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces
{
    public class ListViewPieceCP : BaseGraphicsCP, ISimpleValueObject<int>, ISelectableObject, IEnabledObject
    {

        private int _Index; // this is only for information for something else.  will not influence what is drawn.
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                if (SetProperty(ref _Index, value) == true) { }
            }
        }

        private string _DisplayText = "";
        public string DisplayText
        {
            get
            {
                return _DisplayText;
            }

            set
            {
                if (SetProperty(ref _DisplayText, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }

        private bool _CanHighlight = true;
        public bool CanHighlight
        {
            get
            {
                return _CanHighlight;
            }
            set
            {
                if (SetProperty(ref _CanHighlight, value) == true)
                {
                    NeedsHighlighting = CanHighlight;
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }

        private SKColor _TextColor = SKColors.Navy;
        public SKColor TextColor
        {
            get
            {
                return _TextColor;
            }

            set
            {
                if (SetProperty(ref _TextColor, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }

        private bool _SmallerFontSize = false;
        public bool SmallerFontSize
        {
            get
            {
                return _SmallerFontSize;
            }
            set
            {
                if (SetProperty(ref _SmallerFontSize, value) == true)
                {
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate();
                }
            }
        }
        public int ReadMainValue => Index; //i think
        public ListViewPieceCP()
        {
            NeedsHighlighting = true;
        }
        protected virtual void SelectProcesses() { }
        protected virtual bool CanDrawText()
        {
            return true;
        }
        public override void DrawImage(SKCanvas dc)
        {
            var thisRect = GetMainRect();
            if (CanDrawText() == false)
                return;
            if (NeedsToClear == true)
                dc.Clear();// has to clear everytime.
            SelectProcesses();
            DrawSelector(dc);
            float fontSize;
            if (SmallerFontSize == false)
                fontSize = thisRect.Height * 0.8f; // i think
            else
                fontSize = 20;
            var TextPaint = MiscHelpers.GetTextPaint(TextColor, fontSize);

            dc.DrawCustomText(DisplayText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, TextPaint, thisRect, out _); // i think
        }
    }
}