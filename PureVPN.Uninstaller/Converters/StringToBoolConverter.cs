using System;
using System.Globalization;
using System.Windows.Data;

namespace PureVPN.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && targetType == typeof(bool?) && parameter != null)
            {
                return String.Equals(value.ToString().ToLower(), parameter.ToString().ToLower());
            }
            return false;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
