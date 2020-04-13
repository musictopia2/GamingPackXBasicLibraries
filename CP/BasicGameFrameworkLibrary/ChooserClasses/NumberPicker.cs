using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class NumberPicker : SimpleControlObservable
    {
        private readonly ItemChooserClass<NumberPieceCP> _privateChoose;
        public readonly CustomBasicCollection<NumberPieceCP> NumberList = new CustomBasicCollection<NumberPieceCP>();
        public ControlCommand NumberPickedCommand { get; set; }
        public event ChangedNumberValueEventHandler? ChangedNumberValueAsync;
        public delegate Task ChangedNumberValueEventHandler(int Chosen);

        private async Task ChooseNumberAsync(NumberPieceCP piece)
        {
            SelectNumberValue(piece.NumberValue);
            await ChangedNumberValueAsync!.Invoke(piece.NumberValue);
        }

        public NumberPicker(CommandContainer command, IGamePackageResolver resolver) : base(command)
        {
            _privateChoose = new ItemChooserClass<NumberPieceCP>(resolver);
            _privateChoose.ValueList = NumberList;
            MethodInfo method = this.GetPrivateMethod(nameof(ChooseNumberAsync));
            NumberPickedCommand = new ControlCommand(this, method, command);
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
