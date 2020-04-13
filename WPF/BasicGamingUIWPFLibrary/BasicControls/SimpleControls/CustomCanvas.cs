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
using System.Windows.Controls;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using System.Windows;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.SimpleControls
{
    public class CustomCanvas : Canvas, ICanvas
    {
        public void SetLocation(ISelectableObject thisImage, double x, double y)
        {
            SetLeft((UIElement)thisImage, x);
            SetTop((UIElement)thisImage, y);
        }
        public void Clear()
        {
            Children.Clear();
        }
        public void AddChild(ISelectableObject thisImage)
        {
            Children.Add((UIElement)thisImage);
        }
    }
}
