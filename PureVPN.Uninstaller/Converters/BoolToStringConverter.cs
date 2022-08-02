using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PureVPN.Converters
{
    class BoolToStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && value is bool && parameter != null)
            {
                var names = new List<string>();
                if (parameter != null)
                    names = parameter.ToString().Split(',').ToList();

                if (names.Count() > 1)
                    return (bool)value ? names[0].ToString() : names[1].ToString();
                else if (names.Count() == 1)
                    return (bool)value ? names[0].ToString() : "false";
                else
                    return ((bool)value).ToString();
            }
            return "false";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
