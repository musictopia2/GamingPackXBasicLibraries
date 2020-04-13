using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using SkiaSharp.Views.WPF;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Input;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.GameBoards
{
    public class CompleteGameBoard<B> : UserControl, IHandle<RepaintEventModel>
       where B : class, IBaseGameBoardCP
    {
        public readonly SkiaSharpGameBoard ThisElement; //must be public now though.
        private SKElement? _otherElement;
        private B? _privateBoard;
        public void LoadBoard()
        {
            _privateBoard = Resolve<B>();
            MouseUp += GameboardWPF_MouseUp;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = _privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            if (_privateBoard.DrawBoardEarly)
            {
                _otherElement = new SKElement();
                _otherElement.PaintSurface += OtherPaint;
                Grid grid = new Grid();
                grid.Children.Add(_otherElement);
                grid.Children.Add(ThisElement);
                Content = grid;
                return;
            }
            Content = ThisElement;
        }
        private void OtherPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            _privateBoard!.DrawGraphicsForBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height); //hopefully can cast properly.
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {

            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void GameboardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisPos = e.GetPosition(ThisElement);
            ThisElement.StartClick(thisPos.X, thisPos.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
        public CompleteGameBoard()
        {
            ThisElement = new SkiaSharpGameBoard();
        }
    }
}
