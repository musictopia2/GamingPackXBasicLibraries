using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;

namespace BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers
{
    public class NumberChooserXF : ContentView
    {
        private NumberPicker? _thisMod;
        private CustomBasicCollection<NumberPieceCP>? _numberList;
        private Grid? _thisGrid;
        public int Columns { get; set; } //no need for binding support for this one
        public int Rows { get; set; } = 1; //defaults to one row
        private IWidthHeight? _graphicsSize;
        private NumberPieceXF? FindControl(NumberPieceCP thisPiece)
        {
            foreach (var thisCon in _thisGrid!.Children)
            {
                var Deck = (NumberPieceXF)thisCon!;
                if (Deck.BindingContext.Equals(thisPiece) == true)
                    return Deck;
            }
            return null;
        }
        public int TotalRows { get; set; } = 0;
        private void PieceBindings(NumberPieceXF thisGraphics, NumberPieceCP thisPiece)
        {
            thisGraphics.HeightRequest = _graphicsSize!.GetWidthHeight;
            thisGraphics.WidthRequest = _graphicsSize.GetWidthHeight;
            thisGraphics.IsVisible = true; // i think needs to manually be set.
            var thisBind = GetCommandBinding(nameof(NumberPicker.NumberPickedCommand));
            thisGraphics.SetBinding(NumberPieceXF.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.BindingContext = thisPiece;
            thisGraphics.SetBinding(NumberPieceXF.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(IsEnabledProperty, new Binding(nameof(BaseGraphicsCP.IsEnabled)));
            thisGraphics.SetBinding(NumberPieceXF.NumberValueProperty, new Binding(nameof(NumberPieceCP.NumberValue)));
            thisGraphics.SendPiece(thisPiece);
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void PopulateList()
        {
            _thisGrid!.Children.Clear();
            int x = default;
            int c;
            int r;
            c = 0;
            r = 0;
            foreach (var thisPiece in _numberList!)
            {
                NumberPieceXF thisGraphics = new NumberPieceXF();
                PieceBindings(thisGraphics, thisPiece);
                if (Columns == 0 && Rows == 1)
                    AddControlToGrid(_thisGrid, thisGraphics, 0, x);
                else if (Columns == 1 && Rows == 1)
                    AddControlToGrid(_thisGrid, thisGraphics, x, 0);
                else if (Columns > 1)
                {
                    AddControlToGrid(_thisGrid, thisGraphics, r, c);
                    c += 1;
                    if (c >= Columns)
                    {
                        c = 0;
                        r += 1;
                    }
                }
                else
                {
                    // rows
                    AddControlToGrid(_thisGrid, thisGraphics, r, c);
                    r += 1;
                    if (r >= Rows)
                    {
                        r = 0;
                        c += 1;
                    }
                }
                if (r > 14 || c > 14)
                    throw new BasicBlankException("Rethinking is now required.");
                x += 1;
            }
        }
        public void LoadLists(NumberPicker mod)
        {
            _thisMod = mod;
            IsVisible = _thisMod.Visible; //i think
            _numberList = mod.NumberList;
            Margin = new Thickness(5, 5, 5, 5);
            _numberList.CollectionChanged += NumberList_CollectionChanged;
            _thisMod.PropertyChanged += ThisMod_PropertyChanged;
            _thisGrid = new Grid();
            _thisGrid.HorizontalOptions = LayoutOptions.Start;
            _thisGrid.VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            _thisGrid.RowSpacing = 0;
            _thisGrid.ColumnSpacing = 0;
            _graphicsSize = Resolve<IWidthHeight>(); //you do have to register that one now.
            _thisGrid = new Grid();
            AddAutoColumns(_thisGrid, 15);
            if (TotalRows > 0)
            {
                HeightRequest = _graphicsSize.GetWidthHeight * TotalRows;
                TotalRows.Times(x => AddPixelRow(_thisGrid, _graphicsSize.GetWidthHeight));
                _thisGrid.ColumnSpacing = 5;
            }
            else
                AddAutoRows(_thisGrid, 15);
            PopulateList();
            Content = _thisGrid;
        }
        private void ThisMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NumberPicker.Visible))
            {
                IsVisible = _thisMod!.Visible;
            }
        }
        private void NumberList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PopulateList();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var thisPiece = (NumberPieceCP)thisItem!;
                    var thisGraphics = FindControl(thisPiece)!;
                    _thisGrid!.Children.Remove(thisGraphics); // should remove that domino.
                }
                return;
            }
            throw new BasicBlankException("Problem.  Needs to know what to do now"); // iffy.
        }
        public NumberChooserXF()
        {
            IsVisible = false;
        }
    }
}