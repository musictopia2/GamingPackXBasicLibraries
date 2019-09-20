using BasicGameFramework.CommandClasses;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.ChooserClasses
{
    public class NumberPicker : SimpleControlViewModel
    {
        private readonly ItemChooserClass<NumberPieceCP> _privateChoose;
        public readonly CustomBasicCollection<NumberPieceCP> NumberList = new CustomBasicCollection<NumberPieceCP>();
        public ControlCommand<NumberPieceCP> NumberPickedCommand { get; set; }
        public event ChangedNumberValueEventHandler? ChangedNumberValueAsync;
        public delegate Task ChangedNumberValueEventHandler(int Chosen);
        public NumberPicker(IBasicGameVM thisMod) : base(thisMod)
        {
            _privateChoose = new ItemChooserClass<NumberPieceCP>(thisMod);
            _privateChoose.ValueList = NumberList;
            NumberPickedCommand = new ControlCommand<NumberPieceCP>(this, async items =>
            {
                SelectNumberValue(items.NumberValue); //do this as well.
                await ChangedNumberValueAsync!.Invoke(items.NumberValue); //looks like we never did the autotype here.

            }, thisMod, thisMod.CommandContainer!);
        }
        public int NumberToChoose(bool requiredToChoose = true, bool useHalf = true)
        {
            return _privateChoose.ItemToChoose(requiredToChoose, useHalf);
        }
        public void UnselectAll()
        {
            NumberList.UnselectAllObjects();
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        protected override void EnableChange()
        {
            NumberList.SetEnabled(IsEnabled);
        }
        public void LoadNormalNumberRangeValues(int lowestNumber, int highestNumber, int diffs = 1)
        {
            if (lowestNumber > highestNumber)
                throw new BasicBlankException("The largest number must be higher than lowest number");
            if (lowestNumber < 0)
                throw new BasicBlankException("The lowest number cannot be less than 0.  If that is needed, then rethinking is required");
            if (diffs < 1)
                throw new BasicBlankException("Must have a differential of at least 1.  Otherwise, will loop forever");
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            int x;
            var loopTo = highestNumber;
            for (x = lowestNumber; x <= loopTo; x += diffs)
                tempList.Add(x);
            LoadNumberList(tempList);
        }
        public void LoadNumberList(CustomBasicList<int> thisList)
        {
            if (thisList.Any(Items => Items < 0))
                throw new BasicBlankException("You should not be allowed to use less than 0.  If that is needed, then rethinking is required");
            CustomBasicList<NumberPieceCP> tempList = new CustomBasicList<NumberPieceCP>();
            thisList.ForEach(items =>
            {
                NumberPieceCP ThisNumber = new NumberPieceCP();
                ThisNumber.NumberValue = items;
                tempList.Add(ThisNumber);
            });
            NumberList.ReplaceRange(tempList);
        }
        public void SelectNumberValue(int number)
        {
            NumberList.SelectSpecificItem(items => items.NumberValue, number);
        }
    }
}