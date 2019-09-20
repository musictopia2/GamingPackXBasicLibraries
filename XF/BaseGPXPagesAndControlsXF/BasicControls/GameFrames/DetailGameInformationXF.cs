using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.GameFrames
{
    public class DetailGameInformationXF : BaseFrameXF
    {
        private Grid? _mainGrid;
        private SimpleLabelGridXF? _thisHelp;
        protected override void FirstSetUp()
        {
            _thisHelp = new SimpleLabelGridXF();
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            base.FirstSetUp();
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            ScrollView thisScroll = new ScrollView();
            Text = "Additional Information"; // defaults to that but its adjustable
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            _mainGrid = _thisHelp.GetContent; // hopefully as i add to it, its still okay.
            thisScroll.Orientation = ScrollOrientation.Both;
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