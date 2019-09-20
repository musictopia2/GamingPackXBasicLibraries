using BasicGameFramework.GameGraphicsCP.MiscClasses;
namespace BasicGameFramework.GameGraphicsCP.Interfaces
{
    public interface ISkiaSharpGameBoard
    {
        event SingleClickBoardEventHandler SingleClickBoard;
        event CPPaintEventHandler CPPaint; // has to process the events for this then.
    }
}