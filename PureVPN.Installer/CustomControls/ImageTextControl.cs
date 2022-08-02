using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PureVPN.CustomControls
{
    public class ImageTextControl : ContentControl
    {
        static ImageTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTextControl),
                new FrameworkPropertyMetadata(typeof(ImageTextControl)));
        }

        public string TextToShow
        {
            get { return (string)GetValue(TextToShowProperty); }
            set { SetValue(TextToShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextToShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextToShowProperty =
            DependencyProperty.Register("TextToShow", typeof(string), typeof(ImageTextControl), new PropertyMetadata(""));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageTextControl), new PropertyMetadata(null));

        public string TooltipText
        {
            get { return (string)GetValue(TooltipTextProperty); }
            set { SetValue(TooltipTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TooltipText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipTextProperty =
            DependencyProperty.Register("TooltipText", typeof(string), typeof(ImageTextControl), new PropertyMetadata(""));

    }
}
