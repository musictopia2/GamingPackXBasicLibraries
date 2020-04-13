using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BasicGamingUIXFLibrary.BasicControls.Misc
{
    public class PlayerBoardXF<TR> : ContentView //decided to be a regular deck of cards for this now.
        where TR : RegularTrickCard, new()
    {
        private PlayerBoardObservable<TR>? _thisMod;
        private DeckObservableDict<TR>? _cardList;
        private Grid? _thisGrid;
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        public void LoadList(PlayerBoardObservable<TR> thisMod)
        {
            _thisMod = thisMod;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CardList_CollectionChanged;
            if (thisMod.Game == PlayerBoardObservable<TR>.EnumGameList.None)
                throw new BasicBlankException("Must choose Skuck or Horseshoes");
            if (_cardList.Count == 0)
                throw new BasicBlankException("Must have cardlist already");
            _thisGrid = new Grid();
            GamePackageDIContainer thisC = Resolve<GamePackageDIContainer>();
            IProportionImage thisImage = thisC.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = _cardList.First().DefaultSize;
            SKSize usedSize = thisSize.GetSizeUsed(thisImage.Proportion);
            var pixels = usedSize.Height / 2;
            int x;
            var loopTo = _cardList.Count;
            for (x = 1; x <= loopTo; x++)
                GridHelper.AddPixelRow(_thisGrid, (int)pixels);
            GridHelper.AddPixelRow(_thisGrid, (int)pixels);
            pixels = usedSize.Width + 6;
            for (x = 1; x <= 4; x++)
                GridHelper.AddPixelColumn(_thisGrid, (int)pixels);
            PopulateControls();
            Content = _thisGrid;
        }

        private void PopulateControls()
        {
            _thisGrid!.Children.Clear();
            foreach (var thisCard in _cardList!)
            {
                DeckOfCardsXF<TR> thisGraphics = new DeckOfCardsXF<TR>();
                thisGraphics.HorizontalOptions = LayoutOptions.Start;
                thisGraphics.VerticalOptions = LayoutOptions.Start;
                thisGraphics.Margin = new Thickness(0, 0, 6, 0);
                thisGraphics.SendSize(ts.TagUsed, thisCard);
                var thisBind = GetCommandBinding(nameof(PlayerBoardObservable<TR>.CardCommand));
                thisGraphics.SetBinding(DeckOfCardsXF<TR>.CommandProperty, thisBind);
                thisGraphics.CommandParameter = thisCard;
                var (Row, Column) = _thisMod!.GetRowColumnData(thisCard);
                GridHelper.AddControlToGrid(_thisGrid, thisGraphics, Row - 1, Column - 1);
                Grid.SetRowSpan(thisGraphics, 2);
            }
        }
        private void CardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                PopulateControls();
            else
                throw new BasicBlankException("Unsure");
        }
    }
}