using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers
{
    public class EnumPickerWPF<GC, GW, E, LI> : UserControl
                where GC : BaseGraphicsCP, IEnumPiece<E>, new()
                where GW : BaseGraphicsWPF<GC>, new()
                where E : struct, Enum
                where LI : IEnumListClass<E>, new()
    {
        private SimpleEnumPickerVM<E, GC, LI>? _thisMod;
        private CustomBasicCollection<GC>? _itemList;
        private Grid? _thisGrid;
        public int Rows { get; set; } = 1;
        public int Columns { get; set; }
        public float GraphicsHeight { get; set; } = 150;
        public float GraphicsWidth { get; set; } = 150;
        private GW? FindControl(GC thisPiece)
        {
            foreach (var thisCon in _thisGrid!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.DataContext.Equals(thisPiece) == true)
                    return deck;
            }
            return null;
        }
        private void PieceBindings(GW thisGraphics, GC thisPiece)
        {
            thisGraphics.Height = GraphicsHeight;
            thisGraphics.Width = GraphicsWidth;
            thisGraphics.Visibility = Visibility.Visible; // i think needs to manually be set.
            var thisBind = GetCommandBinding(nameof(SimpleEnumPickerVM<E, GC, LI>.EnumChosenCommand));
            thisGraphics.SetBinding(BaseGraphicsWPF<GC>.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // must be piece, not simply the color.  something else will figure out the color.
            thisGraphics.Margin = new Thickness(5, 5, 5, 5);
            thisGraphics.DataContext = thisPiece;
            thisGraphics.SetBinding(BaseGraphicsWPF<GC>.IsSelectedProperty, new Binding(nameof(BaseGraphicsCP.IsSelected))); // i think
            thisGraphics.SetBinding(BaseGraphicsWPF<GC>.NeedsHighLightingProperty, new Binding(nameof(BaseGraphicsCP.NeedsHighlighting)));
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
                GW thisGraphics = new GW();
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
        public void LoadLists(SimpleEnumPickerVM<E, GC, LI> mod)
        {
            _thisMod = mod;
            _itemList = mod.ItemList;
            Margin = new Thickness(10, 10, 10, 10);
            _itemList.CollectionChanged += ItemList_CollectionChanged;
            _thisMod.PropertyChanged += ThisMod_PropertyChanged;
            if (_thisMod.Visible == true)
                Visibility = Visibility.Visible; //try this way too.  because on games like board games, it can be visible at this point.
            _thisGrid = new Grid();
            if (Columns > 1 || Rows > 1)
            {
                GraphicsHeight = 100;
                GraphicsWidth = 100;
            }
            _thisGrid = new Grid();
            AddAutoColumns(_thisGrid, 15);
            AddAutoRows(_thisGrid, 15);
            PopulateList();
            Content = _thisGrid;
        }
        private void ThisMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SimpleEnumPickerVM<E, GC, LI>.Visible))
            {
                if (_thisMod!.Visible == true)
                {
                    Visibility = Visibility.Visible;
                }
                else
                    Visibility = Visibility.Collapsed;// do this instead of the bindings.  since we know what type of viewmodel has to be used here.
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
        public EnumPickerWPF()
        {
            Visibility = Visibility.Collapsed; //has to be proven true
        }
    }
}