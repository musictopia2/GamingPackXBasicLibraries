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
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Helpers
{
    public static class SharedUIFunctions
    {
        public static Button GetGamingButton(string text, string name)
        {
            Button thisBut = new Button();
            thisBut.FontSize = 35;
            thisBut.Foreground = Brushes.Black;
            thisBut.Background = Brushes.Aqua;
            thisBut.Name = name;
            thisBut.Content = text;
            thisBut.BorderBrush = Brushes.White;
            thisBut.BorderThickness = new Thickness(1, 1, 1, 1);
            thisBut.Margin = new Thickness(3);
            return thisBut;
        }
        public static void SetDefaultStartOrientations(Control control) //brand new.
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
        }
        public static Binding GetVisibleBinding(string visiblePath)
        {
            return WindowHelper.GetVisibleBinding(visiblePath);
        }
        public static TextBlock GetDefaultLabel()
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.Foreground = Brushes.Aqua;
            return thisLabel;
        }

        public static void AddVerticalLabelGroup(string header, string name, StackPanel stack, bool boldHeader = false)
        {
            //i think we need this.
            TextBlock thisLabel = GetDefaultLabel();
            thisLabel.Text = $"{header}:";
            if (boldHeader == true)
            {
                thisLabel.FontWeight = FontWeights.Bold;
            }
            stack.Children.Add(thisLabel);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(TextBlock.TextProperty, new Binding(name)); //i guess can do old fashioned here too.
            thisLabel.TextWrapping = TextWrapping.Wrap;
            stack.Children.Add(thisLabel);
        }

    }
}