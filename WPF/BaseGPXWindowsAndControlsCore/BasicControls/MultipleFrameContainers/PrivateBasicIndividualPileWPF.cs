using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
namespace BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers
{
    public class PrivateBasicIndividualPileWPF<CA, GC, GW> : BaseFrameWPF //had to be public for games like captive queens
        where CA : class, IDeckObject, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsWPF<CA, GC>, new()
    {
        public BasicPileInfo<CA>? ThisPile;
        public BasicMultiplePilesCP<CA>? MainMod;
        private GW? _thisGraphics;
        private DeckObservableDict<CA>? _thisList = null;
        private void CalculateEnables()
        {
            if (MainMod!.IsEnabled == false)
                IsEnabled = false;
            else if (ThisPile!.IsEnabled == false)
                IsEnabled = false;
            else
                IsEnabled = true;
        }
        internal SKPoint CardLocation { get; set; } // this is needed for the animations part.
        private Grid? _thisGrid; // for now until i figure out why the cards are not showinng up properly.  it did show the collectionchange
        public void Init(string tagUsed) //you do need to call in the tagused.
        {
            if (ThisPile == null)
                throw new Exception("You must have sent in a pile before calling the init");
            if (MainMod == null)
                throw new Exception("You must have sent in the main multiple piles view model so its able to populate the actual card");
            ThisPile.PropertyChanged += ThisPile_PropertyChanged;
            MainMod.PropertyChanged += MainMod_PropertyChanged;
            DataContext = ThisPile;
            if (MainMod.HasFrame == true && MainMod.HasText == true)
                SetBinding(TextProperty, new Binding(nameof(BasicPileInfo<CA>.Text)));
            if (MainMod.HasText == false)
                Text = "";
            _thisGrid = new Grid();
            _thisGraphics = new GW();
            if (MainMod.Style == BasicMultiplePilesCP<CA>.EnumStyleList.HasList)
            {
                _thisList = ThisPile.ObjectList;
                _thisList.CollectionChanged += ThisList_CollectionChanged;
            }
            var tempCard = MainMod.GetLastCard(ThisPile);
            if (tempCard.DefaultSize.Height == 0 || tempCard.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height cannot be 0.  Rethink");
            _thisGraphics.SendSize(tagUsed, tempCard); //i think
            _thisGraphics.DataContext = MainMod.GetLastCard(ThisPile);
            Binding thisBind = new Binding(nameof(BasicMultiplePilesCP<CA>.PileCommand));
            thisBind.Source = MainMod; // has to be that one  still has to be this one.
            _thisGraphics.SetBinding(BaseDeckGraphicsWPF<CA, GC>.CommandProperty, thisBind);
            _thisGraphics.CommandParameter = ThisPile; // this time, the parameter is the pile, not the card
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(PileViewModel<CA>.Visible), false));
            if (MainMod.HasFrame == true && MainMod.HasText == true)
            {
                var thisRect = ThisFrame.GetControlArea();
                _thisGraphics.Margin = new System.Windows.Thickness(thisRect.Left, thisRect.Top, 3, 3); // i think everything else would be fine (well see)
                CardLocation = new SKPoint(thisRect.Left + 3, thisRect.Top + 3);
            }
            else if (MainMod.HasFrame == false)
                CardLocation = new SKPoint(0, 0);
            else
            {
                CardLocation = new SKPoint(5, 5);
                _thisGraphics.Margin = new Thickness(5, 5, 3, 3);
            }
            if (MainMod.HasFrame)
                _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(_thisGraphics);
            Content = _thisGrid;
            _thisGraphics.Init(); // this should be okay.  will do the right thing afterall
            CalculateEnables();
            _thisGraphics.DoInvalidate(); //can do this instead.
        }
        private void ThisList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var thisCard = MainMod!.GetLastCard(ThisPile!);
            _thisGraphics!.Visibility = Visibility.Visible;
            _thisGraphics.DataContext = thisCard;
        }
        private void MainMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BasicMultiplePilesCP<CA>.IsEnabled))
                CalculateEnables();
        }
        private void ThisPile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BasicPileInfo<CA>.IsEnabled))
                CalculateEnables();
            if (e.PropertyName == nameof(BasicPileInfo<CA>.IsSelected))
            {
                var ThisCard = (CA)_thisGraphics!.DataContext;
                ThisCard.IsSelected = ThisPile!.IsSelected; // i think
            }
            if (MainMod!.Style == BasicMultiplePilesCP<CA>.EnumStyleList.SingleCard)
            {
                if (e.PropertyName == nameof(BasicPileInfo<CA>.ThisObject))
                {
                    var ThisCard = MainMod.GetLastCard(ThisPile!);
                    _thisGraphics!.DataContext = ThisCard;
                }
            }
        }
    }
}