using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Input;
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class GamePackageLoaderPickerCP : ObservableObject, IListViewPicker
    {
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
            int x;
            x = 1;
            foreach (var firstText in thisList)
            {
                ListViewPieceCP newText = new ListViewPieceCP();
                newText.Index = x;
                newText.DisplayText = firstText;
                newText.IsEnabled = true;
                TextList.Add(newText); //try this way.
                x += 1;
            }
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
            Visible = true; //for this one, show as true.
        }
    }
}
