using PureVPN.Uninstaller.ViewModels;
using System.Dynamic;
using System.Windows.Media;
using MvvmCore;
using PureVPN.Uninstaller.Interfaces;
using System.Windows;

namespace PureVPN.Uninstaller.Helpers
{
    public class DialogHelper
    {
        public static bool? ShowDialog(BaseViewModel content)
        {
            return IoC.Get<IWindowManager>().ShowDialog(content, Settings);
        }

        public static void ShowWindow(BaseViewModel content)
        {
            IoC.Get<IWindowManager>().ShowWindow(content, Settings);
        }

        public static void CloseActiveDialog(bool result = false)
        {
            IoC.Get<IWindowManager>().Close(result);
        }

        public static Window ShowNotification(BaseViewModel content, double stayTime = 7, bool showCloseButton = true)
        {
            return null;
            //return IoC.Get<INotificationManager>().Show(content, expirationTime: TimeSpan.FromSeconds(stayTime), showCloseButton: showCloseButton);
        }

        public static void CloseNotification()
        {

        }

        private static ExpandoObject Settings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.ShowInTaskbar = false;
                settings.Owner = App.MainWindow;
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.WindowStyle = WindowStyle.None;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Background = new SolidColorBrush(Colors.Transparent);
                settings.BorderThickness = new Thickness(0, 0, 0, 0);
                settings.SizeToContent = SizeToContent.WidthAndHeight;
                settings.AllowsTransparency = true;
                return settings;
            }
        }
    }
}
