using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace BasicGamingUIWPFLibrary.Converters
{
    public class BasicColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string thisStr = value.ToString()!;
            var newColor = System.Windows.Media.ColorConverter.ConvertFromString(thisStr);
            return new SolidColorBrush((Color)newColor);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}