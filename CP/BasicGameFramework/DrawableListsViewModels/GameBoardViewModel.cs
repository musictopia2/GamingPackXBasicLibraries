using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Threading.Tasks;
namespace BasicGameFramework.DrawableListsViewModels
{
    public abstract class GameBoardViewModel<D> : ObservableObject, IControlVM where D : IDeckObject, new()
    {
        public DeckObservableDict<D> ObjectList = new DeckObservableDict<D>();
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int MaxCards { get; set; } = 0;
        private string _Text = "";
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (SetProperty(ref _Text, value) == true) { }
            }
        }
        private bool _HasFrame = true;
        public bool HasFrame
        {
            get
            {
                return _HasFrame;
            }

            set
            {
                if (SetProperty(ref _HasFrame, value) == true) { }
            }
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (SetProperty(ref _IsEnabled, value) == true)
                    // code to run
                    ChangeEnabled();
            }
        }
        private bool _Visible = true; // since i already did the bindings there, has to be there instead.
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (SetProperty(ref _Visible, value) == true) { }
            }
        }
        protected virtual void ChangeEnabled() { }
        public void LoadSavedGame(IDeckDict<D> list)
        {
            ObjectList.ReplaceRange(list); //risk a breaking change here. so i don't have to use strings anymore for this.
        }
        public int CalculateMaxCards()
        {
            if (MaxCards == 0)
                return Columns * Rows;
            return MaxCards;
        }
        protected D GetObject(int row, int column)
        {
            int x;
            int y;
            int z = 0;
            var loopTo = Rows;
            for (x = 1; x <= loopTo; x++)
            {
                var loopTo1 = Columns;
                for (y = 1; y <= loopTo1; y++)
                {
                    if (z + 1 > ObjectList.Count)
                        throw new BasicBlankException($"Somehow was out of range.  z was {z + 1} and count was {ObjectList.Count}");
                    if (y == column && x == row)
                        return ObjectList[z];
                    z += 1;
                }
            }
            throw new BasicBlankException("No card at row " + row + " and column " + column);
        }
        protected (int row, int column) GetRowColumnData(D thisCard)
        {
            int x;
            int y;
            int z = 0;
            var loopTo = Rows;
            for (x = 1; x <= loopTo; x++)
            {
                var loopTo1 = Columns;
                for (y = 1; y <= loopTo1; y++)
                {
                    var tempCard = ObjectList[z];
                    z += 1;
                    if (tempCard.Deck == thisCard.Deck)
                        // this will presume its the same.  if i am wrong, can fix.
                        return (x, y);// hopefully this works.
                }
            }
            throw new BasicBlankException("Can't find row/column data for Card With Deck Of " + thisCard.Deck);
        }
        public void TradeObject(int row, int column, D newObject)
        {
            var FirstObject = GetObject(row, column);
            ObjectList.ReplaceItem(FirstObject, newObject);
        }
        public void TradeObject(int oldDeck, D newObject)
        {
            var FirstObject = ObjectList.GetSpecificItem(oldDeck);
            ObjectList.ReplaceItem(FirstObject, newObject); //maybe no need to check for duplicates because its using dictionary now.
        }
        protected abstract Task ClickProcessAsync(D thisObject); // this is where we need code for when clicking a card
        private IBasicEnableProcess? _networkProcess;
        private Func<bool>? _customFunction; // i think this will have no parameters for this one.
        public void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
        {
            _networkProcess = nets;
            _customFunction = fun;
        }
        bool IControlVM.CanExecute()
        {
            if (Visible == false)
                return false;
            if (IsEnabled == false)
                return false;
            return true;
        }
        public void ReportCanExecuteChange()
        {
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
            {
                IsEnabled = false; //i think
                return;
            }
            if (_thisMod.NewGameVisible == true)
            {
                IsEnabled = false;
                return; //because if the new game is visible, you can't do anything with the controls period.
            }
            if (_networkProcess == null)
            {
                IsEnabled = true;
                return; //because you did not send something in.
            }
            if (_networkProcess.CanEnableBasics() == false)
            {
                IsEnabled = false;
                return;
            }
            IsEnabled = _customFunction!();
        }
        public PlainCommand<D> ObjectCommand { get; set; }
        public EnumCommandBusyCategory BusyCategory { get; set; } = EnumCommandBusyCategory.None; //most of the time, none.
        protected CommandContainer CommandContainer;
        private readonly IBasicGameVM _thisMod;
        public GameBoardViewModel(IBasicGameVM thisMod)
        {

            ObjectCommand = new PlainCommand<D>(async items => await ClickProcessAsync(items),
                thisObject =>
                {
                    if (thisObject.Visible == false)
                        return false;
                    if (IsEnabled == false)
                        return false;
                    if (Visible == false)
                        return false;
                    return true;
                }, thisMod, thisMod.CommandContainer!);
            _thisMod = thisMod;
            CommandContainer = thisMod.CommandContainer!;
            CommandContainer.AddControl(this); //i think it should be here instead.
        }
    }
}