using System;
using System.Globalization;
using System.Windows.Data;

namespace PureVPN.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool))
                return !(bool)value;
            return false;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
