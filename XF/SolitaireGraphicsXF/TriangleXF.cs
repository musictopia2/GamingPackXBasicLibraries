using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MiscClasses;
using BaseSolitaireClassesCP.TriangleClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SolitaireGraphicsXF
{
    public class TriangleXF : ContentView, ISpecialSolitaireReposition
    {
        private TriangleViewModel? _thisMod;
        private CustomBasicCollection<SolitaireCard>? _cardList; //this was one that could not use the dictionary because of how its done.
        private AbsoluteLayout? _thisCanvas;
        SKSize _size;
        private void PopulateCards()
        {
            _thisCanvas!.Children.Clear();
            _thisMod!.CardList.ForEach(thisCard =>
            {
                var thisGrapics = GetGraphics(thisCard);
                _thisCanvas.Children.Add(thisGrapics);
            });
            if (_thisCanvas.Children.Count > 0)
                PrivatePositions();
        }
        private DeckOfCardsXF<SolitaireCard> FindControl(SolitaireCard thisCard)
        {
            foreach (var firstControl in _thisCanvas!.Children)
            {
                var thisGraphics = firstControl as DeckOfCardsXF<SolitaireCard>;
                if (thisGraphics!.BindingContext.Equals(thisCard))
                    return thisGraphics;
            }
            throw new BasicBlankException("UI Not Found");
        }
        private Binding GetCommandBinding(string path)
        {
            Binding output = new Binding(path);
            output.Source = _thisMod;
            return output;
        }
        private DeckOfCardsXF<SolitaireCard> GetGraphics(SolitaireCard thisCard)
        {
            DeckOfCardsXF<SolitaireCard> output = new DeckOfCardsXF<SolitaireCard>();
            output.SendSize(ts.TagUsed, thisCard);
            output.CommandParameter = thisCard;
            var binding = GetCommandBinding(nameof(TriangleViewModel.CardCommand));
            output.SetBinding(DeckOfCardsXF<SolitaireCard>.CommandProperty, binding);
            return output; //hopefully this is enough (?)
        }
        public void Init(TriangleViewModel thisMod)
        {
            SolitaireCard card = new SolitaireCard();
            GamePackageDIContainer tt = Resolve<GamePackageDIContainer>();
            IProportionImage pp = tt.Resolve<IProportionImage>(ts.TagUsed);
            _size = card.DefaultSize.GetSizeUsed(pp.Proportion);
            _thisMod = thisMod;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CollectionChange;
            _thisCanvas = new AbsoluteLayout();
            _thisMod.PositionUI = this;
            PopulateCards();
            Content = _thisCanvas;
        }
        private void CollectionChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateCards();
        }
        private void PrivatePositions()
        {
            if (_cardList == null)
                return;
            _cardList.ForEach(thisCard =>
            {
                var thisGraphics = FindControl(thisCard);
                var thisRect = new Rectangle(thisCard.Location.X, thisCard.Location.Y, _size.Width, _size.Height);
                AbsoluteLayout.SetLayoutBounds(thisGraphics, thisRect);
            });
        }
        void ISpecialSolitaireReposition.RepositionCardsOnUI() //hopefully this simple.
        {
            PrivatePositions();
        }
    }
}