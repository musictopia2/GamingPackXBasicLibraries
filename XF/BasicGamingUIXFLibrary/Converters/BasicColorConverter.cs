using System;
using System.Globalization;
using Xamarin.Forms;
namespace BasicGamingUIXFLibrary.Converters
{
    public class BasicColorConverter : IValueConverter
    {
        //hopefully no need for yahtzee (well see).
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string thisStr = value.ToString()!;
            return Color.FromHex(thisStr); //hopefully this simple (?)
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}