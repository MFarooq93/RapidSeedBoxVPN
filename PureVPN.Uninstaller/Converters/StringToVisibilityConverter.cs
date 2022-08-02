using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PureVPN.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && value is string && targetType == typeof(Visibility))
            {
                bool valid = !String.IsNullOrEmpty(value.ToString());
                if (parameter != null && parameter.ToString().ToLower().Contains("invert"))
                    valid = !valid;
                return valid ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
