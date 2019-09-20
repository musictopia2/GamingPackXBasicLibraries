using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGameFramework.GameGraphicsCP.Animations
{
    public class AnimateTrickClass<S, T>
        where S : struct, Enum
        where T : class, ITrickCard<S>
    {
        public ITrickCanvas? GameBoard { get; set; }
        private class TrickInfo
        {
            public float StartX { get; set; }
            public float StartY { get; set; }
            public float DiffX { get; set; }
            public float DiffY { get; set; }
            public float CurrentX { get; set; }
            public float CurrenyY { get; set; }
            public int Index { get; set; } // needs index because the winning card will not be here.
        }
        public int LongestTravelTime { get; set; }
        private const float _internval = 20;
        private int _destinationX;
        private int _destinationY;
        private int _totalSteps;
        private CustomBasicList<TrickInfo>? _trickList;
        private BasicTrickAreaViewModel<S, T>? _tempArea;
        public async Task MovePiecesAsync(BasicTrickAreaViewModel<S, T> thisArea)
        {
            _tempArea = thisArea;
            _destinationX = (int)thisArea.WinCard!.Location.X;
            _destinationY = (int)thisArea.WinCard.Location.Y;
            var index = thisArea.CardList.IndexOf(thisArea.WinCard);
            if (index == -1)
                throw new BasicBlankException("Win card not found");
            var ThisLoc = GameBoard!.GetStartingPoint(index);
            _destinationX = (int)ThisLoc.X;
            _destinationY = (int)ThisLoc.Y;
            _trickList = new CustomBasicList<TrickInfo>();
            var temps = LongestTravelTime / _internval;
            _totalSteps = (int)temps;
            if (LongestTravelTime == 0)
                throw new BasicBlankException("Longest travel time not set");
            foreach (var thisCard in _tempArea.CardList)
            {
                if (thisCard.Equals(thisArea.WinCard) == false)
                {
                    TrickInfo thisTrick = new TrickInfo();
                    thisTrick.Index = _tempArea.CardList.IndexOf(thisCard);
                    var thisLocation = GameBoard.GetStartingPoint(thisTrick.Index);
                    thisTrick.CurrentX = thisLocation.X;
                    thisTrick.CurrenyY = thisLocation.Y;
                    thisTrick.StartX = thisLocation.X;
                    thisTrick.StartY = thisLocation.Y;
                    double totalxDistance = default;
                    double totalYDistance = default;
                    totalxDistance = (double)_destinationX - thisTrick.StartX;
                    totalYDistance = (double)_destinationY - thisTrick.StartY;
                    thisTrick.DiffX = (float)totalxDistance / _totalSteps;
                    thisTrick.DiffY = (float)totalYDistance / _totalSteps;
                    _trickList.Add(thisTrick);
                }
            }
            await RunAnimationsAsync();
        }
        private async Task RunAnimationsAsync()
        {
            try
            {
                await Task.Delay(500); // so others can see what was put down before the animations start.
                int x;
                var loopTo = _totalSteps - 1;
                for (x = 1; x <= loopTo; x++)
                {
                    await Task.Delay((int)_internval);
                    foreach (var thisTrick in _trickList!)
                    {
                        thisTrick.CurrentX += thisTrick.DiffX;
                        thisTrick.CurrenyY += thisTrick.DiffY;
                        GameBoard!.SetLocation(thisTrick.Index, thisTrick.CurrentX, thisTrick.CurrenyY);
                    }
                }
                foreach (var thisCard in _tempArea!.CardList)
                    thisCard.Visible = false;
                foreach (var thisTrick in _trickList!)
                    GameBoard!.SetLocation(thisTrick.Index, thisTrick.StartX, thisTrick.StartY);
            }
            catch (Exception ex)
            {
                IError thisE = Resolve<IError>(); //this should now work.
                thisE.ShowError(ex.Message);
                return;
            }
        }
    }
}