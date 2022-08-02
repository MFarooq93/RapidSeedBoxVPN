using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PureVPN.Converters
{
    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && value is bool && targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
