using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.TriangleClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows.Controls;
using System.Windows.Data;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses
{
    public class TriangleWPF : UserControl, ISpecialSolitaireReposition
    {
        private TriangleObservable? _thisMod;
        private CustomBasicCollection<SolitaireCard>? _cardList; //this was one that could not use the dictionary because of how its done.
        private Canvas? _thisCanvas;
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
        private DeckOfCardsWPF<SolitaireCard> FindControl(SolitaireCard thisCard)
        {
            foreach (var firstControl in _thisCanvas!.Children)
            {
                var thisGraphics = firstControl as DeckOfCardsWPF<SolitaireCard>;
                if (thisGraphics!.DataContext.Equals(thisCard))
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
        private DeckOfCardsWPF<SolitaireCard> GetGraphics(SolitaireCard thisCard)
        {
            DeckOfCardsWPF<SolitaireCard> output = new DeckOfCardsWPF<SolitaireCard>();
            output.SendSize(ts.TagUsed, thisCard);
            output.CommandParameter = thisCard;
            var binding = GetCommandBinding(nameof(TriangleObservable.CardCommand));
            output.SetBinding(GraphicsCommand.CommandProperty, binding);
            return output; //hopefully this is enough (?)
        }
        public void Init(TriangleObservable thisMod)
        {
            _thisMod = thisMod;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CollectionChange;
            _thisCanvas = new Canvas();
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
                Canvas.SetLeft(thisGraphics, thisCard.Location.X);
                Canvas.SetTop(thisGraphics, thisCard.Location.Y);
            });
        }
        void ISpecialSolitaireReposition.RepositionCardsOnUI() //hopefully this simple.
        {
            PrivatePositions();
        }
    }
}
