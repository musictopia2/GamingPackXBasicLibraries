using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.Dice;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    /// <summary>
    /// this time decided to not allow inheritance.  the unique stuff is being done via interfaces.
    /// </summary>
    public sealed class ScoresheetVM<D> : SimpleControlViewModel, IHandleAsync<SelectionChosenEventModel>
        where D : SimpleDice, new()
    {
        public readonly BasicYahtzeeGame<D> MainGame; //the interface may need to know about this one.
        readonly EventAggregator _thisE;
        public ControlCommand<RowInfo> RowClickedCommand { get; set; }
        private RowInfo? _privateChosen;
        private readonly IBasicGameVM _thisMod;
        private readonly IYahtzeeStyle<D> _thisI; //try this way.
        public CustomBasicList<RowInfo> RowList = new CustomBasicList<RowInfo>();
        public CustomBasicList<DiceInformation> DiceList = new CustomBasicList<DiceInformation>();
        public ScoresheetVM(IBasicGameVM thisMod, BasicYahtzeeGame<D> mainGame, EventAggregator thisE, IYahtzeeStyle<D> thisI) : base(thisMod)
        {
            MainGame = mainGame;
            _thisE = thisE;
            _thisMod = thisMod;
            _thisI = thisI;
            RowClickedCommand = new ControlCommand<RowInfo>(this, async items =>
            {
                if (mainGame.SaveRoot!.RollNumber == 1)
                {
                    await thisMod.ShowGameMessageAsync("Sorry, you have to roll at least once before choosing a category");
                    return;
                }
                _privateChosen = items;
                thisMod.CommandContainer!.ManuelFinish = true; //you have to manually do it at this point.
                if (HasWarning(items) == true)
                {
                    WarningEventModel ThisWarn = new WarningEventModel();
                    ThisWarn.Message = "Are you sure you want to mark off " + items.Description;
                    thisE.Subscribe(this); //i think
                    await thisE.PublishAsync(ThisWarn); //i think
                    return;
                }
                await ProcessMoveAsync(items);
            }, thisMod, thisMod.CommandContainer!);
        }
        public bool CanPlay(RowInfo thisRow) //decided to be public.  the interface can use it.
        {
            if (Visible == false)
                return false;
            if (IsEnabled == false)
                return false;
            if (_thisMod.CommandContainer!.IsExecuting == true)
                return false; //even this.
            if (thisRow.HasFilledIn() == true)
                return false;// because you already filled in
            return true;
        }
        private bool HasLoaded;
        public void CheckErrors(bool startLoading) //so interfaces can call this.
        {
            if (startLoading == false)
            {
                if (HasLoaded == false)
                    throw new BasicBlankException("Must load the board before doing anything else");
            }
        }
        public void LoadBoard()
        {
            CheckErrors(true);
            RowInfo thisRow;
            thisRow = new RowInfo(EnumRowEnum.Header, true);
            thisRow.RowNumber = 0;
            RowList.Add(thisRow);
            thisRow = new RowInfo(EnumRowEnum.Header, false); // 2 headers first
            thisRow.RowNumber = 0;
            CustomBasicList<string> tempList = new CustomBasicList<string>
            {
                "Aces (1's)",
                "Dueces (2's)",
                "Treys (3's)",
                "Fours",
                "Fives",
                "Sixes"
            };
            int x = 0;
            foreach (var thisItem in tempList)
            {
                x += 1;
                RowInfo NewRow = new RowInfo();
                NewRow.RowSection = EnumRowEnum.Regular;
                NewRow.Description = thisItem;
                NewRow.IsTop = true;
                NewRow.RowNumber = x;
                RowList.Add(NewRow);
            }
            thisRow.IsTop = true;
            thisRow.RowSection = EnumRowEnum.Bonus;
            thisRow.Description = "Bonus";
            x += 1;
            thisRow.RowNumber = x;
            RowList.Add(thisRow);
            x += 1;
            thisRow = new RowInfo();
            thisRow.IsTop = true;
            thisRow.RowSection = EnumRowEnum.Totals;
            thisRow.RowNumber = x;
            RowList.Add(thisRow);
            x = 0;
            tempList = _thisI.GetBottomText;
            foreach (var thisItem in tempList)
            {
                x += 1;
                RowInfo newRow = new RowInfo();
                newRow.RowSection = EnumRowEnum.Regular;
                newRow.Description = thisItem;
                newRow.IsTop = false;
                newRow.RowNumber = x;
                RowList.Add(newRow);
            }
            x += 1;
            thisRow = new RowInfo();
            thisRow.IsTop = false;
            thisRow.RowSection = EnumRowEnum.Totals;
            thisRow.RowNumber = x;
            RowList.Add(thisRow);
            HasLoaded = true;
        }
        public bool IsGameOver => RowList.Where(items => items.RowSection == EnumRowEnum.Regular)
            .All(Items => Items.HasFilledIn() == true);
        private int GetTopScore => RowList.Where(items => items.IsTop == true
            && items.PointsObtained.HasValue == true && (items.RowSection == EnumRowEnum.Regular
        || items.RowSection == EnumRowEnum.Bonus))
        .Sum(items => items.PointsObtained!.Value);
        private int GetBottomScore => RowList.Where(items => items.PointsObtained.HasValue == true && items.IsTop == false
            && items.RowSection == EnumRowEnum.Regular).Sum(items => items.PointsObtained!.Value);
        public CustomBasicList<RowInfo> GetAvailableScores => RowList.Where(items => items.RowSection == EnumRowEnum.Regular
         && items.HasFilledIn() == false).ToCustomBasicList();
        public int TotalScore => GetTopScore + GetBottomScore;
        private void PopulateTopScores()
        {
            6.Times(x =>
            {
                if (RowList[x].HasFilledIn() == false)
                {
                    RowList[x].Possible = DiceList.Where(Items => Items.Value == x).Sum(Items => Items.Value);
                    if (RowList[x].Possible == 0)
                        RowList[x].Possible = null;
                }
            });
        }
        public bool HasFullHouse()
        {
            var tempList = DiceList.GroupOrderDescending(items => items.Value);
            if (tempList.Count() != 2)
                return false;
            if (tempList.First().Count() != 3)
                return false;
            return true; //i think.  if i am wrong, rethink.
        }
        public bool HasAllFive()
        {
            int count = DiceList.MaximumDuplicates(items => items.Value);
            return count == 5;
        }
        public bool HasKind(int HowMany)
        {
            int count = DiceList.MaximumDuplicates(items => items.Value);
            return count >= HowMany;
        }
        public bool HasStraight(bool smallOnly)
        {
            var tempList = DiceList.OrderBy(items => items.Value).GroupBy(items => items.Value).ToCustomBasicList();
            if (tempList.Count != 5 && smallOnly == false)
                return false;
            if (tempList.Count < 4)
                return false;
            //focus on large first.
            if (tempList.Count == 5)
            {
                if (tempList.First().Key == 1 && tempList.Last().Key == 6 && smallOnly == false)
                    return false;
                else if (smallOnly == false)
                    return true;
                bool rets = false;
                for (int x = 1; x <= 3; x++)
                {
                    for (int y = x; y <= x + 3; y++)
                    {
                        rets = tempList.Any(Items => Items.Key == y);
                        if (rets == false)
                            break;
                    }
                    if (rets == true)
                        return true;
                }
                return false;
            }
            for (int x = 1; x <= 3; x++)
            {
                if (tempList.First().Key == x && tempList.Last().Key == x + 3)
                    return true;
            }
            return false;
        }
        public int CalculateDiceTotal => DiceList.Sum(items => items.Value);
        public RowInfo GetRowSent(int index) //done.
        {
            return RowList[index];
        }
        private async Task ProcessMoveAsync(RowInfo thisRow) //done.
        {
            if (_privateChosen == null)
                throw new BasicBlankException("Did not save selection");
            _privateChosen = null;
            if (MainGame.ThisData!.MultiPlayer == true)
            {
                int index = RowList.IndexOf(thisRow);
                await MainGame.ThisNet!.SendMoveAsync(index); //decided to be this way.
            }
            await MainGame.MakeMoveAsync(thisRow);
        }
        public bool HasWarning(RowInfo currentRow) //done i think.
        {
            if (currentRow.Possible.HasValue == false)
                return true;
            return false;
        }
        public void PopulatePossibleScores()
        {
            ClearRecent();
            ClearPossibleScores();
            DiceList = _thisI.GetDiceList();
            if (DiceList.Count != 5)
                throw new BasicBlankException("Must have 5 dice, not " + DiceList.Count);
            PopulateTopScores();
            _thisI.PopulateBottomScores(this);
        }
        private void ClearPossibleScores()
        {
            RowList.ForEach(Items => Items.ClearPossibleScores());
        }
        public void ClearRecent()
        {
            RowList.ForEach(Items => Items.IsRecent = false);
        }
        private bool Extra5OfAKind(RowInfo currentRow)
        {
            if (HasAllFive() == false)
                return false;
            if (currentRow.IsAllFive() == true)
                return false;
            if (currentRow.PointsObtained.HasValue == false)
                throw new BasicBlankException("If its 5 of a kind and no score, should have shown as allfives.");
            if (_thisI.HasExceptionFor5Kind == true)
                return true;
            if (currentRow.PointsObtained == 0)
                return false; //no exception.  means if you marked it off, then nothing period.
            return true;
        }
        public void StartTurn()
        {
            MainGame.SingleInfo!.MissNextTurn = false; //if it matters, then done here.
            ClearRecent();
        }
        private void FinishMarking(RowInfo currentRow)
        {
            RowList.Last().PointsObtained = GetBottomScore;
            RowInfo tempRow;
            if (NeedsToCalculateBonus() == false)
            {
                // figure out the totals
                tempRow = (from Items in RowList
                           where Items.IsTop == true && Items.RowSection == EnumRowEnum.Totals
                           select Items).Single();
                tempRow.PointsObtained = GetTopScore;
                if (Extra5OfAKind(currentRow) == true)
                    _thisI.Extra5OfAKind(this);
                return; // because no need to calculate bonus
            }
            tempRow = (from Items in RowList
                       where Items.RowSection == EnumRowEnum.Bonus
                       select Items).Single();
            tempRow.PointsObtained = _thisI.BonusAmount(GetTopScore);
            tempRow = (from Items in RowList
                       where Items.IsTop == true && Items.RowSection == EnumRowEnum.Totals
                       select Items).Single();
            tempRow.PointsObtained = GetTopScore;
            if (currentRow == null == false)
            {
                if (Extra5OfAKind(currentRow!) == true)
                    _thisI.Extra5OfAKind(this);
            } // no need to invalidate visual because of data binding.
        }
        public void MarkScore(RowInfo currentRow)
        {
            currentRow.IsRecent = true;
            if (currentRow.Possible.HasValue == true)
                currentRow.PointsObtained = currentRow.Possible;
            else
                currentRow.PointsObtained = 0;// i think.  since its nullable  needs to be different
            ClearPossibleScores();
            FinishMarking(currentRow);
        }
        public void ClearBoard() // would do at the beginning of a game
        {
            var thisList = (from items in RowList
                            where items.RowSection != EnumRowEnum.Header
                            select items).ToList();
            foreach (var thisRow in thisList)
                thisRow.ClearText();
        }
        private bool NeedsToCalculateBonus()
        {
            bool rets = RowList.Where(items => items.IsTop == true && items.RowSection == EnumRowEnum.Regular)
                .All(Items => Items.HasFilledIn() == true);
            if (rets == false)
                return false;
            RowInfo thisRow = RowList.Single(items => items.RowSection == EnumRowEnum.Bonus);
            return !thisRow.HasFilledIn();
        }
        protected override void EnableChange() { }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        public async Task HandleAsync(SelectionChosenEventModel message)
        {
            _thisE.Unsubscribe(this); //unregister now.
            switch (message.OptionChosen)
            {
                case EnumOptionChosen.Yes:
                    await ProcessMoveAsync(_privateChosen!);
                    break;
                case EnumOptionChosen.No:
                    _thisMod.CommandContainer!.ManuelFinish = false; //i think
                    _thisMod.CommandContainer.IsExecuting = false; //i think now you are free to choose something else.
                    break;
                default:
                    throw new BasicBlankException("Should have chosen yes or no");
            }
        }
    }
}