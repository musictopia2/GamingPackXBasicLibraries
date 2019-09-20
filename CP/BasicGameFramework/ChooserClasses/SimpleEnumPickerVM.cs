using BasicGameFramework.CommandClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.ChooserClasses
{
    public class SimpleEnumPickerVM<E, PI, LI> : SimpleControlViewModel where PI : BaseGraphicsCP, IEnumPiece<E>, new()
        where E : struct, Enum where LI : IEnumListClass<E>, new()
    {
        public CustomBasicCollection<PI> ItemList = new CustomBasicCollection<PI>(); //this is needed so the databinding will work.
        private EnumAutoSelectCategory _AutoSelectCategory;
        public EnumAutoSelectCategory AutoSelectCategory
        {
            get
            {
                return _AutoSelectCategory;
            }
            set
            {
                if (SetProperty(ref _AutoSelectCategory, value) == true)
                {
                }
            }
        }
        private E? _ItemChosen;
        public E? ItemChosen
        {
            get { return _ItemChosen; }
            set
            {
                if (SetProperty(ref _ItemChosen, value))
                {
                    ItemSelectionChanged!(value);
                }
            }
        }
        public event ItemClickedEventHandler? ItemClickedAsync;
        public delegate Task ItemClickedEventHandler(E ThisPiece);
        public event ItemChangedEventHandler? ItemSelectionChanged;
        public delegate void ItemChangedEventHandler(E? ThisPiece); //can't be async since property does this.
        protected CustomBasicList<PI> PrivateGetList()
        {
            CustomBasicList<E> firstList = _thisChoice.GetEnumList();
            CustomBasicList<PI> tempList = new CustomBasicList<PI>();
            firstList.ForEach(items =>
            {
                PI thisTemp = new PI();
                thisTemp.EnumValue = items;
                thisTemp.IsSelected = false;
                thisTemp.IsEnabled = IsEnabled; //start with false.  to prove the problem with bindings.
                tempList.Add(thisTemp);
            });
            return tempList;
        }
        public void LoadEntireList()
        {
            var tempList = PrivateGetList();
            ItemList.ReplaceRange(tempList);
        }
        public void LoadEntireListExcludeOne(E thisEnum)
        {
            var firstList = PrivateGetList();
            firstList.KeepConditionalItems(Items => Items.Equals(thisEnum) == false);
            ItemList.ReplaceRange(firstList);
        }
        private readonly IEnumListClass<E> _thisChoice;
        public ControlCommand<PI> EnumChosenCommand { get; set; }
        public SimpleEnumPickerVM(IBasicGameVM thisMod) : base(thisMod)
        {
            _thisChoice = new LI();
            LoadEntireList();
            EnumChosenCommand = new ControlCommand<PI>(this, async thisOption =>
            {
                if (AutoSelectCategory == EnumAutoSelectCategory.AutoEvent)
                {
                    await ItemClickedAsync!.Invoke(thisOption.EnumValue);
                    return;
                }
                if (AutoSelectCategory == EnumAutoSelectCategory.AutoSelect)
                {
                    SelectSpecificItem(thisOption.EnumValue);
                    ItemSelectionChanged!.Invoke(thisOption.EnumValue);
                    return;
                }
                throw new BasicBlankException("Not Supported");
            }, thisMod, thisMod.CommandContainer!);

        }
        protected override bool CanEnableFirst()
        {
            return AutoSelectCategory == EnumAutoSelectCategory.AutoEvent || AutoSelectCategory == EnumAutoSelectCategory.AutoSelect;
        }
        protected override void EnableChange()
        {
            EnumChosenCommand.ReportCanExecuteChange();
            ItemList.ForEach(Items =>
            {
                Items.IsEnabled = IsEnabled;
            });
        }
        protected override void VisibleChange()
        {
            EnumChosenCommand.ReportCanExecuteChange();
        }
        public E SelectedItem(int index)
        {
            return ItemList[index].EnumValue;
        }
        public void UnselectAll()
        {
            ItemList.ForEach(items => items.IsSelected = false);
        }
        public void SelectSpecificItem(E optionChosen)
        {
            ItemList.ForEach(items =>
            {
                if (items.EnumValue.Equals(optionChosen) == true)
                    items.IsSelected = true;
                else
                    items.IsSelected = false;
            });
        }
        public void ChooseItem(E optionChosen)
        {
            ItemList.KeepConditionalItems(items => items.EnumValue.Equals(optionChosen));
            if (ItemList.Count != 1)
                throw new BasicBlankException("Did not have just one choice for option chosen.  Rethink");
        }
        public E ItemToChoose()
        {
            return ItemList.GetRandomItem().EnumValue;
        }
        protected override void PrivateEnableAlways() { }
    }
}