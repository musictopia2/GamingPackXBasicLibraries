using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXPagesAndControlsXF.BasicControls.GameBoards
{
    public class CompleteGameBoardXF<B> : ContentView, IHandle<RepaintEventModel>
       where B : class, IBaseGameBoardCP
    {
        public readonly SkiaSharpGameBoardXF ThisElement; //must be public now though.
        private SKCanvasView? _otherElement;
        private B? _privateBoard;
        public void LoadBoard()
        {
            _privateBoard = Resolve<B>();
            ThisElement.EnableTouchEvents = true;
            ThisElement.Touch += ThisElement_Touch;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = _privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            HorizontalOptions = LayoutOptions.Start;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            if (_privateBoard.DrawBoardEarly)
            {
                _otherElement = new SKCanvasView();
                _otherElement.PaintSurface += OtherPaint; //hopefully no need for inputtransparent (?)
                Grid grid = new Grid();
                grid.Children.Add(_otherElement);
                grid.Children.Add(ThisElement);
                Content = grid;
                return;
            }
            Content = ThisElement;
        }
        private void ThisElement_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            ThisElement.StartClick(e.Location.X, e.Location.Y);
        }
        private void OtherPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            _privateBoard!.SetDimensions(e.Info.Width, e.Info.Height);
            _privateBoard.DrawGraphicsForBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height); //hopefully can cast properly.
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateSurface();
        }
        public CompleteGameBoardXF()
        {
            ThisElement = new SkiaSharpGameBoardXF();
        }
    }
}