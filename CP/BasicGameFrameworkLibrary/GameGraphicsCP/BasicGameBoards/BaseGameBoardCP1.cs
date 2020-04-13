using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using SkiaSharp;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards
{
    public abstract class BaseGameBoardCP<G> : IBaseGameBoardCP where G : BaseGraphicsCP, new()
    {
        public abstract string TagUsed { get; }
        public bool DrawBoardEarly { protected set; get; } = false; //default to false.

        private readonly IGamePackageResolver _mainContainer;
        private ISkiaSharpGameBoard? _thisElement;
        private readonly IProportionBoard _thisPro;

        public void LinkBoard()
        {
            //if (_thisElement != null)
            //{
            //    return;
            //}
            _thisElement = _mainContainer.Resolve<ISkiaSharpGameBoard>(TagUsed); //hopefully this works.
            _thisElement.CPPaint += ThisElement_CPPaint;
            _thisElement.SingleClickBoard += ThisElement_SingleClickBoard;
        }

        public BaseGameBoardCP(IGamePackageResolver mainContainer, ISkiaSharpGameBoard thisElement)
        {
            _mainContainer = mainContainer;
            _thisPro = mainContainer.Resolve<IProportionBoard>(TagUsed);
            
            //for games like countdown where each player has their own, i can't register.  otherwise, would get hosed.
            _thisElement = thisElement;
            _thisElement.CPPaint += ThisElement_CPPaint;
            _thisElement.SingleClickBoard += ThisElement_SingleClickBoard;
        }
        public BaseGameBoardCP(IGamePackageResolver mainContainer) //we need the advanced one here though.
        {

            _thisPro = mainContainer.Resolve<IProportionBoard>(TagUsed);
            _mainContainer = mainContainer;
            if (mainContainer.RegistrationExist<ISkiaSharpGameBoard>(TagUsed))
            {
                LinkBoard();
            }
        }
        private async void ThisElement_SingleClickBoard(double x, double y)
        {
            await ClickProcessAsync(new SKPoint((int)x, (int)y));
        }
        private async void ThisElement_CPPaint(SKCanvas canvas, double width, double height)
        {
            if (width == 0 || height == 0)
                return;
            if (height == 2)
            {
                return; //because i don't think it should be 2.
            }
            if (CanStartPaint() == false)
                return;
            bool needsfirst = false;
            if (_didPaint == false)
            {
                needsfirst = true;
                _actualHeight = (int)height;
                _actualWidth = (int)width;
                SetUpPaints();
                CreateSpaces();
                _didPaint = true; // because you already started it.
            }
            DrawBoard(canvas);
            if (needsfirst && BasicGameBoardDelegates.AfterPaintAsync != null) //maybe had to paint first or access violation.
            {
                await BasicGameBoardDelegates.AfterPaintAsync(); //hopefully this simple.
            }
        }
        public virtual void DrawGraphicsForBoard(SKCanvas canvas, float width, float height) { } //i think its best for games like candyland to just use this one.  if i make abstract, then it breaks every game.  not all need it.
        protected abstract void CreateSpaces();
        protected abstract SKSize OriginalSize { get; set; }
        public SKSize SuggestedSize()
        {
            float diffs;
            diffs = _thisPro.Proportion;
            var tempSize = OriginalSize;

            return new SKSize(tempSize.Width * diffs, tempSize.Height * diffs);
        }
        private bool _didPaint;
        protected abstract bool CanStartPaint();
        public void SetDimensions(int width, int height) //this is also needed when doing 2 parts (especially from tablets).
        {
            _actualWidth = width;
            _actualHeight = height;
        }
        private int _actualWidth; //i don't think we need property this time (?)
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
            var heigthDiffs = _actualHeight / OriginalSize.Height;
            return new SKSize(width * widthDiffs, height * heigthDiffs);
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
            SKPoint newPoint;
            SKSize newSize;
            newPoint = GetActualPoint(oldPoint);
            newSize = GetActualSize(oldSize);
            return SKRect.Create(newPoint, newSize);
        }
        protected abstract Task ClickProcessAsync(SKPoint thisPoint);
        /// <summary>
        /// for games like candyland, this should not draw the board itself but the things on the board.  for the board itself, call DrawGraphicsForBoard
        /// </summary>
        /// <param name="canvas"></param>
        protected abstract void DrawBoard(SKCanvas canvas);
        protected virtual G GetGamePiece(string color, SKPoint location)
        {
            G thisPiece = new G();
            {
                var withBlock = thisPiece;
                withBlock.Location = location;
                withBlock.MainColor = color;
                withBlock.ActualHeight = PieceHeight;
                withBlock.ActualWidth = PieceWidth;
                withBlock.NeedsToClear = false; // can't clear in this case.
            }
            return thisPiece; // i have to override to add other things  add to templates
        }
        protected G GetGamePiece(string color, SKRect bounds)
        {
            return GetGamePiece(color, bounds.Location);
        }
    }
}