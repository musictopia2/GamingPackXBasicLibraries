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
using BasicGameFrameworkLibrary.GameBoardCollections;
using System.Windows;
using BasicControlsAndWindowsCore.Helpers;
using System.Windows.Media;
using SkiaSharp;
using System.Windows.Data;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.GameBoards
{
    public abstract class BasicGameBoard<S> : UserControl
        where S : class, IBasicSpace, new()
    {
        protected abstract Control GetControl(S thisItem, int index); // this is where i do all the necessary bindings
        //at least i don't have to worry about that part yet.

        //hopefully no need for view model because its totally different now

        private readonly Grid _mainGrid;
        //protected IBasicGameVM GameModel;
        private bool _hasHeaders;

        protected double GetHeight()
        {
            Control thisCon;
            thisCon = (Control)_mainGrid.Children[0];
            return thisCon.ActualHeight; // try this way.
        }

        protected UIElement FindControl(int row, int column)
        {
            foreach (var thisControl in _mainGrid.Children)
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
            GridHelper.AddControlToGrid(_mainGrid, thisCon, row, column); // so the inherited versions still don't have to know about it.
        }
        public void CreateHeadersColumnsRows(CustomBasicList<string> rowSource, CustomBasicList<string> columnSource, string veryFirstHeader = "")
        {
            GridHelper.AddAutoColumns(_mainGrid, 1);
            GridHelper.AddAutoRows(_mainGrid, 1);
            _hasHeaders = true;
            int x = 0;
            foreach (var thisRow in rowSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisRow.ToString()); // can always use this
                if (LeftRightMargins > 0)
                {
                    thisLabel.Margin = new Thickness(LeftRightMargins, 0, LeftRightMargins, 0);

                }
                GridHelper.AddControlToGrid(_mainGrid, thisLabel, x, 0); // i think 0 is fine here.
            }
            x = 0;
            foreach (var thisColumn in columnSource)
            {
                x += 1;
                var thisLabel = CreateLabel(thisColumn.ToString());
                GridHelper.AddControlToGrid(_mainGrid, thisLabel, 0, x);
            }
            if (veryFirstHeader != "")
            {
                var thisLabel = CreateLabel(veryFirstHeader);
                GridHelper.AddControlToGrid(_mainGrid, thisLabel, 0, 0); // this has a header for both.  to support bowling dice game.
            }
        }
        protected bool CanHeadersBold = true;
        protected float HeaderFontSize = 14;
        protected float LeftRightMargins = 0;
        protected IBoardCollection<S>? PieceList;
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
        //this may be iffy too now.

        public void UpdateControls(IBoardCollection<S> itemsSource)
        {
            _mainGrid.Children.Clear();
            CreateControls(itemsSource); //try this.  hopefully this simple.
        }

        //the create controls and update controls could be iffy.


        public void CreateControls(IBoardCollection<S> itemsSource) // has to be at least this view model
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
                                GridHelper.AddControlToGrid(_mainGrid, thisControl, x - 1, y - 1);
                            else
                                GridHelper.AddControlToGrid(_mainGrid, thisControl, x, y);// because of the headers and footers.
                            z += 1; // 0 based
                        }
                    }
                }
            }
        }
        protected SKPoint GetLocation(object thisItem)
        {
            foreach (var thisFirst in _mainGrid.Children)
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
        protected virtual bool CanAddControl(IBoardCollection<S> itemsSource, int row, int column) => true; //defaults to true.

        //its possible this has to manually be done (?)

        protected Binding GetColorBinding(string colorPath)
        {
            Binding thisBind = new Binding(colorPath);
            Converters.BasicColorConverter ThisC = new Converters.BasicColorConverter();
            thisBind.Converter = ThisC;
            return thisBind;
        }


        //looks like there is now a serious problem.
        //because there is the source part of it.
        //when finding the control, hopefully it knows what view model it goes it.
        

        //there are 2 iffy parts:
        //1.  source
        //2.  being able to make it work for all that inherit from it (assignablefrom).


        //no need for command binding because we do automatically now.

        //protected Binding GetCommandBinding(string commandPath)
        //{
        //    Binding thisBind = new Binding(commandPath);
        //    thisBind.Source = GameModel; // i think
        //    return thisBind;
        //}
        protected virtual void StartInit() { }
        protected Grid ParentGrid;
        protected void LoadGridColumnRows() // may not have to override because bowling dice game is using a different one that behaves a little differently.
        {
            GridHelper.AddAutoRows(_mainGrid, PieceList!.GetTotalRows());
            GridHelper.AddAutoColumns(_mainGrid, PieceList.GetTotalColumns());
        }
        public BasicGameBoard()
        {
            ParentGrid = new Grid();
            _mainGrid = new Grid();
            ParentGrid.Children.Add(_mainGrid);
            StartInit();
            //GameModel = Resolve<IBasicGameVM>();
            Content = ParentGrid;
        }

    }
}
