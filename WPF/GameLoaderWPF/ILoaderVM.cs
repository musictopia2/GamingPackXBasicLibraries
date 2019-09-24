using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GameLoaderWPF
{
    public interface ILoaderVM
    {
        void Init(Window window, IStartUp starts);
        GamePackageLoaderPickerCP? PackagePicker { get; set; }
        string Title { get; set; }
    }
}