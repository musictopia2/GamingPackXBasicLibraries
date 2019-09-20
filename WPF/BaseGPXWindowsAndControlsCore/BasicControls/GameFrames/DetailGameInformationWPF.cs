using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameFrames
{
    public class DetailGameInformationWPF : BaseFrameWPF
    {
        private Grid? _mainGrid;
        private SimpleLabelGrid? _thisHelp;
        protected override void FirstSetUp()
        {
            _thisHelp = new SimpleControls.SimpleLabelGrid();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top; // sometimes has to manually do though.
            base.FirstSetUp();
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            ScrollViewer thisScroll = new ScrollViewer();
            Text = "Additional Information"; // defaults to that but its adjustable
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            _mainGrid = _thisHelp.GetContent; // hopefully as i add to it, its still okay.
            thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Content = _mainGrid;
            firstGrid.Children.Add(thisScroll);
            Content = firstGrid;
        }
        public void AddRow(string header, string bindingPath, IValueConverter? converterChoice = null)
        {
            _thisHelp!.AddRow(header, bindingPath, converterChoice);
        }
    }
}