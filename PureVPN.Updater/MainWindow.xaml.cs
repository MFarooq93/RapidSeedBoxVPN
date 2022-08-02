using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tally;

namespace PureVPN.Updater
{
    public partial class MainWindow : Window
    {
        public WebClient webClient { get; set; } = null;
        public string fileName { get; set; } = string.Empty;
        public bool IsDownloaded { get; set; } = false;
        System.Timers.Timer monitorDownload;
        System.Timers.Timer startDownload;
        long bytesRecieved = 0;
        long bytesRecievedLast = 0;
        bool messageBoxShown = false;

        Uri SetupDownloadUrl => new Uri("https://dzglif4kkvz04.cloudfront.net/windows-2.0/packages/production/purevpn_setup.exe");

        public MainWindow()
        {
            InitializeComponent();

            ReleaseNotesCtrl.LoadPage();

            SetLanguageDictionary(App.language_code);

            startDownload = new System.Timers.Timer(300);
            startDownload.Elapsed += StartDownload_Elapsed;
            startDownload.AutoReset = false;
            startDownload.Start();

            monitorDownload = new System.Timers.Timer(30000);
            monitorDownload.Elapsed += MonitorDownload_Elapsed;
            monitorDownload.AutoReset = true;
            monitorDownload.Start();

            string[] segments = SetupDownloadUrl.Segments;
            string strDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\purevpn\";
            if (!Directory.Exists(strDir))
            {
                Directory.CreateDirectory(strDir);
            }
            fileName = strDir + segments[segments.Length - 1];

            Show();
            Activate();

            MixpanelService mixpanelService = new MixpanelService();
            Dictionary<string, object> properties = new Dictionary<string, object>();

            // default properties
            properties.Add("$os", "Windows");
            properties.Add("$app_version_string", App.app_version_string);
            properties.Add("$app_build_number", App.app_build_number);
            properties.Add("app_release_code", App.app_release_code);
            properties.Add("$os_version", App.os_version);
            properties.Add("$user_id", App.user_id);

            // addtional properties
            properties.Add("forced", App.app_forced);
            properties.Add("auto_update", App.app_auto_update);
            properties.Add("current", App.app_version_string);
            properties.Add("available", App.live_version);

            Task.Run(async () =>
            {
                await mixpanelService.Fire("app_update_start", App.user_id, properties);
            });
        }

        private void DownloadFile()
        {
            try
            {
                bytesRecieved = 0;
                bytesRecievedLast = 0;

                try
                {
                    if (webClient != null)
                    {
                        if (webClient.IsBusy)
                        {
                            webClient.CancelAsync();
                        }
                        webClient.Dispose();
                    }

                    webClient = null;
                }
                catch { /*No need*/ }

                webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(WebClient_DownloadFileCompleted);
                webClient.DownloadFileAsync(SetupDownloadUrl, fileName);
            }
            catch { /*No need*/ }
        }

        public static void SetLanguageDictionary(string language_code)
        {
            if (language_code == "") language_code = "en";
            ResourceDictionary dict = new ResourceDictionary();

            switch (language_code)
            {
                case "en":
                    dict.Source = new Uri("..\\Resources\\Localization\\StringResources.xaml", UriKind.Relative);
                    break;
            }
            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        void StartDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DownloadFile();
        }

        async void MonitorDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            monitorDownload.Stop();

            try
            {
                if ((!IsDownloaded) && (bytesRecieved == bytesRecievedLast))
                {
                    
                    string messageBoxTitle = "";
                    string messageBoxDescription = "";

                    if (await IsInternetAvailable())
                    {
                        if (webClient != null)
                            webClient.CancelAsync();

                        messageBoxTitle = "Something went wrong";
                        messageBoxDescription = "Download was interrupted due unexpected problem. Would you like to try again?";
                    }
                    else
                    {
                        messageBoxTitle = "No internet Connection";
                        messageBoxDescription = "Download was interrupted due to no internet connection. Would you like to try again?";
                    }

                    if(!messageBoxShown)
                    {
                        messageBoxShown = true;
                        await System.Windows.Application.Current.Dispatcher.BeginInvoke(
                     new Action(() =>
                     {
                         var result = MessageBoxExtended.Show(
                                                    Message: messageBoxDescription,
                                                    Caption: messageBoxTitle,
                                                    Buttons: MessageBoxButton.YesNo,
                                                    popTitle: "PureVPN",
                                                    positiveButtonContent: Properties.Resources.Yes,
                                                    negativeButtonContent: Properties.Resources.No
                                                    );

                         if (result == MessageBoxResult.Yes)
                         {
                             messageBoxShown = false;
                             new Thread(() =>
                             {
                                 Thread.CurrentThread.IsBackground = true;
                                 DownloadFile();
                             }).Start();

                         }
                         else if (result == MessageBoxResult.No)
                         {
                             ExitApplication();
                         }
                     }));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            bytesRecievedLast = bytesRecieved;

            monitorDownload.Start();
        }

        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;
        
        public async Task<bool> IsInternetAvailable()
        {
            if (await CheckHttpTraffic("https://www.google.com"))
                return true;

            if (await CheckHttpTraffic("http://captive.apple.com"))
                return true;

            if (await CheckHttpTraffic("https://www.Baidu.com"))
                return true;

            return false;
        }

        public async Task<bool> CheckHttpTraffic(string url)
        {
            try
            {
                var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(4);
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public void WebClient_DownloadProgressChanged(Object sender, DownloadProgressChangedEventArgs e)
        {
            bytesRecieved = e.BytesReceived;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.BytesReceived < 1048576)
                    downloadedKb.Content = (e.BytesReceived / 1024).ToString() + " KB";
                else
                {
                    double BytesReceived = e.BytesReceived;
                    downloadedKb.Content = (BytesReceived / 1048576).ToString("0.0") + " MB";
                }

                Progress.Value = e.ProgressPercentage;
                StatusText.Text = e.ProgressPercentage.ToString() + " %";
            }));

            if (messageBoxShown)
            {
                messageBoxShown = false;
                var msgBoxWindow = FindWindow(null, "PureVPN Dialog Box");
                if (msgBoxWindow != IntPtr.Zero)
                {
                    SendMessage(msgBoxWindow, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                IsDownloaded = true;
                monitorDownload.Stop();
                Process.Start(fileName);

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Application.Current.Shutdown();
                }));
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        public void ExitApplication()
        {
            if (webClient != null && webClient.IsBusy)
            {
                webClient.CancelAsync();
                Application.Current.Shutdown();
            }
            else
            {
                if (IsDownloaded)
                {
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        Process.Start(startInfo);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                else
                {
                    App.Current.Shutdown();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ShowInTaskbar = false;
            Hide();
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Collapsed;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
