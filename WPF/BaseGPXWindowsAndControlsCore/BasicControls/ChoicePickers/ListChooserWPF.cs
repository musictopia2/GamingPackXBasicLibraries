using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers
{
    public class ListChooserWPF : UserControl
    {
        private IListViewPicker? _thisMod;
        private CustomBasicCollection<ListViewPieceCP>? _textList;
        public int ItemHeight { get; set; } = 40; // decided to try at 40.  can adjust as needed.
        public int ItemWidth { get; set; } = 300; //if 300 is not enough for default, can adjust.
        private StackPanel? _thisStack;
        public Orientation Orientation { get; set; } = Orientation.Vertical; //default to vertical but could be horizontal if necessary

        //wpf can have wrap but xamarin forms do not.

        public int TotalColumns { get; set; }
        private Grid? _thisGrid;
        private void PieceBindings(ListPieceWPF thisGraphics, ListViewPieceCP thisPiece)
        {
            thisGraphics.Width = ItemWidth;
            thisGraphics.Height = ItemHeight; // lets set to 20.
            thisGraphics.Visibility = Visibility.Visible;
            var thisBind = GetCommandBinding(nameof(ListViewPicker.ItemSelectedCommand));
            thisGraphics.SetBinding(ListPieceWPF.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.Margin = new Thickness(5, 0, 5, 5);
            thisGraphics.DataContext = thisPiece;
            thisGraphics.SetBinding(ListPieceWPF.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(IsEnabledProperty, new Binding(nameof(BaseGraphicsCP.IsEnabled)));
            thisGraphics.SetBinding(ListPieceWPF.TextProperty, new Binding(nameof(ListViewPieceCP.DisplayText)));
            thisGraphics.SetBinding(ListPieceWPF.IndexProperty, new Binding(nameof(ListViewPieceCP.Index)));
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
                ListPieceWPF thisGraphics = new ListPieceWPF();
                PieceBindings(thisGraphics, thisPiece);
                if (TotalColumns == 0)
                    _thisStack!.Children.Add(thisGraphics);
                else
                {
                    if (row + 1 > _thisGrid!.RowDefinitions.Count)
                        AddAutoRows(_thisGrid, 1); //add one as needed.
                    AddControlToGrid(_thisGrid!, thisGraphics, row, column);
                    column++;
                    if (column + 1 > TotalColumns)
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
            _thisMod.PropertyChanged += ThisMod_PropertyChanged;
            if (TotalColumns == 0)
            {
                _thisStack = new StackPanel();
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
        private void ThisMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IListViewPicker.Visible))
            {
                if (_thisMod!.Visible == true)
                    Visibility = Visibility.Visible;
                else
                    Visibility = Visibility.Collapsed;// do this instead of the bindings.  since we know what type of viewmodel has to be used here.
            }
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