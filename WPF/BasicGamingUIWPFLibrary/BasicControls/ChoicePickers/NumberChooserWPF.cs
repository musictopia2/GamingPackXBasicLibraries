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
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using System.Collections.Specialized;

namespace BasicGamingUIWPFLibrary.BasicControls.ChoicePickers
{
    public class NumberChooserWPF : UserControl
    {
        private NumberPicker? _thisMod;
        private CustomBasicCollection<NumberPieceCP>? _numberList;
        private Grid? _thisGrid;
        public int Columns { get; set; } //no need for binding support for this one
        public int Rows { get; set; } = 1; //defaults to one row
        private IWidthHeight? _graphicsSize;
        private NumberPieceWPF? FindControl(NumberPieceCP thisPiece)
        {
            foreach (var thisCon in _thisGrid!.Children)
            {
                var Deck = (NumberPieceWPF)thisCon!;
                if (Deck.DataContext.Equals(thisPiece) == true)
                    return Deck;
            }
            return null;
        }
        private void PieceBindings(NumberPieceWPF thisGraphics, NumberPieceCP thisPiece)
        {
            thisGraphics.Height = _graphicsSize!.GetWidthHeight;
            thisGraphics.Width = _graphicsSize.GetWidthHeight;
            thisGraphics.Visibility = Visibility.Visible; // i think needs to manually be set.
            var thisBind = GetCommandBinding(nameof(NumberPicker.NumberPickedCommand)); //okay this is old fashioned this time.
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.Margin = new Thickness(5, 5, 5, 5);
            thisGraphics.DataContext = thisPiece;
            thisGraphics.SetBinding(NumberPieceWPF.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(IsEnabledProperty, new Binding(nameof(BaseGraphicsCP.IsEnabled)));
            thisGraphics.SetBinding(NumberPieceWPF.NumberValueProperty, new Binding(nameof(NumberPieceCP.NumberValue)));
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
                NumberPieceWPF thisGraphics = new NumberPieceWPF();
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
            _numberList = mod.NumberList;
            Margin = new Thickness(10, 10, 10, 10);
            _numberList.CollectionChanged += NumberList_CollectionChanged;
            //_thisMod.PropertyChanged += ThisMod_PropertyChanged;
            _thisGrid = new Grid();
            _graphicsSize = Resolve<IWidthHeight>(); //you do have to register that one now.
            _thisGrid = new Grid();
            AddAutoColumns(_thisGrid, 15);
            AddAutoRows(_thisGrid, 15);
            PopulateList();
            Content = _thisGrid;
        }
        //private void ThisMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(NumberPicker.Visible))
        //    {
        //        if (_thisMod!.Visible == true)
        //            Visibility = Visibility.Visible;
        //        else
        //            Visibility = Visibility.Collapsed;// do this instead of the bindings.  since we know what type of viewmodel has to be used here.
        //    }
        //}
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
    }
}
