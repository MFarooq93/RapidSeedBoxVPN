using PureVPN.Uninstaller.Assistance;
using PureVPN.Uninstaller.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Tally;

namespace PureVPN.Uninstaller.ViewModels
{
    public class UnInstallerViewModel : BaseViewModel
    {
        private BackgroundWorker uninstallWorker;

        public BackgroundWorker UninstallWorker
        {
            get
            {
                if (uninstallWorker == null)
                {
                    uninstallWorker = new BackgroundWorker();
                    uninstallWorker.DoWork += UninstallWorker_DoWork;
                    uninstallWorker.RunWorkerCompleted += UninstallWorker_RunWorkerCompleted;
                    uninstallWorker.WorkerReportsProgress = true;
                    uninstallWorker.ProgressChanged += UninstallWorker_ProgressChanged;
                }
                return uninstallWorker;
            }
        }

        private int uninstallProgress;

        public int UninstallProgress
        {
            get { return uninstallProgress; }
            set
            {
                uninstallProgress = value;
                NotifyOfPropertyChange(() => UninstallProgress);
            }
        }

        private string uninstallerMessage;

        public string UninstallerMessage
        {
            get { return uninstallerMessage; }
            set
            {
                uninstallerMessage = value;
                NotifyOfPropertyChange(() => UninstallerMessage);
            }
        }

        void UninstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DisplayMessage("It’s sad to see you leave");

            KillProcess(); // Exit application
            UninstallWorker.ReportProgress(10);

            // Uninstall Atom dependencies
            string atomDependencyFullPath = string.Empty;

            string atomDependencyBasePath = RegistryHelper.GetAppInstallLocation("AtomSDKInstaller_is1");

            if(File.Exists(System.IO.Path.Combine(atomDependencyBasePath, "unins000.exe")))
                atomDependencyFullPath = System.IO.Path.Combine(atomDependencyBasePath, "unins000.exe");
            else if (File.Exists(System.IO.Path.Combine(atomDependencyBasePath, "unins001.exe")))
                atomDependencyFullPath = System.IO.Path.Combine(atomDependencyBasePath, "unins001.exe");

            if (!String.IsNullOrEmpty(atomDependencyFullPath))
            {
                ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(atomDependencyFullPath);
                FileInfo fileInfo = new FileInfo(psi.FileName);
                psi.WorkingDirectory = fileInfo.Directory.FullName;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process atomProcess = System.Diagnostics.Process.Start(psi);

                while (!atomProcess.HasExited)
                    Thread.Sleep(1000);
            }

            DeleteFilesAndDirectories(); // Delete files from program files and program data and local data
            UninstallWorker.ReportProgress(70);

            // Remove application from registery
            RegistryHelper.RemoveControlPanelProgram("PureVPN");
            UninstallWorker.ReportProgress(100);
            RegistryHelper.DeleteUriSchemeInRegistry();

            // Delete from startup
            RegistryHelper.UnRegisterStartupItem();

            // Delete shortcut
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Utilities.DeleteShortcuts("PureVPN", desktopPath);

            // Delete parent folders
            Utilities.CreateScriptToDeleteAppDir(AppPath);
            Thread.Sleep(2000);
        }

        private void KillProcess()
        {
            try
            {
                var processes = Process.GetProcessesByName("PureVPN").ToList();
                processes.ForEach(x =>
                {
                    try { x.Kill(); }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        string[] FilesToDelete = 
        {
            "purevpn",
            "anatomizer",
            "mixpanel",
            "tally",
            "lib",
            "msvc",
            "split",
            "updater",
            "uninstaller",
            "newtonsoft",
            "enumeration",
            "dotras",
            "restart",
            "wpf",
            "aws",
            "dotnet",
            "ics",
            "install",
            "interop",
            "ionic",
            "microsoft",
            "speed",
            "system",
            "tally"
        };

        string[] DirsToDelete = 
        {
            "bin",
            "driver",
            "purevpnservice"
        };

        private void DeleteFilesAndDirectories()
        {
            try { Directory.Delete(Common.ProgramDataFolderPath, true); }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            if (!String.IsNullOrEmpty(AppPath))
            {
                try
                {
                    Directory.GetDirectories(AppPath).ToList().ForEach(x =>
                    {
                        try
                        {
                            Directory.Delete(x, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                try
                {
                    Directory.GetFiles(AppPath).ToList().ForEach(x =>
                    {
                        try
                        {
                            File.Delete(x);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            if (!String.IsNullOrEmpty(Common.AppDataRootFolderPath))
            {
                try
                {
                    Directory.GetDirectories(Common.AppDataRootFolderPath).ToList().ForEach(x =>
                    {
                        try
                        {
                            string FullDirecotryPath = Path.Combine(x, "AppData", "Local", "GZ_Systems");
                            if (Directory.Exists(FullDirecotryPath))
                                Directory.Delete(FullDirecotryPath, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

        }

        private void DisplayMessage(string message)
        {
            ExecuteOnUIThread(() => UninstallerMessage = message);
        }

        private void UninstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utilities.ExecuteCommand(Path.Combine(Path.GetTempPath(), "delete_uninstaller.bat"), waitForExit: false);
            CloseApp();
        }

        void UninstallWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UninstallProgress = e.ProgressPercentage;
        }

        private string appPath;

        public string AppPath
        {
            get
            {
                if (String.IsNullOrEmpty(appPath))
                    appPath = RegistryHelper.GetAppInstallLocation("PureVPN");
                return appPath;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            UninstallWorker.RunWorkerAsync();
        }


    }
}
