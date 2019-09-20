using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public abstract class TrumpConverterCP : IConverterCP
    {
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            return value.ToString(); //hopefully this simple because we simply want a string for this part.
        }
        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}