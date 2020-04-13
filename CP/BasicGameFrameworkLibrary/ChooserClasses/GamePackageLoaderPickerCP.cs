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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using System.Windows.Input;
using System.Reflection;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class GamePackageLoaderPickerCP : ObservableObject, IListViewPicker
    {
        //this has to have lots of repeating code unfortunately.  by doing as interface, then can at least reuse the ui part of it.
        //we may not need this part (?)
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public void LoadTextList(CustomBasicList<string> thisList)
        {
            //CustomBasicList<ListViewPieceCP> TempList = new CustomBasicList<ListViewPieceCP>();
            int x;
            //do one based.
            x = 1;
            //TextList = new CustomBasicCollection<ListViewPieceCP>();
            foreach (var firstText in thisList)
            {
                ListViewPieceCP newText = new ListViewPieceCP();
                newText.Index = x;
                newText.DisplayText = firstText;
                newText.IsEnabled = true;
                //TempList.Add(newText);
                TextList.Add(newText); //try this way.
                x += 1;
            }
            //TextList.ReplaceRange(TempList);
            if (TextList.Count == 0)
                throw new BasicBlankException("Failed to load text list");
        }
        public ReflectiveCommand ItemSelectedCommand { get; set; }
        public CustomBasicCollection<ListViewPieceCP> TextList = new CustomBasicCollection<ListViewPieceCP>();
        ICommand IListViewPicker.ItemSelectedCommand => ItemSelectedCommand;
        CustomBasicCollection<ListViewPieceCP> IListViewPicker.TextList => TextList;

        public event ItemSelectedEventHandler? ItemSelectedAsync; // better to have too much information than not enough information.
        public delegate Task ItemSelectedEventHandler(int SelectedIndex, string SelectedText);
        public void SelectSpecificItem(int index)
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


        private async Task ProcessItemAsync(ListViewPieceCP piece)
        {
            SelectSpecificItem(piece.Index);
            if (ItemSelectedAsync == null)
            {
                return;
            }
            await ItemSelectedAsync.Invoke(piece.Index, piece.DisplayText);
        }

        public GamePackageLoaderPickerCP()
        {
            MethodInfo method = this.GetPrivateMethod(nameof(ProcessItemAsync));
            ItemSelectedCommand = new ReflectiveCommand(this, method, canExecuteM: null!);
            //ItemSelectedCommand = new ControlCommand(this, method, container);
            //ItemSelectedCommand = new Command<ListViewPieceCP>(async items =>
            //{
            //    SelectSpecificItem(items.Index);
            //    if (ItemSelectedAsync == null)
            //        return;
            //    await ItemSelectedAsync.Invoke(items.Index, items.DisplayText);
            //}, items => true, null);
            Visible = true; //for this one, show as true.
        }
    }
}
