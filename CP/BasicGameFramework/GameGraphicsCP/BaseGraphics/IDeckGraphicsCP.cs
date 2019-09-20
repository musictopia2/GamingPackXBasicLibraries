using SkiaSharp;
namespace BasicGameFramework.GameGraphicsCP.BaseGraphics
{
    public interface IDeckGraphicsCP //maybe not needed since its done with the other. 
    {
        BaseDeckGraphicsCP? MainGraphics { get; set; } //because wpf needs to send in that one too.
        bool Drew { get; set; } //still needs this part.
        bool CanStartDrawing();
        /// <summary>
        /// most of the time, will use the default function.  however, if you need to do something different, you do.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rect_Card"></param>
        /// <param name="ThisPaint"></param>
        void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint); //this is the default.
        void DrawBorders(SKCanvas dc, SKRect rect_Card); //so you can use those functions if needed.
        bool NeedsToDrawBacks { get; } //some games don't even need to draw backs like dice games.
        void DrawBacks(SKCanvas canvas, SKRect rect_Card);
        void DrawImage(SKCanvas canvas, SKRect rect_Card);
        SKColor GetFillColor(); //the color cards can do something different.

        /// <summary>
        /// most of the time, use getdefaultrect but you could do something else.
        /// </summary>
        /// <returns></returns>
        SKRect GetDrawingRectangle();
        void Init(); //this would happen after you create the object.
    }
}