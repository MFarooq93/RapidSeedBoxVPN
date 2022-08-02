using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PureVPN.Updater
{
    public partial class ReleaseNotesControl : System.Windows.Controls.UserControl
    {
        public ReleaseNotesControl()
        {
            InitializeComponent();
        }

        public void LoadPage()
        {
            ChangeLoaderVisibility(true);
            Browser.IsWebBrowserContextMenuEnabled = false;
            Browser.ScriptErrorsSuppressed = true;
            Browser.AllowNavigation = true;
            LoadHtml(App.release_notes_url);
        }

        private void ChangeLoaderVisibility(bool show = false)
        {
            Execute.UIOperation(() =>
            {
                if (show)
                {
                    loader.Visibility = Visibility.Visible;
                    FormHost.Visibility = Visibility.Collapsed;
                }
                else
                {
                    loader.Visibility = Visibility.Collapsed;
                    FormHost.Visibility = Visibility.Visible;
                }
            });
        }

        private void LoadHtml(string url)
        {
            Execute.NewTask(() =>
            {
                Execute.UIOperation(() =>
                {
                    Browser.Navigate(url);
                    HandleNavigations();
                });
            });
        }

        private void HandleNavigations()
        {
            Browser.Navigated += Browser_Navigated;
            Browser.NewWindow += (sender, e) => e.Cancel = true;
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Execute.NewTask(() =>
            {
                System.Threading.Thread.Sleep(1000);
                Execute.UIOperation(() =>
                {
                    ChangeLoaderVisibility();
                });
            });
        }
       
    }
}
;