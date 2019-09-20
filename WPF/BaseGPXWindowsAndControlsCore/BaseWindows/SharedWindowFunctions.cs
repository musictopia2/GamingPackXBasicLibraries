using BasicControlsAndWindowsCore.BasicWindows.Misc;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace BaseGPXWindowsAndControlsCore.BaseWindows
{
    public static class SharedWindowFunctions
    {
        public static Button GetGamingButton(string text, string commandPath)
        {
            Button thisBut = new Button();
            thisBut.FontSize = 35;
            thisBut.Foreground = Brushes.Black;
            thisBut.Background = Brushes.Aqua;
            thisBut.SetBinding(Button.CommandProperty, new Binding(commandPath));
            thisBut.Content = text;
            thisBut.BorderBrush = Brushes.White;
            thisBut.BorderThickness = new Thickness(1, 1, 1, 1);
            thisBut.Margin = new Thickness(2, 2, 2, 2);
            return thisBut;
        }
        public static void SetDefaultStartOrientations(Control control) //brand new.
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
        }
        public static Binding GetVisibleBinding(string visiblePath, bool useCollapsed = true)
        {
            return WindowHelper.GetVisibleBinding(visiblePath, useCollapsed);
        }
        public static TextBlock GetDefaultLabel()
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.Foreground = Brushes.Aqua;
            return thisLabel;
        }
    }
}