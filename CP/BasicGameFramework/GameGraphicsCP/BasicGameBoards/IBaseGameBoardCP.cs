using SkiaSharp;
namespace BasicGameFramework.GameGraphicsCP.BasicGameBoards
{
    public interface IBaseGameBoardCP
    {
        bool DrawBoardEarly { get; }
        string TagUsed { get; }
        void DrawGraphicsForBoard(SKCanvas canvas, float width, float height);
        SKSize SuggestedSize();
        void SetDimensions(int width, int height);
    }
}