using MvvmCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using PureVPN.Installer.ViewModels;
using System.Reflection;
using PureVPN.Installer.Interfaces;
using PureVPN.Installer.Managers;
using System.Diagnostics;
using System.Net;
using Tally;
using PureVPN.Installer.ThirdPartyDependencies;

namespace PureVPN.Installer
{
    public class App
    {
        static public Dispatcher BootstrapperDispatcher { get; private set; }
        static public Window MainWindow { get; private set; }

        public App()
        {
            try { ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            LoadAssemblies();

            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;
            BootstrapperDispatcher.UnhandledException += BootstrapperDispatcher_UnhandledException;
            ViewModelBase.SetNamespace("PureVPNInstaller");
            Configure();
            MainWindow = (IoC.Get<MainViewModel>().View as Window);
            MainWindow.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
            MainWindow.Show();
            Dispatcher.Run();
        }

        void BootstrapperDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e != null && e.Exception != null)
                MessageBox.Show(e.Exception.Message);
            e.Handled = true;
        }

        private static void LoadAssemblies()
        {
            LoadAssembly(Resources.Resources.DotNetZip);
        }

        private static void LoadAssembly(byte[] assembly)
        {
            try { Assembly.Load(assembly); }
            catch (Exception ex) { Console.Write(ex.Message); }
        }

        private void Configure()
        {
            IoC.AddInContainer<MainViewModel>();
            IoC.AddInContainer<AgreementViewModel>();
            IoC.AddInContainer<AdditionalTasksViewModel>();
            IoC.AddInContainer<InstallerViewModel>();
            IoC.AddInContainer<MessageBoxViewModel>();
            IoC.AddInContainer<IWindowManager, WindowManager>();
            IoC.AddInContainer<WebView2DependenciesInstaller>();
        }

        [STAThread]
        static void Main()
        {
            var procList = Process.GetProcesses().Where(x => x.ProcessName == Process.GetCurrentProcess().ProcessName).ToList();
            if (procList.Count > 1)
                Environment.Exit(-1);
            else
                new App();
        }
    }
   
}
