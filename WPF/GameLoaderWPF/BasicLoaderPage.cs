﻿using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace GameLoaderWPF
{
    public abstract class BasicLoaderPage<VM> : Window
        where VM : ILoaderVM, new()
    {
        public int TotalColumns { get; set; }
        protected virtual void StartUp() { }
        protected bool Multiplayer { get; }
        protected virtual Size DefaultWindowSize()
        {
            return new Size(1800, 950);
        }
        public BasicLoaderPage(IStartUp starts, bool multiplayer)
        {
            Multiplayer = multiplayer;
            StartUp();
            OS = EnumOS.WindowsDT; //this part is okay.
            var tempSize = DefaultWindowSize();
            WindowHelper.CurrentWindow = this;
            WindowHelper.SetDefaultLocation();
            WindowHelper.SetSize(tempSize.Width, tempSize.Height);
            Background = Brushes.Navy;
            VM thisMod = new VM();
            thisMod.Init(this, starts);
            ListChooserWPF lists = new ListChooserWPF();
            lists.Orientation = Orientation.Horizontal;
            lists.ItemWidth += 50;
            if (TotalColumns == 0)
                TotalColumns = 5;
            lists.TotalColumns = TotalColumns; //this should be fine so i can test something else.
            if (thisMod.PackagePicker!.TextList.Count == 0)
                throw new BasicBlankException("No items");
            if (multiplayer == true)
                throw new BasicBlankException("Not sure what to do about multiplayer for now because needs some way to have nick names, etc.");
            lists.LoadLists(thisMod.PackagePicker!);
            Content = lists;
            Show();
        }
    }
}