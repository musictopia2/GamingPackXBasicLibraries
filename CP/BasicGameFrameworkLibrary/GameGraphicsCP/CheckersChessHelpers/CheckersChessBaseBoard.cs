using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers
{
    public abstract class CheckersChessBaseBoard<G, S> : BaseGameBoardCP<G>
        where G : BaseGraphicsCP, new()
        where S : CheckersChessSpace<G>, new()
    {
        protected static CustomBasicList<S> PrivateSpaceList = new CustomBasicList<S>(); // something else can reference this as well (?)
        protected override SKSize OriginalSize { get; set; }
        public SKPoint SuggestedOffLocation(bool isReversed)
        {
            if (isReversed == true)
                return new SKPoint(0, 0);
            var bounds = GetBounds();
            return new SKPoint(0, bounds.Height);
        }
        public CheckersChessBaseBoard(IGamePackageResolver mainContainer, CommandContainer command) : base(mainContainer)
        {
            _thisGame = GetGame();
            PrivateSpaceList.Clear(); //try this too.
            if ((int)_thisGame == (int)EnumGame.Chess)
                OriginalSize = new SKSize(320, 320); // will be 40 by 40 for each space
            else
                OriginalSize = new SKSize(500, 500);// can experiment.
            _command = command;
        }
        public static int GetIndexByPoint(int row, int column)
        {
            int x;
            int y;
            int z = 0;
            for (x = 1; x <= 8; x++)
            {
                for (y = 1; y <= 8; y++)
                {
                    z += 1;
                    if (x == row && y == column)
                        return z;
                }
            }
            throw new BasicBlankException("Nothing found");
        }
        public static int GetRealIndex(int originalIndex, bool isReversed)
        {
            if (originalIndex == 0)
                return 0;
            if (isReversed == false)
                return originalIndex;
            var thisSpace = (from Items in PrivateSpaceList
                             where Items.MainIndex == originalIndex
                             select Items).Single();
            return thisSpace.ReversedIndex;
        }
        public static S GetSpace(int originalIndex, bool isReversed)
        {
            if (isReversed == false)
            {
                return (from Items in PrivateSpaceList
                        where Items.MainIndex == originalIndex
                        select Items).Single();
            }
            return (from Items in PrivateSpaceList
                    where Items.ReversedIndex == originalIndex
                    select Items).Single();
        }
        public static S? GetSpace(int row, int column)
        {
            if (row < 1 || row > 8)
                return null;
            if (column < 1 || column > 8)
                return null;
            return (from Items in PrivateSpaceList
                    where Items.Row == row && Items.Column == column
                    select Items).Single();
        }
        protected override void CreateSpaces()
        {
            int x;
            int y;
            SKPaint rowPaint;
            SKPaint thisPaint;
            float locX;
            float locY;
            var thisBounds = GetBounds();
            var diffs = thisBounds.Width / 8;
            locY = 0;
            rowPaint = _firstPaint!;
            for (x = 1; x <= 8; x++)
            {
                thisPaint = rowPaint!;
                locX = 0;
                for (y = 1; y <= 8; y++)
                {
                    S thisSpace = new S();
                    thisSpace.ThisRect = SKRect.Create(locX, locY, diffs, diffs);
                    thisSpace.Column = y;
                    thisSpace.Row = x;
                    thisSpace.MainIndex = GetIndexByPoint(x, y);
                    thisSpace.ReversedIndex = GetIndexByPoint(9 - x, 9 - y);
                    thisSpace.MainPaint = thisPaint;
                    if (thisPaint!.Equals(_firstPaint) == true)
                        thisPaint = _secondPaint!;
                    else
                        thisPaint = _firstPaint!;
                    PrivateSpaceList.Add(thisSpace);
                    locX += diffs;
                }
                if (rowPaint!.Equals(_firstPaint) == true)
                    rowPaint = _secondPaint!;
                else
                    rowPaint = _firstPaint!;
                locY += diffs;
            }
            if (_thisGame == EnumGame.Checkers)
            {
                var tempSize = PrivateSpaceList.First().GetSize();
                PieceHeight = tempSize.Height;
                PieceWidth = tempSize.Width;
            }
        }
        public static CustomBasicList<int> GetBlackStartingSpaces()
        {
            CustomBasicList<int> newList = new CustomBasicList<int>();
            for (var x = 6; x <= 8; x++)
            {
                for (var y = 1; y <= 8; y++)
                {
                    var thisSpace = GetSpace(x, y);
                    if (thisSpace!.MainPaint!.Equals(_secondPaint) == true)
                        newList.Add(thisSpace.MainIndex);
                }
            }
            return newList;
        }
        protected abstract bool CanHighlight(); // so if you can't won't even both doing that part.
        public bool Saved;
        private static SKPaint? _firstPaint;
        private static SKPaint? _secondPaint;
        private SKPaint? _greenPaint;
        private readonly EnumGame _thisGame;
        private readonly CommandContainer _command;

        protected override void SetUpPaints()
        {
            if (_thisGame == EnumGame.Checkers)
            {
                _firstPaint = MiscHelpers.GetSolidPaint(SKColors.White);
                _secondPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            }
            else
            {
                _firstPaint = MiscHelpers.GetSolidPaint(SKColors.WhiteSmoke);
                _secondPaint = MiscHelpers.GetSolidPaint(SKColors.Tan);
                _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            }
        }
        //protected abstract void ContinueClick(int index, ICommand thisCommand); // this means you clicked on a space.
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            //await Task.CompletedTask;
            if (CheckersChessDelegates.CanMove == null)
            {
                throw new BasicBlankException("Nobody is handling canmove for checkers/chess.  Rethink");
            }
            if (CheckersChessDelegates.MakeMoveAsync == null)
            {
                throw new BasicBlankException("Nobdy is handling make move for checkes/chess.  Rethink");
            }
            if (CheckersChessDelegates.CanMove.Invoke() == false)
            {
                return;
            }

            //var thisCommand = SpaceCommand();
            //if (thisCommand.CanExecute(0) == false)
            //    return;
            if (PrivateSpaceList.Count == 0)
                throw new Exception("You must have have at least one space to figure out if it can be clicked or not.");
            foreach (var thisSpace in PrivateSpaceList)
            {
                if (MiscHelpers.DidClickRectangle(thisSpace.ThisRect, thisPoint) == true)
                {
                    _command.StartExecuting();
                    await CheckersChessDelegates.MakeMoveAsync(thisSpace.MainIndex);
                    //ContinueClick(thisSpace.MainIndex, thisCommand);
                    return;
                }
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            if (Saved == true)
                return;
            foreach (var thisSpace in PrivateSpaceList)
            {
                SKPaint tempPaint;
                if (thisSpace.MainIndex == SpaceSelected && _thisGame == EnumGame.Chess)
                    tempPaint = _greenPaint!;
                else
                    tempPaint = thisSpace!.MainPaint!;
                canvas.DrawRect(thisSpace.ThisRect, tempPaint);
            }
            foreach (var thisSpace in PrivateSpaceList)
            {
                var thisPiece = thisSpace.GetGamePiece();
                if (thisPiece != null)
                {
                    thisPiece.Location = thisSpace.GetLocation();
                    var ThisSize = thisSpace.GetSize();
                    thisPiece.ActualHeight = ThisSize.Height;
                    thisPiece.ActualWidth = ThisSize.Width;
                    thisPiece.NeedsToClear = false;
                    if ((int)_thisGame == (int)EnumGame.Checkers && thisSpace.MainIndex == SpaceSelected && CanHighlight() == true)
                        thisPiece.MainColor = cs.Yellow;
                    thisPiece.DrawImage(canvas);
                }
            }
            PossibleAnimations(canvas);
            if (CanHighlight() == false || _thisGame == EnumGame.Checkers)
                return;

            foreach (var thisSpace in PrivateSpaceList)
                thisSpace.HighlightSpaces(canvas);
        }
        protected abstract void PossibleAnimations(SKCanvas thisCanvas);
        //protected abstract ICommand SpaceCommand();
        protected abstract EnumGame GetGame();
        public static int SpaceSelected { get; set; } // this is used so it can do something different.
        public void ClearBoard()
        {
            foreach (var thisSpace in PrivateSpaceList)
                thisSpace.ClearSpace();
            AfterClearBoard();
        }
        protected abstract void AfterClearBoard();
    }
}