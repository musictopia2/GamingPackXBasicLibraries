using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
namespace BasicGameFramework.GameGraphicsCP.CheckersChessHelpers
{
    public abstract class CheckersChessSpace<G> where G : BaseGraphicsCP, new() // no need for databinding this time.  because a method will have to be called to repaint now.
    {
        public int MainIndex { get; set; }
        public int ReversedIndex { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool WasReversed { get; set; } // in some cases, it needs to know this.
        internal SKPaint? MainPaint;
        protected abstract EnumGame GetGame();
        public SKRect ThisRect { get; set; } // this is the rectangle which is needed in order to draw things on it.
        public SKPoint GetLocation()
        {
            if (_thisGame == EnumGame.Checkers)
                return new SKPoint(ThisRect.Location.X, ThisRect.Location.Y);
            return new SKPoint(ThisRect.Location.X + 3, ThisRect.Location.Y + 3); // can rethink if necessary (?)
        }
        public SKSize GetSize()
        {
            if (_thisGame == EnumGame.Checkers)
                return ThisRect.Size;
            return new SKSize(32, 32); // must be 32 by 32 for the images being drawn.
        }
        public abstract void ClearSpace(); // whatever needs to be done to clear the space.
        protected internal virtual void HighlightSpaces(SKCanvas thisCanvas) { }
        protected internal abstract G? GetGamePiece();
        private readonly EnumGame _thisGame;
        public CheckersChessSpace()
        {
            _thisGame = GetGame();
        }
    }
}