using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.DIContainers;
using System.Windows.Input;
using BasicGameFrameworkLibrary.Extensions;
using System.Reflection;
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class ListViewPicker : SimpleControlObservable, IListViewPicker
    {
        //no problem having the enum here since it applies only to listviewpicker.  decided to not change this part.


        //decided to risk manually creating the reflection command.  this will help later if it works out.

        public readonly CustomBasicCollection<ListViewPieceCP> TextList = new CustomBasicCollection<ListViewPieceCP>();
        //could be iffy (?)

        public enum EnumIndexMethod
        {
            Unknown = 0,
            ZeroBased = 1,
            OneBased = 2
        }
        public enum EnumSelectionMode
        {
            SingleItem = 1,
            MultipleItems = 2
        }

        private async Task ProcessClickAsync(ListViewPieceCP piece)
        {
            if (SelectionMode == EnumSelectionMode.SingleItem)
            {
                SelectSpecificItem(piece.Index);
            }
            else if (piece.IsSelected)
            {
                piece.IsSelected = false;
            }
            else
            {
                piece.IsSelected = true;
            }
            if (ItemSelectedAsync == null)
            {
                return; //ignore because not there.
            }
            await ItemSelectedAsync.Invoke(piece.Index, piece.DisplayText);
        }

        public ListViewPicker(CommandContainer container, IGamePackageResolver resolver) : base(container)
        {
            _privateChoose = new ItemChooserClass<ListViewPieceCP>(resolver);
            _privateChoose.ValueList = TextList;
            MethodInfo method = this.GetPrivateMethod(nameof(ProcessClickAsync));
            ItemSelectedCommand = new ControlCommand(this, method, container);
        }

        public EnumIndexMethod IndexMethod { get; set; } // so when i send the list, it knows whether to start with 0 or 1.
        public EnumSelectionMode SelectionMode { get; set; } = EnumSelectionMode.SingleItem;
        public event ItemSelectedEventHandler? ItemSelectedAsync; // better to have too much information than not enough information.
        public delegate Task ItemSelectedEventHandler(int selectedIndex, string selectedText);
        private readonly ItemChooserClass<ListViewPieceCP> _privateChoose;
        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (SetProperty(ref _selectedIndex, value) == true)
                {
                }
            }
        }
        public string GetText(int index)
        {
            if (IndexMethod == EnumIndexMethod.ZeroBased)
                return TextList[index].DisplayText;
            if (IndexMethod == EnumIndexMethod.OneBased)
                return TextList[index - 1].DisplayText;// i think
            throw new BasicBlankException("Don't know the index method");
        }
        public int IndexOf(string text)
        {
            return TextList.Where(Items => Items.DisplayText == text).Single().Index;
        }
        public void LoadTextList(CustomBasicList<string> thisList)
        {
            if (IndexMethod == EnumIndexMethod.Unknown)
                throw new BasicBlankException("Must know the index method in order to continue");
            CustomBasicList<ListViewPieceCP> TempList = new CustomBasicList<ListViewPieceCP>();
            int x;
            if (IndexMethod == EnumIndexMethod.OneBased)
                x = 1;
            else
                x = 0;
            foreach (var firstText in thisList)
            {
                ListViewPieceCP newText = new ListViewPieceCP();
                newText.Index = x;
                newText.DisplayText = firstText;
                TempList.Add(newText);
                x += 1;
            }
            TextList.ReplaceRange(TempList);
        }
        public void UnselectAll()
        {
            TextList.UnselectAllObjects();
        }
        public void SelectSpecificItem(int index)
        {
            if (SelectionMode == EnumSelectionMode.SingleItem)
            {
                foreach (var thisText in TextList)
                {
                    if (thisText.Index == index)
                        thisText.IsSelected = true;
                    else
                        thisText.IsSelected = false;
                }
                return;
            }
            throw new BasicBlankException("Should have used SelectSeveralItems for selecting several items");
        }
        public void ShowOnlyOneSelectedItem(string text)
        {
            if (SelectionMode == EnumSelectionMode.MultipleItems)
                throw new BasicBlankException("Must have single selection for showing one selected item");
            ListViewPieceCP thisPick = TextList.Single(Items => Items.DisplayText == text);
            TextList.ReplaceAllWithGivenItem(thisPick); //i think this is best.  so a person see's just one item.
            //games like payday and maybe game of life does it this way.
        }
        public int Count()
        {
            return TextList.Count;
        }
        public void SelectSeveralItems(CustomBasicList<int> thisList)
        {
            if (SelectionMode == EnumSelectionMode.SingleItem)
                throw new BasicBlankException("Cannot select several items because you chose to select only one item.");
            UnselectAll();
            foreach (var thisItem in thisList)
            {
                var news = (from Items in TextList
                            where Items.Index == thisItem
                            select Items).Single();
                news.IsSelected = true;
            }
        }
        public CustomBasicList<int> GetAllSelectedItems()
        {
            if (SelectionMode == EnumSelectionMode.SingleItem)
                throw new BasicBlankException("Cannot get all selected items because there was only one selected.  Try using the property SelectedIndex");
            return (from Items in TextList
                    where Items.IsSelected == true
                    select Items.Index).ToCustomBasicList();
        }
        //public ControlCommand<ListViewPieceCP> ItemSelectedCommand { get; set; }
        
        public ControlCommand ItemSelectedCommand { get; set; }
        
        ICommand IListViewPicker.ItemSelectedCommand { get => ItemSelectedCommand; }

        CustomBasicCollection<ListViewPieceCP> IListViewPicker.TextList => TextList;

        
        protected override void EnableChange()
        {
            TextList.SetEnabled(IsEnabled); //i think this was needed too.
        }
        protected override void PrivateEnableAlways() { }
        //protected override void VisibleChange() { }
        public int ItemToChoose(bool requiredToChoose = true, bool useHalf = true)
        {
            return _privateChoose.ItemToChoose(requiredToChoose, useHalf);
        }

    }
}
