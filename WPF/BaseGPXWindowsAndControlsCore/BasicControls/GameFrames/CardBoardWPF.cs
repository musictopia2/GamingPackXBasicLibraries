using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameFrames
{
    public class CardBoardWPF<CA, GC, GW> : BaseFrameWPF
            where CA : IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<CA, GC>, new()
    {
        private DeckObservableDict<CA>? objectList;
        private Grid? _mainGrid;
        private GameBoardViewModel<CA>? _thisMod;
        private GW? FindControl(CA thisCard)
        {
            foreach (var thisCon in _mainGrid!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.DataContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        private string _tagUsed = "";
        private void PopulateList()
        {
            int x;
            int y;
            int z = 0;
            _mainGrid!.Children.Clear(); // clear and redo
            var loopTo = _thisMod!.Rows;
            for (x = 1; x <= loopTo; x++)
            {
                var loopTo1 = _thisMod.Columns;
                for (y = 1; y <= loopTo1; y++)
                {
                    if (z + 1 <= objectList!.Count)
                    {
                        var thisCard = objectList[z];
                        var thisGraphics = new GW();
                        CardBindings(thisGraphics, thisCard);
                        GridHelper.AddControlToGrid(_mainGrid, thisGraphics, x - 1, y - 1);
                        z += 1;
                    }
                }
            }
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void CardBindings(GW thisDeck, CA thisCard)
        {
            thisDeck.SendSize(_tagUsed, thisCard);
            var thisBind = GetCommandBinding(nameof(GameBoardViewModel<CA>.ObjectCommand));
            thisDeck.SetBinding(BaseDeckGraphicsWPF<CA, GC>.CommandProperty, thisBind);
            thisDeck.CommandParameter = thisCard;
        }
        public void UpdateList(GameBoardViewModel<CA> mod)
        {
            _thisMod = mod;
            DataContext = null;
            DataContext = _thisMod;
            objectList!.CollectionChanged -= ObjectList_CollectionChanged;
            objectList = mod.ObjectList;
            _mainGrid!.ColumnDefinitions.Clear();
            _mainGrid.RowDefinitions.Clear();
            SetUpGrid();
            objectList.CollectionChanged += ObjectList_CollectionChanged;
            PopulateList(); //i think.
        }
        private void SetUpGrid()
        {
            int x;
            var loopTo = _thisMod!.Columns;
            for (x = 1; x <= loopTo; x++)
                GridHelper.AddPixelColumn(_mainGrid!, (int)_sizeUsed.Width);
            var loopTo1 = _thisMod.Rows;
            for (x = 1; x <= loopTo1; x++)
                GridHelper.AddPixelRow(_mainGrid!, (int)_sizeUsed.Height);
        }
        private SKSize _sizeUsed;
        public void LoadList(GameBoardViewModel<CA> mod, string tagUsed)
        {
            _tagUsed = tagUsed;
            _thisMod = mod;
            DataContext = _thisMod;
            objectList = _thisMod.ObjectList;
            objectList.CollectionChanged += ObjectList_CollectionChanged;
            Grid firstGrid = new Grid();
            if (_thisMod.HasFrame == true)
                firstGrid.Children.Add(ThisDraw);
            _mainGrid = new Grid();
            firstGrid.Children.Add(_mainGrid);
            var firstCard = new CA(); // for size
            if (firstCard.DefaultSize.Height == 0 || firstCard.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height must be greater than 0 when initializging hand wpf");
            IGamePackageResolver thisR = (IGamePackageResolver)cons;
            IProportionImage thisI = thisR.Resolve<IProportionImage>(tagUsed);
            _sizeUsed = firstCard.DefaultSize.GetSizeUsed(thisI.Proportion);
            SetUpGrid();
            PopulateList();
            SetBinding(TextProperty, nameof(GameBoardViewModel<CA>.Text));
            SetBinding(IsEnabledProperty, nameof(GameBoardViewModel<CA>.IsEnabled));
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(GameBoardViewModel<CA>.Visible)));
            if (_thisMod.HasFrame == true)
            {
                var thisRect = ThisFrame.GetControlArea();
                _mainGrid.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            }
            Content = firstGrid;
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PopulateList();
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count > 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                var oldCard = (CA)e.OldItems[0]!;
                var thisGraphics = FindControl(oldCard);
                thisGraphics!.DataContext = e.NewItems[0];
                thisGraphics.CommandParameter = e.NewItems[0]!;
                thisGraphics.RepaintManually();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                return;
            }
            else if (e.Action == NotifyCollectionChangedAction.Add) //this was needed to support pyramid solitaire
            {
                if (e.NewItems.Count != 1)
                    throw new BasicBlankException("Can only handle one item being added");
                GW thisDeck = new GW();
                CardBindings(thisDeck, (CA)e.NewItems[0]!);
                if (_thisMod!.Rows > 1)
                    throw new BasicBlankException("Card can only be added if there are more columns, but not more rows.  If I need rows too, then rethink");
                int column = objectList!.Count - 1;
                GridHelper.AddControlToGrid(_mainGrid!, thisDeck, 0, column);
            }
            else
            {
                throw new BasicBlankException("Not sure how to handle the action " + e.Action.ToString());
            }
        }
    }
}