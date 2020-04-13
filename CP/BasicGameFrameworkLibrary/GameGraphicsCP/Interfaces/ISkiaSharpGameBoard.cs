using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces
{
    public interface ISkiaSharpGameBoard
    {
        event SingleClickBoardEventHandler SingleClickBoard;
        event CPPaintEventHandler CPPaint; // has to process the events for this then.
    }
}