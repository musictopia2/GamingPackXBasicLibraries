using System;
using System.Globalization;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.Converters
{
    public class YahtzeeColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool thisBool = bool.Parse(value.ToString()!);
            if (thisBool == true)
                return Color.Yellow;
            return Color.White;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}