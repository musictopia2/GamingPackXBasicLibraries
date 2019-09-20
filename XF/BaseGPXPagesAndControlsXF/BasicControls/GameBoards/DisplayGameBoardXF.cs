using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.GameBoards
{
    /// <summary>
    /// this is intended to be used on games like bowling dice game.
    /// where it has columns and rows but its mainly for display.
    /// a player does not technically move to the space.
    /// 
    /// </summary>
    public abstract class DisplayGameBoardXF : ContentView //for now, i think this can represent anything.
    {
        protected abstract int HowManyRows { get; }
        protected abstract int HowManyColumns { get; }
        private readonly Grid _thisGrid;
        protected View FindControl(int row, int column)
        {
            foreach (var thisControl in _thisGrid.Children)
            {
                var tempRow = Grid.GetRow((View)thisControl!);
                var tempColumn = Grid.GetColumn((View)thisControl!);
                if (tempRow == row && tempColumn == column)
                    return (View)thisControl!;
            }
            throw new BasicBlankException("Control not found for row " + row + " and column " + column);
        }
        protected void AddControlToGrid(ContentView thisCon, int row, int column)
        {
            GridHelper.AddControlToGrid(_thisGrid, thisCon, row, column); // so the inherited versions still don't have to know about it.
        }
        public void CreateHeadersColumnsRows(CustomBasicList<string> rowSource, CustomBasicList<string> columnSource, string veryFirstHeader = "")
        {
            GridHelper.AddAutoColumns(_thisGrid, 1);
            GridHelper.AddAutoRows(_thisGrid, 1);
            if (rowSource.Count != HowManyRows)
                throw new BasicBlankException("The row headers must equal the row count");
            if (columnSource.Count != HowManyColumns)
                throw new BasicBlankException("The column headers must equal the column count");
            int x = 0;
            foreach (var thisRow in rowSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisRow); // can always use this
                if (LeftRightMargins > 0)
                    thisLabel.Margin = new Thickness(LeftRightMargins, 0, LeftRightMargins, 0);
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, x, 0); // i think 0 is fine here.
            }
            x = 0;
            foreach (var thisColumn in columnSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisColumn);
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 0, x);
            }
            if (veryFirstHeader != "")
            {
                var thisLabel = CreateLabel(veryFirstHeader);
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 0, 0); // this has a header for both.  to support bowling dice game.
            }
        }
        protected bool CanHeadersBold = true;
        protected float HeaderFontSize = 14;
        protected float LeftRightMargins = 0;
        private Label CreateLabel(string text)
        {
            Label thisLabel = new Label();
            thisLabel.Text = text;
            thisLabel.TextColor = Color.White;
            if (CanHeadersBold == true)
                thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.FontSize = HeaderFontSize;
            thisLabel.HorizontalOptions = LayoutOptions.Center;
            thisLabel.VerticalOptions = LayoutOptions.Center; // needs to be centered.
            return thisLabel;
        }
        protected virtual void StartInit() { }
        protected Grid ParentGrid;
        protected virtual void LoadGridColumnRows() // may have to override this because for bowling dice game, we don't know for a while how many columns
        {
            GridHelper.AddAutoRows(_thisGrid, HowManyRows);
            GridHelper.AddAutoColumns(_thisGrid, HowManyColumns);
        }
        public DisplayGameBoardXF()
        {
            ParentGrid = new Grid();
            _thisGrid = new Grid();
            ParentGrid.Children.Add(_thisGrid);
            StartInit();
            LoadGridColumnRows();
            Content = ParentGrid;
        }
    }
}