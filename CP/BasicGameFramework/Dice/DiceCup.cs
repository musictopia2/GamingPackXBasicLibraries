using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.NetworkingClasses.Interfaces;
using BasicGameFramework.SimpleMiscClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BasicGameFramework.Dice
{
    public class DiceCup<D> : SimpleControlViewModel, IRollMultipleDice<D> where D :
        IStandardDice, new()
    {
        public IGamePackageResolver? MainContainer { get; set; }
        private IAsyncDelayer? _delay;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IDiceEvent<D> _thisEvent; //its used in delegate
#pragma warning restore IDE0052 // Remove unread private members
        public ControlCommand<D> DiceCommand { get; set; }
        private readonly INetworkMessages? _thisNet;
        public DiceCup(DiceList<D> PrivateList, IDiceEvent<D> thisEvent) : base(thisEvent)
        {
            DiceList = PrivateList;
            _thisEvent = thisEvent;
            BasicData thisData = thisEvent.MainContainer!.Resolve<BasicData>();
            if (thisData.MultiPlayer == true)
                _thisNet = thisEvent.MainContainer.Resolve<INetworkMessages>();
            DiceCommand = new ControlCommand<D>(this, async Items => await thisEvent.DiceClicked(Items), thisEvent, thisEvent.CommandContainer!);
        }
        private bool _CanShowDice;
        public bool CanShowDice
        {
            get { return _CanShowDice; }
            set
            {
                if (SetProperty(ref _CanShowDice, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public int TotalDiceValue => DiceList.Sum(Items => Items.Value); //this is so common might as well have a routine for it.
        public DiceList<D> DiceList { get; } //only because its needed for the wpf/xamarin forms part.
        int _originalNumber;
        private int _HowManyDice;
        public int HowManyDice
        {
            get { return _HowManyDice; }
            set
            {
                if (SetProperty(ref _HowManyDice, value))
                {
                    if (value > _originalNumber)
                        _originalNumber = value;
                }
            }
        }
        private bool _HasDice;
        public bool HasDice
        {
            get { return _HasDice; }
            set
            {
                if (SetProperty(ref _HasDice, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _ShowDiceListAlways;
        public bool ShowDiceListAlways
        {
            get { return _ShowDiceListAlways; }
            set
            {
                if (SetProperty(ref _ShowDiceListAlways, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool ShowHold { get; set; } //i think this does not need to pass on information to the view model.
        public void SelectUnselectDice(int index) //since i have shortcut, if i do through another way, it will be allowed.
        {
            if (ShowHold == true)
                throw new BasicBlankException("The dice is being held, not selected");
            D thisDice = DiceList.Single(Items => Items.Index == index);
            thisDice.IsSelected = !thisDice.IsSelected;
        }
        public void HoldUnholdDice(int index)
        {
            if (ShowHold == false)
                throw new BasicBlankException("The dice is being selected, not held");
            D thisDice = DiceList[index - 1];
            thisDice.Hold = !thisDice.Hold;
        }
        public void UnholdDice() => DiceList.ForEach(items => items.Hold = false);
        public bool IsDiceHeld(int index)
        {
            if (ShowHold == false)
                throw new BasicBlankException("The dice is being selected, not held");
            return DiceList[index].Hold;
        }
        public int HowManyHeldDice()
        {
            if (ShowHold == false)
                throw new BasicBlankException("The dice is being selected, not held");
            return DiceList.Count(Items => Items.Hold);
        }
        public bool HasSelectedDice()
            => DiceList.Exists(items => items.IsSelected == true);
        public CustomBasicList<D> ListSelectedDice()
        {
            if (ShowHold == true)
                throw new BasicBlankException("The dice is being held, not selected");
            return DiceList.GetSelectedItems();
        }
        public async Task<CustomBasicList<CustomBasicList<D>>> GetDiceList(string body)
        {
            return await js.DeserializeObjectAsync<CustomBasicList<CustomBasicList<D>>>(body);
        }
        public CustomBasicList<CustomBasicList<D>> RollDice(int howManySections = 6)
        {
            if (DiceList.Count() != HowManyDice)
                RedoList();
            CustomBasicList<CustomBasicList<D>> output = new CustomBasicList<CustomBasicList<D>>();
            AsyncDelayer.SetDelayer(this, ref _delay!); //try both places.
            IGenerateDice<int> ThisG = MainContainer!.Resolve<IGenerateDice<int>>();
            CustomBasicList<int> possList = ThisG.GetPossibleList;
            possList.MainContainer = MainContainer;
            D tempDice;
            int chosen;
            howManySections.Times(() =>
            {
                CustomBasicList<D> firsts = new CustomBasicList<D>();
                for (int i = 0; i < HowManyDice; i++)
                {
                    tempDice = DiceList[i];
                    if (tempDice.Hold == false) //its uncommon enough that has to be different for different types of dice games.
                    {
                        chosen = possList.GetRandomItem();
                        tempDice = new D();
                        tempDice.Index = i + 1; //i think
                        tempDice.Populate(chosen); //so they can do what they need to.
                    }
                    firsts.Add(tempDice);
                }
                output.Add(firsts);
            });
            return output;
        }
        public void ClearDice()
        {
            if (MainContainer == null)
                throw new BasicBlankException("Needs container in order to clear dice");
            if (DiceList.MainContainer == null)
                DiceList.MainContainer = MainContainer; //try this too.
            HasDice = true;
            HowManyDice = _originalNumber;
            DiceList.Clear(HowManyDice);
        }
        private void RedoList()
        {
            DiceList.Clear(HowManyDice);
        }
        public async Task SendMessageAsync(CustomBasicList<CustomBasicList<D>> thisList)
        {
            await _thisNet!.SendAllAsync("rolled", thisList); //i think
        }
        public async Task SendMessageAsync(string Category, CustomBasicList<CustomBasicList<D>> thisList)
        {
            await _thisNet!.SendAllAsync(Category, thisList); //i think
        }
        public async Task ShowRollingAsync(CustomBasicList<CustomBasicList<D>> diceCollection, bool showVisible)
        {
            CanShowDice = showVisible;
            AsyncDelayer.SetDelayer(this, ref _delay!); //because for multiplayer, they do this part but not the other.
            await diceCollection.ForEachAsync(async firsts =>
            {
                DiceList.ReplaceDiceRange(firsts);
                int tempCount = DiceList.Count;
                if (DiceList.Any(Items => Items.Index > tempCount || Items.Index <= 0))
                    throw new BasicBlankException("Index cannot be higher than the dicecount or less than 1");
                HasDice = true;
                if (CanShowDice == true)
                {
                    Visible = true;
                    await _delay.DelayMilli(50);
                }
            });
        }
        public async Task ShowRollingAsync(CustomBasicList<CustomBasicList<D>> thisCol)
        {
            await ShowRollingAsync(thisCol, true);
        }
        public void ReplaceDiceRange(ICustomBasicList<D> thisList)
        {
            DiceList.ReplaceDiceRange(thisList);
            HowManyDice = DiceList.Count;
        }
        public void ReplaceSelectedDice()
        {
            CustomBasicList<D> TempList = DiceList.GetSelectedItems();
            DiceList.ReplaceDiceRange(TempList);
            HowManyDice = DiceList.Count;
        }
        public int ValueOfOnlyDice => DiceList.Single().Value;
        public void RemoveSelectedDice()
        {
            if (ShowHold == true)
                throw new BasicBlankException("Cannot remove selected dice because its being held instead");
            DiceList.RemoveSelectedDice();
            HowManyDice = DiceList.Count;
        }
        public void RemoveConditionalDice(Predicate<D> predicate)
        {
            DiceList.RemoveConditionalDice(predicate);
            HowManyDice = DiceList.Count;
        }
        public void HideDice()
        {
            HasDice = false;
        }
        protected override void EnableChange()
        {
            DiceCommand.ReportCanExecuteChange();
        }
        protected override void VisibleChange()
        {
            DiceCommand.ReportCanExecuteChange();
        }
        protected override void PrivateEnableAlways() { }
    }
}