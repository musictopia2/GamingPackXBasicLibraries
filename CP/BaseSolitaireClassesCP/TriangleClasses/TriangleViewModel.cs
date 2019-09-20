using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MiscClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BaseSolitaireClassesCP.TriangleClasses
{
    public abstract class TriangleViewModel : ObservableObject
    {
        public CustomBasicCollection<SolitaireCard> CardList = new CustomBasicCollection<SolitaireCard>();
        private readonly int _maxRowsColumns;
        private bool _inPlay = false;
        private float _totalWidth;
        private float _cardWidth;
        private float _cardHeight;
        public ISpecialSolitaireReposition? PositionUI;
        public PlainCommand<SolitaireCard> CardCommand { get; set; }
        private readonly IProportionImage _thisP;
        public TriangleViewModel(ITriangleVM thisMod, int maxColumnsRows)
        {
            _maxRowsColumns = maxColumnsRows;
            _thisP = thisMod.MainContainer!.Resolve<IProportionImage>(ts.TagUsed);
            CardCommand = new PlainCommand<SolitaireCard>(async thisCard =>
            {
                await thisMod.CardClickedAsync(thisCard);
            }, thisCard =>
            {
                if (thisCard.IsEnabled == false || thisCard.Visible == false)
                    return false;
                return _inPlay;
            }, thisMod, thisMod.CommandContainer!);
            LoadBoard();
        }
        private void LoadBoard()
        {
            CardList = new CustomBasicCollection<SolitaireCard>();
            SolitaireCard tempCard = new SolitaireCard();
            SKSize tempSize = tempCard.DefaultSize.GetSizeUsed(_thisP.Proportion);
            _cardWidth = tempSize.Width;
            _cardHeight = tempSize.Height;
            _totalWidth = _maxRowsColumns * _cardWidth;
            int nums = HowManyCards;
            nums.Times(x =>
            {
                tempCard = new SolitaireCard();
                tempCard.IsEnabled = false;
                CardList.Add(tempCard);
            });
            PositionCards();
        }
        protected void ClearBoard()
        {
            PositionCards();
            CardList.ForEach(thisCard =>
            {
                thisCard.Visible = true;
                thisCard.IsEnabled = false;
            });
            _inPlay = true;
        }
        protected void RecalculateEnables()
        {
            CardList.ForEach(thisCard => thisCard.IsEnabled = false); //must be proven true.
            int firsts = CardList.Count - _maxRowsColumns + 1;
            int x;
            for (x = firsts; x <= CardList.Count; x++)
            {
                var thisCard = CardList[x - 1];
                thisCard.IsEnabled = thisCard.Visible;
            }
            int firstNum = firsts;
            int y;
            int secondNumber;
            int finalNumber;
            SolitaireCard firstCard;
            SolitaireCard secondCard;
            SolitaireCard finalCard;
            for (x = _maxRowsColumns - 1; x >= 1; x += -1)
            {
                var loopTo1 = (firstNum + x) - 1;
                for (y = firstNum; y <= loopTo1; y++)
                {
                    secondNumber = y + 1;
                    finalNumber = y - x;
                    firstCard = CardList[y - 1]; // because its 0 based
                    secondCard = CardList[secondNumber - 1];
                    finalCard = CardList[finalNumber - 1];
                    if ((firstCard.Visible == false) & (secondCard.Visible == false) & (finalCard.Visible == true))
                        finalCard.IsEnabled = true;
                }
                firstNum -= x;
            }
        }
        private int HowManyCards
        {
            get
            {
                int y = 0;
                _maxRowsColumns.Times(x => y += x);
                return y;
            }
        }
        private void PositionCards()
        {
            float firstTop = 0;
            float firstLeft = (_totalWidth - _cardWidth) / 2;
            float divHeight = _cardHeight / 2;
            float divWidth = _cardWidth / 2;
            int y = 0;
            float newLeft;
            float newTop;
            SolitaireCard thisCard;
            int oldy = 0;
            int q = 0;
            _maxRowsColumns.Times(x =>
            {
                y += x;
                oldy = y - oldy;
                newLeft = firstLeft;
                newTop = firstTop;
                oldy.Times(z =>
                {
                    q++;
                    thisCard = CardList[q - 1]; //because 0 based.
                    thisCard.Location = new SKPoint(newLeft, newTop);
                    newLeft += _cardWidth;
                });
                firstTop += divHeight;
                firstLeft -= divWidth;
                oldy = y;
            });
            if (PositionUI != null)
                PositionUI.RepositionCardsOnUI(); //this tells it to reposition now.
        }
        public SavedTriangle GetSavedTriangles()
        {
            SavedTriangle output = new SavedTriangle();
            output.InPlay = _inPlay;
            output.CardList = CardList.ToCustomBasicList();
            return output;
        }
        public virtual void LoadSavedTriangles(SavedTriangle thisT)
        {
            _inPlay = thisT.InPlay;
            CardList.ReplaceRange(thisT.CardList);
        }
    }
}