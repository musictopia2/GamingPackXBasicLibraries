using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Animations
{
    public class AnimateSkiaSharpGameBoard
    {
        readonly EventAggregator _thisE;
        public SKPoint LocationFrom { get; set; }
        public SKPoint LocationTo { get; set; }
        public SKPoint CurrentLocation { get; set; }
        public bool AnimationGoing { get; set; } // so the gameboard knows whether it needs something special or not.
        private float _destinationX;
        private float _destinationY;
        private float _startX;
        private float _startY;
        private float _totalSteps;
        public int LongestTravelTime { get; set; }
        private const float _interval = 20;
        public AnimateSkiaSharpGameBoard()
        {
            _thisE = Resolve<EventAggregator>();

        }
        public async Task DoAnimateAsync()
        {
            _startX = LocationFrom.X;  // myLocation.X
            _startY = LocationFrom.Y; // myLocation.Y
            _destinationX = LocationTo.X;
            _destinationY = LocationTo.Y;
            _totalSteps = LongestTravelTime / _interval;
            CurrentLocation = LocationFrom; // for now.
            AnimationGoing = true; // so when they access the information, they will do something different.
            _thisE.RepaintMessage(EnumRepaintCategories.FromSkiasharpboard);
            float totalXDistance;
            float totalYDistance;
            float eachx = 0;
            float eachy = 0;
            int x;
            float upTox = 0;
            float upToy = 0;
            await Task.Run(() =>
            {
                totalXDistance = _destinationX - _startX;
                totalYDistance = _destinationY - _startY;
                eachx = totalXDistance / _totalSteps;
                eachy = totalYDistance / _totalSteps;
                upTox = _startX;
                upToy = _startY;
            });
            var loopTo = _totalSteps - 1;
            for (x = 1; x <= loopTo; x++)
            {
                await Task.Delay((int)_interval);
                upTox += eachx;
                upToy += eachy;
                CurrentLocation = new SKPoint(upTox, upToy);
                _thisE.RepaintMessage(EnumRepaintCategories.FromSkiasharpboard);
            }
            AnimationGoing = false;
        }
    }
}