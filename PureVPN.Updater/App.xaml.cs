using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace PureVPN.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string language_code { get; set; } = "en";
        public static string app_version_string { get; set; } = "";
        public static string app_build_number { get; set; } = "";
        public static string app_release_code { get; set; } = "";
        public static string os_version { get; set; } = "";
        public static string user_id { get; set; } = "";
        public static string live_version { get; set; } = "";
        public static string release_notes_url { get; set; } = "";
        public static string app_forced { get; set; } = "";
        public static string app_auto_update { get; set; } = "";


        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "PureVPN Updater";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                ActivateWindow();
                Application.Current.Shutdown();
            }

            base.OnStartup(e);

        }

        private static void ActivateWindow()
        {
            var otherWindow = FindWindow(null, "PureVPN Updater");
            if (otherWindow != IntPtr.Zero)
            {
                ShowWindow(otherWindow, 9);
                SetForegroundWindow(otherWindow);
            }
        }

        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [STAThread()]
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                string argumentKey = args[i];
                string argumentValue = args[i + 1];

                switch (argumentKey)
                {
                    case "-app_version_string":
                        app_version_string = argumentValue;
                        break;
                    case "-app_build_number":
                        app_build_number = argumentValue;
                        break;
                    case "-app_release_code":
                        app_release_code = argumentValue;
                        break;
                    case "-os_version":
                        os_version = argumentValue;
                        break;
                    case "-user_id":
                        user_id = argumentValue;
                        break;
                    case "-live_version":
                        live_version = argumentValue;
                        break;
                    case "-release_notes_url":
                        release_notes_url = argumentValue;
                        break;
                    case "-app_forced":
                        app_forced = argumentValue;
                        break;
                    case "-app_auto_update":
                        app_auto_update = argumentValue;
                        break;
                }
                
            }

            App app = new App();
            app.InitializeComponent();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run();
            
        }
    }
}
