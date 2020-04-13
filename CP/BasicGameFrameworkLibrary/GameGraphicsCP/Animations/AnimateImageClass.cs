using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.Animations
{
    public class AnimateImageClass : IHandle<ClearAnimationEventModel>
    {
        public ICanvas? GameBoard { get; set; }
        private ISelectableObject? _image;
        public int LongestTravelTime { get; set; }
        private const float _interval = 20;
        private int _destinationX;
        private int _destinationY;
        private int _startX;
        private int _startY;
        private int _totalSteps;
        public SKPoint LocationFrom { get; set; }
        public SKPoint LocationTo { get; set; }
        public bool CanClearAtEnd { get; set; }
        public AnimateImageClass()
        {
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
        }
        public void Handle(ClearAnimationEventModel message)
        {
            GameBoard!.Clear();
        }
        public async Task DoAnimateAsync(ISelectableObject thisImage)
        {
            thisImage.IsSelected = false;
            GameBoard!.Clear();
            GameBoard.AddChild(thisImage);
            try
            {
                GameBoard.SetLocation(thisImage, LocationFrom.X, LocationFrom.Y);
            }
            catch (Exception ex)
            {
                UIPlatform.ShowError(ex.Message);
                return;
            }
            _startX = (int)LocationFrom.X;  // myLocation.X
            _startY = (int)LocationFrom.Y; // myLocation.Y
            _destinationX = (int)LocationTo.X;
            _destinationY = (int)LocationTo.Y;
            var temps = LongestTravelTime / _interval;
            _totalSteps = (int)temps;
            _image = thisImage;
            await RunAnimations();
        }
        private async Task RunAnimations()
        {
            try
            {
                double totalXDistance;
                double totalYDistance;
                double eachx = 0;
                double eachy = 0;
                int x;
                int upTox = 0;
                int upToy = 0;
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
                    upTox += (int)eachx;
                    upToy += (int)eachy;
                    GameBoard!.SetLocation(_image!, upTox, upToy);
                }
                await Task.Delay((int)_interval);
                GameBoard!.SetLocation(_image!, _destinationX, _destinationY);
                if (CanClearAtEnd == false)
                    GameBoard.Clear();
            }
            catch (Exception ex)
            {
                UIPlatform.ShowError(ex.Message); //well see how well this works (?)
                return;
            }
        }
    }
}