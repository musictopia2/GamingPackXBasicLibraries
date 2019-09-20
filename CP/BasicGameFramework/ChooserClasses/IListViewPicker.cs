using BasicGameFramework.CommandClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.ComponentModel;
using System.Windows.Input;
namespace BasicGameFramework.ChooserClasses
{
    public interface IListViewPicker : INotifyPropertyChanged
    {
        ICommand ItemSelectedCommand { get; }
        bool Visible { get; set; }
        CustomBasicCollection<ListViewPieceCP> TextList { get; }
    }
}