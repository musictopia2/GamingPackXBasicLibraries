using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameBoards
{
    public abstract class BasicGameBoard<T> : UserControl where T : class, IBasicSpace, new()
    {
        protected abstract Control GetControl(T thisItem, int index); // this is where i do all the necessary bindings
        private readonly Grid _thisGrid;
        protected IBasicGameVM GameModel;
        private bool _hasHeaders;
        protected double GetHeight()
        {
            Control thisCon;
            thisCon = (Control)_thisGrid.Children[0];
            return thisCon.ActualHeight; // try this way.
        }
        protected UIElement FindControl(int row, int column)
        {
            foreach (var thisControl in _thisGrid.Children)
            {
                var tempItem = thisControl as UIElement;
                var tempRow = Grid.GetRow(tempItem);
                var tempColumn = Grid.GetColumn(tempItem);
                if (tempRow == row && tempColumn == column)
                    return tempItem!;
            }
            throw new Exception("Control not found for row " + row + " and column " + column);
        }
        protected void AddControlToGrid(Control thisCon, int row, int column)
        {
            GridHelper.AddControlToGrid(_thisGrid, thisCon, row, column); // so the inherited versions still don't have to know about it.
        }
        public void CreateHeadersColumnsRows(CustomBasicList<string> rowSource, CustomBasicList<string> columnSource, string veryFirstHeader = "")
        {
            GridHelper.AddAutoColumns(_thisGrid, 1);
            GridHelper.AddAutoRows(_thisGrid, 1);
            _hasHeaders = true;
            int x = 0;
            foreach (var thisRow in rowSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisRow.ToString()); // can always use this
                if (LeftRightMargins > 0)
                    thisLabel.Margin = new Thickness(LeftRightMargins, 0, LeftRightMargins, 0);
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, x, 0); // i think 0 is fine here.
            }
            x = 0;
            foreach (var thisColumn in columnSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisColumn.ToString());
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
        protected IBoardCollection<T>? PieceList;
        private TextBlock CreateLabel(string text)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.Text = text;
            thisLabel.Foreground = Brushes.White;
            if (CanHeadersBold == true)
                thisLabel.FontWeight = FontWeights.Bold;
            else
                thisLabel.FontWeight = FontWeights.Regular;
            thisLabel.FontSize = HeaderFontSize;
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            thisLabel.VerticalAlignment = VerticalAlignment.Center; // needs to be centered.
            return thisLabel;
        }
        public void UpdateControls(IBoardCollection<T> itemsSource)
        {
            _thisGrid.Children.Clear();
            CreateControls(itemsSource); //try this.  hopefully this simple.
        }
        public void CreateControls(IBoardCollection<T> itemsSource) // has to be at least this view model
        {
            if (itemsSource == null)
                throw new BasicBlankException("The list is null.  Therefore, can't create controls");
            if (itemsSource.Count() == 0)
                throw new Exception("Must have at least one item from the list.  Otherwise, it will show nothing.  If there are exceptions, then can take out");
            PieceList = itemsSource;
            LoadGridColumnRows(); //try this.
            int x;
            int y;
            int z = 0;
            var loopTo = itemsSource.GetTotalRows();
            for (x = 1; x <= loopTo; x++)
            {
                var loopTo1 = itemsSource.GetTotalColumns();
                for (y = 1; y <= loopTo1; y++)
                {
                    if (z < itemsSource.Count())
                    {
                        if (CanAddControl(itemsSource, x, y) == true)
                        {
                            var thisItem = itemsSource[x, y];
                            var thisControl = GetControl(thisItem, z); // needs to get in the habit of using 0 based
                            if (_hasHeaders == false)
                                GridHelper.AddControlToGrid(_thisGrid, thisControl, x - 1, y - 1);
                            else
                                GridHelper.AddControlToGrid(_thisGrid, thisControl, x, y);// because of the headers and footers.
                            z += 1; // 0 based
                        }
                    }
                }
            }
        }
        protected SKPoint GetLocation(object thisItem)
        {
            foreach (var thisFirst in _thisGrid.Children)
            {
                var thisCon = (Control)thisFirst!;
                if (thisCon.DataContext == thisItem == true)
                {
                    var thisRow = Grid.GetRow(thisCon);
                    var thisCol = Grid.GetColumn(thisCon);
                    var x = CalculateLeft((float)thisCon.Width, thisCol);
                    x += (float)Padding.Left;
                    var y = thisRow * thisCon.Height;
                    y += Padding.Top;
                    return new SKPoint(x, (float)y);
                }
            }
            return new SKPoint(0, 0);
        }
        protected virtual float CalculateLeft(float oldWidth, float column)
        {
            return column * oldWidth;
        }
        protected virtual bool CanAddControl(IBoardCollection<T> itemsSource, int row, int column) => true; //defaults to true.
        protected Binding GetColorBinding(string colorPath)
        {
            Binding thisBind = new Binding(colorPath);
            Converters.ColorConverter ThisC = new Converters.ColorConverter();
            thisBind.Converter = ThisC;
            return thisBind;
        }
        protected Binding GetCommandBinding(string commandPath)
        {
            Binding thisBind = new Binding(commandPath);
            thisBind.Source = GameModel; // i think
            return thisBind;
        }
        protected virtual void StartInit() { }
        protected Grid ParentGrid;
        protected void LoadGridColumnRows() // may not have to override because bowling dice game is using a different one that behaves a little differently.
        {
            GridHelper.AddAutoRows(_thisGrid, PieceList!.GetTotalRows());
            GridHelper.AddAutoColumns(_thisGrid, PieceList.GetTotalColumns());
        }
        public BasicGameBoard()
        {
            ParentGrid = new Grid();
            _thisGrid = new Grid();
            ParentGrid.Children.Add(_thisGrid);
            StartInit();
            GameModel = Resolve<IBasicGameVM>();
            Content = ParentGrid;
        }
    }
}