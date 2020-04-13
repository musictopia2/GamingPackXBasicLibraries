using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    //can risk doing with less generics.
    public class SimpleEnumPickerVM<E, PI> : SimpleControlObservable where PI : BaseGraphicsCP, IEnumPiece<E>, new()
        where E : struct, Enum
    {
        public CustomBasicCollection<PI> ItemList = new CustomBasicCollection<PI>(); //this is needed so the databinding will work.
        private EnumAutoSelectCategory _autoSelectCategory;
        public EnumAutoSelectCategory AutoSelectCategory
        {
            get
            {
                return _autoSelectCategory;
            }
            set
            {
                if (SetProperty(ref _autoSelectCategory, value) == true)
                {
                }
            }
        }
        private E? _itemChosen;
        public E? ItemChosen
        {
            get { return _itemChosen; }
            set
            {
                if (SetProperty(ref _itemChosen, value))
                {
                    ItemSelectionChanged!(value);
                }
            }
        }
        public event ItemClickedEventHandler? ItemClickedAsync;
        public delegate Task ItemClickedEventHandler(E piece);
        public event ItemChangedEventHandler? ItemSelectionChanged;
        public delegate void ItemChangedEventHandler(E? piece); //can't be async since property does this.
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
        public ControlCommand EnumChosenCommand { get; set; }

        private async Task ProcessClickAsync(PI chosen)
        {
            if (AutoSelectCategory == EnumAutoSelectCategory.AutoEvent)
            {
                await ItemClickedAsync!.Invoke(chosen.EnumValue);
                return;
            }
            if (AutoSelectCategory == EnumAutoSelectCategory.AutoSelect)
            {
                SelectSpecificItem(chosen.EnumValue);
                ItemSelectionChanged!.Invoke(chosen.EnumValue);
                return;
            }
            throw new BasicBlankException("Not Supported");
        }

        public SimpleEnumPickerVM(CommandContainer container, IEnumListClass<E> thisChoice ) : base(container)
        {
            _thisChoice = thisChoice;
            LoadEntireList();
            MethodInfo method = this.GetPrivateMethod(nameof(ProcessClickAsync));
            EnumChosenCommand = new ControlCommand(this, method, container);

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