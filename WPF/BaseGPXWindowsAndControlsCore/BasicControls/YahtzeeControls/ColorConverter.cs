using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace BaseGPXWindowsAndControlsCore.BasicControls.YahtzeeControls
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool thisBool;
            thisBool = bool.Parse(value.ToString()!);
            if (thisBool == true)
                return Brushes.Yellow;
            return Brushes.White;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}