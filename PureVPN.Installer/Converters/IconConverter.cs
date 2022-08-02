using System;
using System.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Tally;

namespace PureVPN.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                Icon ico = (value as Icon);
                Bitmap bits = ico.ToBitmap();
                MemoryStream strm = new MemoryStream();
                // add the stream to the image streams collection so we can get rid of it later
                // _imageStreams.Add(strm);
                bits.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.BeginInit();
                bitmap.StreamSource = strm;
                bitmap.EndInit();
                // freeze it here for performance
                bitmap.Freeze();
            }
            catch (Exception ex) { Logger.Log(ex); }
            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
