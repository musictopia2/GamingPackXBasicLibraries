using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System;
using System.Collections.Specialized;
using System.Linq; //sometimes i do use linq.
using System.Threading.Tasks;
namespace BasicGameFramework.DrawableListsViewModels
{
    public class SavedScatteringPieces<D> where D : IDeckObject, new()
    {
        public bool HasDrawn { get; set; }
        public bool ClearedOut { get; set; }
        public DeckObservableDict<D> RemainingList { get; set; } = new DeckObservableDict<D>();
    }
    public abstract class ScatteringPiecesViewModel<D, L> : SimpleControlViewModel where D : ILocationDeck, new()
        where L : class, IScatterList<D>, new()
    {
        protected abstract Task ClickedPieceAsync(int Deck); // deck is the card number (needs to be deck so each one can use the function to return the proper card
        protected abstract Task ClickedBoardAsync();
        private bool _mVarHasDrawn;
        private bool _clearedOut;
        protected L? ObjectList;
        protected string ProtectedText = ""; // text would already be given when doing this
        public DeckObservableDict<D> RemainingList = new DeckObservableDict<D>();
        private readonly IProportionImage _thisI;
        public ControlCommand<D> ObjectCommand { get; set; } // needs the commands
        public ControlCommand BoardCommand { get; set; }
        private bool _privateObjectClicked;
        public static string TagUsed => "scatter";
        private readonly RandomGenerator _rs;
        private readonly EventAggregator _thisE;
        public ScatteringPiecesViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            Visible = true; // the old did not have this.  will always be true here.
            _thisI = thisMod.MainContainer!.Resolve<IProportionImage>(TagUsed);
            _rs = thisMod.MainContainer.Resolve<RandomGenerator>();
            _thisE = thisMod.MainContainer.Resolve<EventAggregator>();
            ObjectCommand = new ControlCommand<D>(this, async items =>
            {
                _privateObjectClicked = true;
                await ClickedPieceAsync(items.Deck);
            }, thisMod, thisMod.CommandContainer!);
            BoardCommand = new ControlCommand(this, async items =>
            {
                if (_privateObjectClicked == true)
                {
                    _privateObjectClicked = false;
                    return;
                }
                await ClickedBoardAsync();
            }, thisMod, thisMod.CommandContainer!);
        }
        private SKSize _MaxSize;
        public SKSize MaxSize
        {
            get
            {
                return _MaxSize;
            }
            set
            {
                if (SetProperty(ref _MaxSize, value) == true)
                {
                }
            }
        }
        private string _TextToAppear = "";
        public string TextToAppear
        {
            get
            {
                return _TextToAppear;
            }
            set
            {
                if (SetProperty(ref _TextToAppear, value) == true)
                {
                }
            }
        }
        private bool _CanDraw = true;
        public bool CanDraw
        {
            get
            {
                return _CanDraw;
            }

            set
            {
                if (SetProperty(ref _CanDraw, value) == true)
                {
                }
            }
        }
        public void LoadPlayerPieces(IDeckDict<D> thisList, ref DeckObservableDict<D> hand, bool isFirst)
        {
            hand.ReplaceRange(thisList);
            if (isFirst == false)
                return;
            DeckRegularDict<D> newList = new DeckRegularDict<D>();
            foreach (var thisItem in thisList)
                newList.Add(RemainingList.GetSpecificItem(thisItem.Deck));
            RemovePieces(newList);
        }
        public void SavedGame(SavedScatteringPieces<D> save)
        {
            _mVarHasDrawn = save.HasDrawn;
            _clearedOut = save.ClearedOut;
            RemainingList.ReplaceRange(save.RemainingList);
        }
        public SavedScatteringPieces<D> SavedData()
        {
            SavedScatteringPieces<D> output = new SavedScatteringPieces<D>();
            output.HasDrawn = _mVarHasDrawn;
            output.ClearedOut = _clearedOut;
            output.RemainingList = RemainingList.ToObservableDeckDict();
            return output;
        }
        protected int DrawPiece()
        {
            return RemainingList.GetRandomItem().Deck;
        }
        protected void RemoveSinglePiece(int deck)
        {
            var thisPiece = RemainingList.GetSpecificItem(deck);
            RemainingList.RemoveSpecificItem(thisPiece);
            _mVarHasDrawn = true;
        }
        protected void RemovePieces(IDeckDict<D> thisList)
        {
            RemainingList.RemoveGivenList(thisList, NotifyCollectionChangedAction.Remove);
        }
        protected void GetFirstPieces(int howMany, out DeckObservableDict<D> thisList)
        {
            if (ObjectList!.Count == 0)
                throw new Exception("There must be at least one piece in order to get the first pieces");
            thisList = new DeckObservableDict<D>(); //i think.
            if (RemainingList.Count < howMany)
                throw new Exception("There are only " + RemainingList.Count + " but was trying to get " + howMany + " pieces");
            ICustomBasicList<D> newList;
            newList = RemainingList.GetRandomList(true, howMany);
            if (RemainingList.Count == 0)
                throw new Exception("Can't have 0 pieces remaining after getting random list");
            foreach (var thisCard in newList)
                thisCard.IsUnknown = false;
            thisList.ReplaceRange(newList);
        }
        public void ScatterPieces()
        {
            int maxPointx;
            int maxPointy;
            if (MaxSize.Width == 0 || MaxSize.Height == 0)
                throw new Exception("Must specify height and width in order to scatter the pieces");
            if (RemainingList.Count == 0)
                return;// nothing to scatter anymore.
            D tempObject;
            tempObject = RemainingList.First();
            SKSize NewSize = tempObject.DefaultSize.GetSizeUsed(_thisI.Proportion);
            maxPointx = (int)MaxSize.Width - (int)NewSize.Width - 3;
            maxPointy = (int)MaxSize.Height - (int)NewSize.Height - 3;
            foreach (var thisCard in RemainingList)
            {
                var Locx = _rs.GetRandomNumber(maxPointx, 3);
                var Locy = _rs.GetRandomNumber(maxPointy, 20);

                thisCard.Location = new SKPoint(Locx, Locy); // you need location now.
            }
        }
        public void PopulateBoard() //try this way this time.
        {
            ObjectList!.ClearObjects(); //i think.
            ObjectList.ShuffleObjects();
            if (ObjectList.Count == 0)
                throw new Exception("Must have at least one piece after reshuffling");
            foreach (var thisCard in ObjectList)
                thisCard.IsUnknown = true;
            RemainingList.ReplaceRange(ObjectList);
            if (MaxSize.Width > 0 && MaxSize.Height > 0)
            {
                ScatterPieces(); // the next time, the cross platform processes has to do it.
                _thisE.Publish(new ScatteringCompletedEventModel());
            }
        }
        public void PopulateTotals()
        {
            if (ProtectedText == "")
                throw new Exception("The text must be filled out.");
            int totals;
            if (_clearedOut == true)
                totals = 0;
            else
                totals = RemainingList.Count;// try this way.
            TextToAppear = ProtectedText + " (" + totals + ")";
        }
        protected void EmptyBoard()
        {
            _clearedOut = true;
            PopulateTotals();
        }
        public void NewTurn()
        {
            _mVarHasDrawn = false;
        }
        public bool HasDrawn()
        {
            return _mVarHasDrawn;
        }
        protected override bool CanEnableFirst()
        {
            if (CanDraw == false)
                return false;
            if (_mVarHasDrawn == true)
                return false;
            if (HasPieces() == false)
                return false;
            return true;
        }
        protected bool HasPieces()
        {
            if (RemainingList.Count == 0)
                return false;
            if (_clearedOut == true)
                return false;
            return true;
        }
        protected override void EnableChange() { }
        protected override void VisibleChange() { }
    }
}