using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.Dominos
{
    public class DominosBoneYardClass<D> : ScatteringPiecesViewModel<D, DominosBasicShuffler<D>>
        where D : IDominoInfo, new()
    {
        private readonly IDominoGamesVM<D> _thisMod;
        public DominosBoneYardClass(IDominoGamesVM<D> thisMod) : base(thisMod)
        {
            ProtectedText = "Bone Yard";
            _thisMod = thisMod;
            ObjectList = thisMod.MainContainer!.Resolve<DominosBasicShuffler<D>>(); //i think
        }
        protected override async Task ClickedBoardAsync()
        {
            int deck = DrawPiece();
            await _thisMod.DrewDominoAsync(RemainingList.GetSpecificItem(deck));
        }
        public D FindDoubleDomino(int whichOne)
        {
            var output = RemainingList.Single(Items => Items.FirstNum == whichOne && Items.SecondNum == whichOne);
            RemoveDomino(output);
            output.IsUnknown = false;
            return output;
        }
        public void RemoveDomino(D domino)
        {
            RemoveSinglePiece(domino.Deck);
        }
        public DeckObservableDict<D> FirstDraw(int howMany)
        {
            GetFirstPieces(howMany, out DeckObservableDict<D> output);
            if (output.Count == 0)
                throw new BasicBlankException("Cannot draw 0 dominos to begin with");
            return output;
        }
        public void EmptyBones()
        {
            EmptyBoard();
        }
        public D DrawDomino()
        {
            int Decks = DrawPiece();
            return RemainingList.GetSpecificItem(Decks);
        }
        public bool HasBone()
        {
            return HasPieces();
        }
        protected override async Task ClickedPieceAsync(int deck)
        {
            await _thisMod.DrewDominoAsync(RemainingList.GetSpecificItem(deck)); //i think
        }
        protected override void PrivateEnableAlways() { }
    }
}