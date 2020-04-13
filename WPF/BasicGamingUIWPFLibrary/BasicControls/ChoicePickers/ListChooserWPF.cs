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
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using System.Windows;
using System.Windows.Data;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using System.ComponentModel;
using System.Collections.Specialized;
using BasicGamingUIWPFLibrary.GameGraphics.Base;

namespace BasicGamingUIWPFLibrary.BasicControls.ChoicePickers
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
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
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
            Visibility = Visibility.Visible; //could be iffy (?)
            //hopefully no need for the other part.
            //_thisMod.PropertyChanged += ThisMod_PropertyChanged;
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
        //private void ThisMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(IListViewPicker.Visible))
        //    {
        //        if (_thisMod!.Visible == true)
        //            Visibility = Visibility.Visible;
        //        else
        //            Visibility = Visibility.Collapsed;// do this instead of the bindings.  since we know what type of viewmodel has to be used here.
        //    }
        //}
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
