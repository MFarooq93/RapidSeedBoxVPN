using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PureVPN.Converters
{
    class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bitmap = new BitmapImage();
            if (value is String 
                && !String.IsNullOrEmpty(value.ToString())
                && targetType == typeof(ImageSource))
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(value.ToString(), UriKind.Absolute);
                bitmap.EndInit();
            }

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
