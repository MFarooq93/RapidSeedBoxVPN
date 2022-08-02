using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tally;


namespace PureVPN.CustomControls
{
    public class ImageWithTextLink : Label
    {
        public ImageWithTextLink()
        {

        }

        public ImageWithTextLink(string image, string text, string link)
        {
            try
            {
                if (!String.IsNullOrEmpty(image))
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(image);
                    bmp.EndInit();
                    Icon = bmp;
                }

                Content = text;
                Link = link;
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public ImageSource Icon
        {
            get { return (ImageSource)base.GetValue(IconProperty); }
            set { base.SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
          DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ImageWithTextLink));

        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(ImageWithTextLink), new PropertyMetadata(""));

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            try
            {
                if (!String.IsNullOrEmpty(Link))
                {
                    Process.Start(Link);
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

    }
}
