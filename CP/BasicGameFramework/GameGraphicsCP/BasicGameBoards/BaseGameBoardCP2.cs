using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using SkiaSharp;
namespace BasicGameFramework.GameGraphicsCP.BasicGameBoards
{
    public abstract class BaseGameBoardCP //used for games like mexican train dominos where no generics are used.  but mostly the same otherwise.
    {
        public abstract string TagUsed { get; }
        private readonly ISkiaSharpGameBoard _thisElement;
        private readonly IProportionBoard _thisPro;
        public BaseGameBoardCP(IGamePackageResolver mainContainer) //we need the advanced one here though.
        {
            _thisElement = mainContainer.Resolve<ISkiaSharpGameBoard>(TagUsed); //hopefully this works.
            _thisPro = mainContainer.Resolve<IProportionBoard>(TagUsed);
            _thisElement.CPPaint += ThisElement_CPPaint;
            _thisElement.SingleClickBoard += ThisElement_SingleClickBoard;
        }
        private void ThisElement_SingleClickBoard(double x, double y)
        {
            ClickProcess(new SKPoint((int)x, (int)y));
        }
        private void ThisElement_CPPaint(SKCanvas canvas, double width, double height)
        {
            if (width == 0 || height == 0)
                return;
            if (CanStartPaint() == false)
                return;
            if (_didPaint == false)
            {
                _actualHeight = (int)height;
                _actualWidth = (int)width;
                SetUpPaints();
                CreateSpaces();
                _didPaint = true; // because you already started it.
            }
            DrawBoard(canvas);
        }
        protected abstract void CreateSpaces();
        protected abstract SKSize OriginalSize { get; set; }
        public SKSize SuggestedSize()
        {
            float diffs;
            diffs = _thisPro.Proportion;
            var TempSize = OriginalSize;

            return new SKSize(TempSize.Width * diffs, TempSize.Height * diffs);
        }
        private bool _didPaint;
        protected abstract bool CanStartPaint();
        private int _actualWidth; //no need for properties i don't think.
        private int _actualHeight;
        public float PieceHeight { get; set; } //decided to risk doing as float because otherwise games like aggravation can't get to compile as easily.
        public float PieceWidth { get; set; }
        protected SKRect GetBounds()
        {
            return new SKRect(0, 0, _actualWidth, _actualHeight);
        }
        protected float ExtraLeft; // - means left.  plus means right
        protected float ExtraTop; // - means higher.  plus means lower
        protected abstract void SetUpPaints();
        protected float GetFontSize(float originalFontSize)
        {
            var diffs = _actualHeight / OriginalSize.Height;
            return originalFontSize * diffs;
        }
        public SKPoint GetActualPoint(float x, float y)
        {
            var thisPoint = new SKPoint(x, y);
            return GetActualPoint(thisPoint);
        }
        public SKPoint GetActualPoint(SKPoint thisPoint) // there are cases where the gameboard processes needs to know this.
        {
            var widthDiffs = _actualWidth / OriginalSize.Width; // try this way. 
            var heightDiffs = _actualHeight / OriginalSize.Height;
            var tempExtraL = ExtraLeft * widthDiffs;
            var tempExtraT = ExtraTop * heightDiffs; // may need this as well.
            var nextLeft = thisPoint.X + ExtraLeft;
            var nextTop = thisPoint.Y + ExtraTop;
            nextLeft *= widthDiffs;
            nextTop *= heightDiffs;
            return new SKPoint(nextLeft + tempExtraL, nextTop + tempExtraT);
        }
        protected SKSize GetActualSize(float width, float height)
        {
            var widthDiffs = _actualWidth / OriginalSize.Width;
            var heightDiffs = _actualHeight / OriginalSize.Height;
            return new SKSize(width * widthDiffs, height * heightDiffs);
        }
        protected SKSize GetActualSize(SKSize size_Current)
        {
            float int_Width;
            float int_Height;
            int_Width = ((_actualWidth / OriginalSize.Width) * size_Current.Width);
            int_Height = ((_actualHeight / OriginalSize.Height) * size_Current.Height);
            return new SKSize(int_Width, int_Height);
        }
        protected SKRect GetActualRectangle(int x, int y, int width, int height)
        {
            return GetActualRectangle(SKRect.Create(x, y, width, height));
        }
        protected SKRect GetActualRectangle(SKRect originalRectangle)
        {
            SKPoint oldPoint;
            oldPoint = new SKPoint(originalRectangle.Left, originalRectangle.Top);
            SKSize oldSize;
            oldSize = new SKSize(originalRectangle.Width, originalRectangle.Height);
            SKPoint NewPoint;
            SKSize NewSize;
            NewPoint = GetActualPoint(oldPoint);
            NewSize = GetActualSize(oldSize);
            return SKRect.Create(NewPoint, NewSize);
        }
        protected abstract void ClickProcess(SKPoint thisPoint);
        protected abstract void DrawBoard(SKCanvas canvas);
    }
}