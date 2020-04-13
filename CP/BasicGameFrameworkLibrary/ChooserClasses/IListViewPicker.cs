using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public interface IListViewPicker : INotifyPropertyChangedEx
    {

        //the itemselectedcommand could be iffy.  hopefully no problem with this (?)

        ICommand ItemSelectedCommand { get; }
        //hopefully don't need the visible anymore since its splitted out now.
        //bool Visible { get; set; }
        CustomBasicCollection<ListViewPieceCP> TextList { get; }
    }
}