using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Collections.Specialized;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace BasicGamingUIXFLibrary.BasicControls.ChoicePickers
{
    public class ListChooserXF : ContentView
    {
        private IListViewPicker? _thisMod;
        private CustomBasicCollection<ListViewPieceCP>? _textList;
        public int ItemHeight { get; set; } = 25; // decided to try at 40.  can adjust as needed.
        public int ItemWidth { get; set; } = 300; //if 300 is not enough for default, can adjust.
        private StackLayout? _thisStack;
        private Grid? _thisGrid;
        public StackOrientation Orientation { get; set; } = StackOrientation.Vertical;
        public int TotalColumns { get; set; }
        private void PieceBindings(ListPieceXF thisGraphics, ListViewPieceCP thisPiece)
        {
            thisGraphics.WidthRequest = ItemWidth;
            thisGraphics.HeightRequest = ItemHeight; // lets set to 20.
            thisGraphics.IsVisible = true;
            var thisBind = GetCommandBinding(nameof(ListViewPicker.ItemSelectedCommand));
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.Margin = new Thickness(5, 0, 5, 5);
            thisGraphics.BindingContext = thisPiece;
            thisGraphics.SetBinding(ListPieceXF.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(IsEnabledProperty, new Binding(nameof(BaseGraphicsCP.IsEnabled)));
            thisGraphics.SetBinding(ListPieceXF.TextProperty, new Binding(nameof(ListViewPieceCP.DisplayText)));
            thisGraphics.SetBinding(ListPieceXF.IndexProperty, new Binding(nameof(ListViewPieceCP.Index)));
            thisGraphics.SendPiece(thisPiece);
        }
        private void PopulateList()
        {
            if (TotalColumns == 0)
                _thisStack!.Children.Clear();
            else
                _thisGrid!.Children.Clear();
            int row = 0;
            int column = 0;
            foreach (var thisPiece in _textList!)
            {
                ListPieceXF thisGraphics = new ListPieceXF();
                PieceBindings(thisGraphics, thisPiece);
                if (TotalColumns == 0)
                    _thisStack!.Children.Add(thisGraphics);
                else
                {
                    if (row + 1 > _thisGrid!.RowDefinitions.Count)
                        AddAutoRows(_thisGrid, 1); //add one as needed.
                    AddControlToGrid(_thisGrid!, thisGraphics, row, column);
                    column++;
                    if (column + 1 > TotalColumns) //try this way.
                    {
                        column = 0;
                        row++;
                    }

                }
            }
        }
        public void LoadLists(IListViewPicker mod)
        {
            _thisMod = mod;
            _textList = _thisMod.TextList;
            _textList.CollectionChanged += TextList_CollectionChanged;
            if (TotalColumns == 0)
            {
                _thisStack = new StackLayout();
                _thisStack.Orientation = Orientation;
            }
            else
            {
                _thisGrid = new Grid();
                AddAutoRows(_thisGrid, 1); //at least one row obviously.
                AddAutoColumns(_thisGrid, TotalColumns);
            }
            Margin = new Thickness(3, 3, 3, 3);
            PopulateList();
            if (TotalColumns == 0)
                Content = _thisStack;
            else
                Content = _thisGrid;
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void TextList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateList();
        }
    }
}