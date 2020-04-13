using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.ChoicePickers
{
    public class EnumPickerXF<GC, GW, E> : ContentView
                where GC : BaseGraphicsCP, IEnumPiece<E>, new()
                where GW : BaseGraphicsXF<GC>, new()
                where E : struct, Enum
    {
        private SimpleEnumPickerVM<E, GC>? _thisMod;
        private CustomBasicCollection<GC>? _itemList;
        private Grid? _thisGrid;
        public int Rows { get; set; } = 1;
        public int Columns { get; set; }
        public int Spacing { get; set; } = -1;
        public float GraphicsHeight { get; set; }
        public float GraphicsWidth { get; set; } //not sure about spacing (?)
        private GW? FindControl(GC thisPiece)
        {
            foreach (var thisCon in _thisGrid!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.BindingContext.Equals(thisPiece) == true)
                    return deck;
            }
            return null;
        }
        private void PieceBindings(GW thisGraphics, GC thisPiece)
        {
            thisGraphics.HeightRequest = GraphicsHeight;
            thisGraphics.WidthRequest = GraphicsWidth;
            thisGraphics.IsVisible = true; // i think needs to manually be set.
            var thisBind = GetCommandBinding(nameof(SimpleEnumPickerVM<E, GC>.EnumChosenCommand));
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.Margin = new Thickness(0, 0, 10, 0); //decided to risk with 10 instead of 5.
            thisGraphics.BindingContext = thisPiece;
            thisGraphics.SetBinding(BaseGraphicsXF<GC>.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(BaseGraphicsXF<GC>.NeedsHighLightingProperty, new Binding(nameof(BaseGraphicsCP.NeedsHighlighting)));
            thisGraphics.SetBinding(IsEnabledProperty, new Binding(nameof(BaseGraphicsCP.IsEnabled)));
            ExtraPieceData(ref thisPiece);
            thisGraphics.SendPiece(thisPiece);
        }
        protected virtual void ExtraPieceData(ref GC thisPiece) { }
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
            foreach (var thisPiece in _itemList!)
            {
                GW ThisGraphics = new GW();
                PieceBindings(ThisGraphics, thisPiece);
                if (Columns == 0 && Rows == 1)
                    AddControlToGrid(_thisGrid, ThisGraphics, 0, x);
                else if (Columns == 1 && Rows == 1)
                    AddControlToGrid(_thisGrid, ThisGraphics, x, 0);
                else if (Columns > 1)
                {
                    AddControlToGrid(_thisGrid, ThisGraphics, r, c);
                    c += 1;
                    if (c >= Columns)
                    {
                        c = 0;
                        r += 1;
                    }
                }
                else
                {
                    AddControlToGrid(_thisGrid, ThisGraphics, r, c);
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
        public void LoadLists(SimpleEnumPickerVM<E, GC> mod)
        {
            _thisMod = mod;
            _itemList = mod.ItemList;
            Margin = new Thickness(5, 5, 5, 5);
            _itemList.CollectionChanged += ItemList_CollectionChanged;
            _thisGrid = new Grid();
            _thisGrid.HorizontalOptions = LayoutOptions.Start;
            _thisGrid.VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            if (GraphicsHeight == 0 || GraphicsWidth == 0)
            {
                IEnumPickerSize picks = Resolve<IEnumPickerSize>();
                if (Columns > 1 || Rows > 1)
                {
                    GraphicsHeight = picks.SmallGraphicsWidthHeight;
                    GraphicsWidth = picks.SmallGraphicsWidthHeight;
                }
                else
                {
                    GraphicsHeight = picks.NormalGraphicsWidthHeight;
                    GraphicsWidth = picks.NormalGraphicsWidthHeight;
                }
            }
            _thisGrid = new Grid();
            if (Spacing > -1)
            {
                _thisGrid.RowSpacing = Spacing;
                _thisGrid.ColumnSpacing = Spacing;
            }
            AddAutoColumns(_thisGrid, 15);
            AddAutoRows(_thisGrid, 15);
            PopulateList();
            Content = _thisGrid;
        }
        
        public void Dispose()
        {
            foreach (var cc in _thisGrid!.Children)
            {
                if (cc is GW gg)
                {
                    gg.Dispose();
                }
            }
        }
        private void ItemList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    var thisPiece = (GC)thisItem!;
                    var thisGraphics = FindControl(thisPiece!);
                    _thisGrid!.Children.Remove(thisGraphics); // should remove that domino.
                }
                return;
            }
            throw new BasicBlankException("Problem.  Needs to know what to do now");
        }
        //public EnumPickerXF()
        //{
        //    IsVisible = false; //has to be proven true
        //}
    }
}