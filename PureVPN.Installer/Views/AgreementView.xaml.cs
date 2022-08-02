using Microsoft.Win32;
using PureVPN.Installer.Assistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PureVPN.Installer.Views
{
    /// <summary>
    /// Interaction logic for AgreementView.xaml
    /// </summary>
    public partial class AgreementView : UserControl
    {
        public AgreementView()
        {
            InitializeComponent();
            Checkbrowser();

            if (!InstallExtChrome.IsEnabled)
                Common.InstallExtensionForChrome = false;
        }

        private void InstallExtChrome_Checked(object sender, RoutedEventArgs e)
        {
            if (InstallExtChrome.IsChecked == true)
                Common.InstallExtensionForChrome = true;
            else
                Common.InstallExtensionForChrome = false;
        }

        private void Checkbrowser()
        {
            var browsers = GetBrowsers();

            if (browsers != null && browsers.Any())
            {

                foreach (var item in browsers)
                {
                    if (!string.IsNullOrEmpty(item.Path) && item.Name.ToLower().Contains("chrome") && System.IO.File.Exists(item.Path.Replace("\"", "")))
                    {
                        InstallExtChrome.IsEnabled = true;
                        InstallExtChrome.IsChecked = true;
                        continue;
                    }
                }
            }
            else
            {
                InstallExtChrome.IsEnabled = true;
            }

        }

        public static List<Browser> GetBrowsers()
        {
            try
            {
                RegistryKey browserKeys;
                //on 64bit the browsers are in a different location
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
                if (browserKeys == null)
                    browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
                string[] browserNames = browserKeys.GetSubKeyNames();
                var browsers = new List<Browser>();
                for (int i = 0; i < browserNames.Length; i++)
                {
                    Browser browser = new Browser();
                    RegistryKey browserKey = browserKeys.OpenSubKey(browserNames[i]);
                    browser.Name = (string)browserKey.GetValue(null);
                    RegistryKey browserKeyPath = browserKey.OpenSubKey(@"shell\open\command");
                    browser.Path = (string)browserKeyPath.GetValue(null).ToString();
                    RegistryKey browserIconPath = browserKey.OpenSubKey(@"DefaultIcon");
                    browser.IconPath = (string)browserIconPath.GetValue(null).ToString();
                    browsers.Add(browser);
                    browser.Version = "unknown";
                }

                return browsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class Browser
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string IconPath { get; set; }
            public string Version { get; set; }
        }
    }
}
