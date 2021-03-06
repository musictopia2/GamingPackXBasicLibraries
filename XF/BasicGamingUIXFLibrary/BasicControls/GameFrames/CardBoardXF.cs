﻿using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Collections.Specialized;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.GameFrames
{
    public class CardBoardXF<CA, GC, GW> : BaseFrameXF
        where CA : IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<CA, GC>, new()
    {
        private DeckObservableDict<CA>? _objectList;
        private Grid? _mainGrid;
        private GameBoardObservable<CA>? _thisMod;
        private GW? FindControl(CA thisCard)
        {
            foreach (var thisCon in _mainGrid!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.BindingContext.Equals(thisCard) == true)
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
                    if (z + 1 <= _objectList!.Count)
                    {
                        var thisCard = _objectList[z];
                        var thisGraphics = new GW();
                        CardBindings(thisGraphics, thisCard);
                        GridHelper.AddControlToGrid(_mainGrid, thisGraphics, x - 1, y - 1);
                        z += 1;
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
            var thisBind = GetCommandBinding(nameof(GameBoardObservable<CA>.ObjectCommand));
            thisDeck.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisDeck.CommandParameter = thisCard;
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
        public void LoadList(GameBoardObservable<CA> mod, string tagUsed)
        {
            _tagUsed = tagUsed;
            _thisMod = mod;
            BindingContext = _thisMod;
            _objectList = _thisMod.ObjectList;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            Grid firstGrid = new Grid();
            if (_thisMod.HasFrame == true)
                firstGrid.Children.Add(ThisDraw);
            _mainGrid = new Grid();
            _mainGrid.ColumnSpacing = 0;
            _mainGrid.RowSpacing = 0;
            firstGrid.Children.Add(_mainGrid);
            var firstCard = new CA(); // for size
            if (firstCard.DefaultSize.Height == 0 || firstCard.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height must be greater than 0 when initializging hand wpf");
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisI = thisR.Resolve<IProportionImage>(tagUsed);
            _sizeUsed = firstCard.DefaultSize.GetSizeUsed(thisI.Proportion);
            SetBinding(TextProperty, new Binding(nameof(GameBoardObservable<CA>.Text)));
            SetBinding(IsEnabledProperty, new Binding(nameof(GameBoardObservable<CA>.IsEnabled)));
            SetBinding(IsVisibleProperty, new Binding(nameof(GameBoardObservable<CA>.Visible)));
            SetUpGrid();
            var tempWidth = _sizeUsed.Width * _thisMod.Columns;
            var tempHeight = _sizeUsed.Height * _thisMod.Rows;
            if (_thisMod.HasFrame == true)
            {
                SetUpMarginsOnParentControl(_mainGrid);
            }
            float tops = (float)_mainGrid.Margin.Top + (float)_mainGrid.Margin.Bottom;
            float lefts = (float)_mainGrid.Margin.Left + (float)_mainGrid.Margin.Right;
            HeightRequest = tempHeight + tops;
            WidthRequest = tempWidth + lefts;
            PopulateList();
            Content = firstGrid;
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                PopulateList();
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count > 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                var oldCard = (CA)e.OldItems[0]!;
                var thisGraphics = FindControl(oldCard);
                thisGraphics!.BindingContext = e.NewItems[0];
                thisGraphics.CommandParameter = e.NewItems[0]!;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                return; //decided to ignore for now on for remove.  that seemed to work well on desktop.  hopefully works well on mobile too.
            else if (e.Action == NotifyCollectionChangedAction.Add) //this was needed to support pyramid solitaire
            {
                if (e.NewItems.Count != 1)
                    throw new BasicBlankException("Can only handle one item being added");
                GW thisDeck = new GW();
                CardBindings(thisDeck, (CA)e.NewItems[0]!);
                if (_thisMod!.Rows > 1)
                    throw new BasicBlankException("Card can only be added if there are more columns, but not more rows.  If I need rows too, then rethink");
                int column = _objectList!.Count - 1;
                GridHelper.AddControlToGrid(_mainGrid!, thisDeck, 0, column);
            }
            else
                throw new BasicBlankException("Not sure how to handle the action " + e.Action.ToString());
        }
    }
}