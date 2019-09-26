using CommonBasicStandardLibraries.CommonConverters;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Globalization;
namespace BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses
{
    public abstract class ServerConverterCP : IConverterCP
    {
        public object Convert(object value, Type targetType, object Parameter, CultureInfo culture)
        {
            if (value == null)
                throw new BasicBlankException("value cannot be null in server converter");
            EnumServerMode mode = (EnumServerMode)value;
            return mode switch
            {
                EnumServerMode.AzureHosting => "Azure Hosting",
                EnumServerMode.HomeHosting => "Home Network Hosting",
                EnumServerMode.LocalHosting => "Local Hosting",
                EnumServerMode.MobileServer => "Mobile Server",
                _ => "Unknown"
            };
        }
        public object ConvertBack(object value, Type targetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}