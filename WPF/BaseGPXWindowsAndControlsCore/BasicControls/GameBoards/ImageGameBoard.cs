using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.GameGraphicsCP.Animations;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameBoards
{
    public abstract class ImageGameBoard<T> : BasicGameBoard<T>, IHandleAsync<AnimatePieceEventModel<T>> where T : class, IBasicSpace, new()
    {
        private AnimateImageClass? _animates;
        private CustomCanvas? _thisCanvas;
        protected bool CanClearAtEnd = true;
        private async Task PrepareToAnimateAsync(AnimatePieceEventModel<T> thisPiece)
        {
            _animates!.CanClearAtEnd = CanClearAtEnd;
            var sourceObj = PieceList![thisPiece.PreviousSpace];
            var destinationObj = PieceList[thisPiece.MoveToSpace];
            _animates.LocationFrom = GetLocation(sourceObj);
            if (thisPiece.UseColumn == true)
            {
                var tempHeights = GetHeight() * -1;
                _animates.LocationFrom = new SKPoint(_animates.LocationFrom.X, (float)tempHeights);
            }
            var thisControl = GetControl(thisPiece.TemporaryObject!, -1); // so the control will have that binded to it.
            _animates.LocationTo = GetLocation(destinationObj);
            await _animates.DoAnimateAsync((ISelectableObject)thisControl); // hopefully will still work (?)
        }
        protected override void StartInit() // if i have anything else, has to be in addition to this.
        {
            _thisCanvas = new CustomCanvas();
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            ParentGrid.Children.Add(_thisCanvas);
            _animates.GameBoard = _thisCanvas; // i think
        }
        public async Task HandleAsync(AnimatePieceEventModel<T> message)
        {
            await PrepareToAnimateAsync(message);
        }
        public ImageGameBoard()
        {
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
        }
    }
}