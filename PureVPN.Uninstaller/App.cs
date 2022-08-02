using MvvmCore;
using PureVPN.Uninstaller.Interfaces;
using PureVPN.Uninstaller.Managers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using PureVPN.Uninstaller.ViewModels;

namespace PureVPN.Uninstaller
{
    public class App
    {
        static public Dispatcher BootstrapperDispatcher { get; private set; }
        static public Window MainWindow { get; private set; }

        public App(string[] args)
        {
            LoadAssemblies();
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;
            ViewModelBase.SetNamespace("Uninstaller");
            Configure();
            MainWindow = (IoC.Get<MainViewModel>().View as Window);
            MainWindow.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
            MainWindow.Show();
            Dispatcher.Run();
        }

        private static void LoadAssemblies()
        {
            try { Assembly.Load(Properties.Resources.DotNetZip); }
            catch (Exception ex) { Console.Write(ex.Message); }
            try { Assembly.Load(Properties.Resources.XamlAnimatedGif); }
            catch (Exception ex) { Console.Write(ex.Message); }
        }

        private void Configure()
        {
            IoC.AddInContainer<MainViewModel>();
            IoC.AddInContainer<ConfirmationViewModel>();
            IoC.AddInContainer<UnInstallerViewModel>();
            IoC.AddInContainer<MessageBoxViewModel>();
            IoC.AddInContainer<IWindowManager, WindowManager>();
        }

        [STAThread]
        static void Main(string[] args)
        {
            var procList = Process.GetProcesses().Where(x => x.ProcessName == Process.GetCurrentProcess().ProcessName).ToList();
            if (procList.Count > 1)
                Environment.Exit(-1);
            else
                new App(args);
        }
    }
}
