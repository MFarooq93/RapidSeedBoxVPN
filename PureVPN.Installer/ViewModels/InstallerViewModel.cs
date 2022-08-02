using MvvmCore;
using PureVPN.Installer.Helpers;
using PureVPN.Installer.Assistance;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Tally;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using PureVPN.Installer.ThirdPartyDependencies;

namespace PureVPN.Installer.ViewModels
{
    public class InstallerViewModel : BaseViewModel
    {
        private BackgroundWorker installWorker;

        public BackgroundWorker InstallWorker
        {
            get
            {
                if (installWorker == null)
                {
                    installWorker = new BackgroundWorker();
                    installWorker.WorkerReportsProgress = true;
                    installWorker.DoWork += InstallWorker_DoWork;
                    installWorker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;
                    installWorker.ProgressChanged += InstallWorker_ProgressChanged;
                }
                return installWorker;
            }
        }

        private int installProgress;

        public int InstallProgress
        {
            get { return installProgress; }
            set
            {
                installProgress = value;
                NotifyOfPropertyChange(() => InstallProgress);
            }
        }

        private string installerMessage;

        public string InstallerMessage
        {
            get { return installerMessage; }
            set
            {
                installerMessage = value;
                NotifyOfPropertyChange(() => InstallerMessage);
            }
        }

        private void InstallWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InstallProgress = e.ProgressPercentage;
        }

        public void RemoveAdvancedInstaller()
        {
            string uninstallPath = RegistryHelper.GetAdvancedInstaller();

            if (!string.IsNullOrEmpty(uninstallPath))
            {
                int exeIndex = uninstallPath.LastIndexOf("exe");
                string path = uninstallPath.Substring(0, exeIndex + 3);
                string args = uninstallPath.Substring(exeIndex + 4, uninstallPath.Length - exeIndex - 4) + " /qb";

                ProcessStartInfo psi = new ProcessStartInfo(path);
                FileInfo fileInfo = new FileInfo(psi.FileName);
                psi.WorkingDirectory = fileInfo.Directory.FullName;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.Arguments = args;
                Process advancedInstallerPath = System.Diagnostics.Process.Start(psi);

                while (!advancedInstallerPath.HasExited)
                    Thread.Sleep(1000);
            }
        }

        private string appPath;

        public string AppPath
        {
            get
            {
                if (String.IsNullOrEmpty(appPath))
                    appPath = RegistryHelper.GetAppInstallLocation("PureVPN Beta (RV)");
                return appPath;
            }
        }

        private void InstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DisplayMessage("Sit back and relax. Installation begins now!");
            InstallWorker.ReportProgress(0);
            Thread.Sleep(1000);

            // Remove advanced installer
            RemoveAdvancedInstaller();
            InstallWorker.ReportProgress(5);

            var isUpdating = Common.IsAlreadyInstalled;
            string installLoc = isUpdating ? Common.ExistingAppLocation : IoC.Get<AdditionalTasksViewModel>().EnteredPath;

            if (!installLoc.ToLower().Split('\\').Last().Equals("purevpn"))
                installLoc = Path.Combine(installLoc, "PureVPN");

            Common.AppDirectory = installLoc;
            Common.InstallationStarted = true;
            InstallWorker.ReportProgress(10);

            KillProcess(); // Exit application if already running
            InstallWorker.ReportProgress(20);

            while (IsProcessRunning())
                Thread.Sleep(1000);

            DisableTapAdapters();
            StopPureVPNService();
            StopAndRemoveOVPNService();

            DeleteOldInstallationFiles(); // Cleanup files from old path
            InstallWorker.ReportProgress(30);

            DeleteFilesAndDirectories(installLoc); // Cleaup files from new selected location
            InstallWorker.ReportProgress(40);

            DeleteIrrelevantFiles(); // Delete files from program data folder
            InstallWorker.ReportProgress(50);

            // Extract Atom dependencies
            Utilities.ExtractZippedResource(Constants.AtomSDKInstaller, Common.AppDirectory);
            InstallWorker.ReportProgress(55);

            //Exract WebView2Installer dependencies
            Utilities.ExtractZippedResource(Constants.WebView2Installer, Common.AppDirectory);
            InstallWorker.ReportProgress(60);

            // Install Atom dependencies
            string dependencyName = "AtomSDKInstaller.exe";
            var dependencyPath = System.IO.Path.Combine(Common.AppDirectory, dependencyName);
            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(dependencyPath);
            FileInfo fileInfo = new FileInfo(psi.FileName);
            psi.WorkingDirectory = fileInfo.Directory.FullName;
            psi.Arguments = "/verysilent";
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process atomProcess = System.Diagnostics.Process.Start(psi);

            while (!atomProcess.HasExited)
                Thread.Sleep(1000);

            int exitCode = atomProcess.ExitCode;
            int dependencyInstallerErrorCode = 0;

            // Check for WebView2 
            var webView2DependencyInstaller = IoC.Get<WebView2DependenciesInstaller>();

            if (exitCode == 0 && webView2DependencyInstaller.CheckIfWebView2InstallationIsRequired())
                dependencyInstallerErrorCode = webView2DependencyInstaller.InstallWebView2Dependencies();

            if (exitCode != 0 || dependencyInstallerErrorCode != 0)
            {
                App.BootstrapperDispatcher.Invoke(delegate
                {
                    var IsExit = IoC.Get<MessageBoxViewModel>().Show("Installation Failed",
                                                    "There was an error during installation. Please click ‘Read More’ below to solve the problem.",
                                                    true, "Exit", "Read More");

                    if (IsExit == false && exitCode != 0)
                        BrowserHelper.OpenBrowser("", "https://www.purevpn.com/applinks/error-support?source=atom&platform=windows&error_code=5111");
                    else if (IsExit == false && dependencyInstallerErrorCode != 0)
                        BrowserHelper.OpenBrowser("", "https://www.purevpn.com/applinks/error-support?source=app&platform=windows&error_code=9003");

                });

                DisplayMessage("It’s sad to see you leave");

                // Uninstall Atom dependencies
                string atomDependencyFullPath = string.Empty;

                string atomDependencyBasePath = RegistryHelper.GetAppInstallLocation("AtomSDKInstaller_is1");

                if (File.Exists(System.IO.Path.Combine(atomDependencyBasePath, "unins000.exe")))
                    atomDependencyFullPath = System.IO.Path.Combine(atomDependencyBasePath, "unins000.exe");
                else if (File.Exists(System.IO.Path.Combine(atomDependencyBasePath, "unins001.exe")))
                    atomDependencyFullPath = System.IO.Path.Combine(atomDependencyBasePath, "unins001.exe");

                if (!String.IsNullOrEmpty(atomDependencyFullPath))
                {
                    ProcessStartInfo psiExit = new System.Diagnostics.ProcessStartInfo(atomDependencyFullPath);
                    FileInfo fileInfoExit = new FileInfo(psiExit.FileName);
                    psiExit.WorkingDirectory = fileInfo.Directory.FullName;
                    psiExit.CreateNoWindow = true;
                    psiExit.UseShellExecute = false;
                    Process atomProcessExit = System.Diagnostics.Process.Start(psiExit);

                    while (!atomProcessExit.HasExited)
                        Thread.Sleep(1000);
                }

                DeleteFilesAndDirectories(); // Delete files from program files and program data and local data
                InstallWorker.ReportProgress(30);

                // Remove application from registery
                RegistryHelper.RemoveControlPanelProgram("PureVPN Beta (RV)");
                InstallWorker.ReportProgress(10);

                // Delete from startup
                RegistryHelper.UnRegisterStartupItem();

                // Delete shortcut
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                Utilities.DeleteShortcuts("PureVPN Beta (RV)", desktopPath);

                // Delete parent folders
                Utilities.CreateScriptToDeleteAppDir(AppPath);
                Thread.Sleep(2000);

                return;
            }

            // Extract Program Files
            Utilities.ExtractZippedResource(Constants.ProgramFileAssetsZip, Common.AppDirectory);
            InstallWorker.ReportProgress(60);

            // Extract local data file
            Utilities.ExtractZippedResource(Constants.LocalDataZip, Common.AppDirectory);
            InstallWorker.ReportProgress(63);

            // Extract Uninstaller
            Utilities.ExtractZippedResource(Constants.UninstallSetupZip, Common.AppDirectory + "\\Uninstaller");
            InstallWorker.ReportProgress(70);

            RegistryHelper.RemoveControlPanelProgram("PureVPN"); // Remove old app from registry
            InstallWorker.ReportProgress(80);

            // Register new app
            var displayIcon = System.IO.Path.Combine(installLoc, "PureVPN.exe");
            var uninstallerLoc = System.IO.Path.Combine(installLoc, @"Uninstaller\Uninstaller.exe");
            RegistryHelper.RegisterControlPanelProgram("PureVPN", "GZ Systems", installLoc, displayIcon, Common.AppVersion, uninstallerLoc);
            InstallWorker.ReportProgress(90);

            InstallVCPIfRequired(); // Install Visual C++
            InstallWorker.ReportProgress(100);

            //Delete URI scheme 
            RegistryHelper.DeleteUriSchemeInRegistry();

            //Register URI scheme
            RegistryHelper.AddUriSchemeInRegistry(Common.AppDirectory);

            var appPath = System.IO.Path.Combine(Common.AppDirectory, "PureVPN.exe");

            // Create app shortcut
            Utilities.CreateAppShortcutToDesktop("PureVPN", string.Format("Location: {0}", appPath), appPath);

            // Delete unistaller shortcut
            Utilities.DeleteUninstallShortcutStartMenu(uninstallerLoc);

            // Delete Atom dependency installer
            try
            {
                File.Delete(dependencyPath);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            // Set fulll permissions to Program Files Path
            Utilities.SetFolderPermission(installLoc, "F", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), true);

            // Set full permissions to Program Data Path
            Utilities.SetFolderPermission(Common.ProgramDataFolderPath, "F", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), true);

            // Install chrome extensions
            if (Common.InstallExtensionForChrome)
                Utilities.AddChromeExtension();

            // Delay launch
            Thread.Sleep(1000);

            //Copy Pure Installer in installed directory
            CopyPureInstallerToInstalationDirectory();

            // Start application
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer.exe";
            info.WorkingDirectory = Common.AppDirectory;
            info.Arguments = isUpdating ? Path.Combine(Common.AppDirectory, "PureVPN.Launcher.exe") : appPath;
            Process.Start(info);
        }

        private void InstallVCPIfRequired()
        {
            #region 32bit VC++
            var isVCRuntimeInstalled = Utilities.IsProgramInstalled("Microsoft Visual C++ 2013 Redistributable (x86)");
            if (!isVCRuntimeInstalled)
            {
                DisplayMessage("Downloading and Installing pre-requisites");
                InstallWorker.ReportProgress(85);
                bool isfileDownloaded = Utilities.DownloadFile(Constants.VCRedistUrl86, Common.ProgramDataFolderPath);
                InstallWorker.ReportProgress(90);
                if (isfileDownloaded)
                {
                    var path = Path.Combine(Common.ProgramDataFolderPath, Constants.VCRedistExe86);
                    Utilities.ExecuteCommand(path, "", "/install /quiet /norestart");
                }
            }
            #endregion

            #region 64bit VC++
            isVCRuntimeInstalled = Utilities.IsProgramInstalled("Microsoft Visual C++ 2013 Redistributable (x64)");
            if (!isVCRuntimeInstalled)
            {
                DisplayMessage("Downloading and Installing pre-requisites");
                InstallWorker.ReportProgress(85);
                bool isfileDownloaded = Utilities.DownloadFile(Constants.VCRedistUrl64, Common.ProgramDataFolderPath);
                InstallWorker.ReportProgress(90);
                if (isfileDownloaded)
                {
                    var path = Path.Combine(Common.ProgramDataFolderPath, Constants.VCRedistExe64);
                    Utilities.ExecuteCommand(path, "", "/install /quiet /norestart");
                }
            }
            #endregion
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

        private bool IsProcessRunning()
        {
            try
            {
                var processes = Process.GetProcessesByName("PureVPN").ToList();

                if (processes.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return false;
        }

        private static void DisableTapAdapters()
        {
            try
            {
                var adapters = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.Description.ToLower().Contains("tap")).ToList();
                if (adapters != null)
                {
                    foreach (var item in adapters)
                    {
                        Utilities.ExecuteCommand("netsh", "", string.Format("interface set interface \"{0}\" disable", item.Name));
                    }
                }
            }
            catch { }
        }

        private static void StopPureVPNService()
        {
            try
            {
                string serviceName = "PureVPNService";
                ServiceHelper.StopService(serviceName);
                ServiceHelper.RemoveService(serviceName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static void StopAndRemoveOVPNService()
        {
            ServiceHelper.StopService("OpenVpnService");
            ServiceHelper.StopService("OpenVPNService");
            ServiceHelper.StopService("OpenVPNServiceInteractive");
            ServiceHelper.StopService("OpenVPNServiceLegacy");
            ServiceHelper.StopService("ovpnagent");
            ServiceHelper.RemoveService("OpenVPNService");
            ServiceHelper.RemoveService("OpenVpnService");
        }


        private void DeleteOldInstallationFiles()
        {
            DeleteFilesAndDirectories(Common.ExistingAppLocation);
        }

        private void DeleteFilesAndDirectories(string dirPath)
        {
            try
            {
                foreach (var item in Directory.GetDirectories(Common.ProgramDataFolderPath))
                {
                    try
                    {
                        Directory.Delete(item, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                foreach (var item in Directory.GetFiles(Common.ProgramDataFolderPath))
                {
                    try
                    {
                        if (!item.Contains("fav.json") && !item.Contains("recent.json"))
                            File.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            if (!String.IsNullOrEmpty(dirPath))
            {
                try
                {
                    Directory.GetFiles(dirPath).ToList().ForEach(x =>
                    {
                        try
                        {
                            //if (FilesToDelete.FirstOrDefault(y => x.ToLower().Contains(y)) != null)
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

                try
                {
                    Directory.GetDirectories(dirPath).ToList().ForEach(x =>
                    {
                        try
                        {
                            //if (DirsToDelete.FirstOrDefault(y => x.ToLower().Equals(y.ToLower())) != null)
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
            }
        }

        private void DisplayMessage(string message)
        {
            ExecuteOnUIThread(() => InstallerMessage = message);
        }

        private void InstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseApp();
        }

        protected override void OnViewLoaded(object view)
        {
            InstallWorker.RunWorkerAsync();
        }

        private static void DeleteIrrelevantFiles()
        {
            try
            {
                var files = Directory.GetFiles(Common.ProgramDataFolderPath).ToList().Where(x =>
                (x.ToLower().EndsWith(".txt") && !x.ToLower().Contains("debuglog")) ||
                x.ToLower().EndsWith(".tmp") ||
                x.ToLower().EndsWith("commands") ||
                x.ToLower().EndsWith("entry") ||
                x.ToLower().EndsWith("file") ||
                x.ToLower().EndsWith("events") ||
                x.ToLower().EndsWith("interfaces")
                ).ToList();

                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            Utilities.GrantFullAccessForFile(file);
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

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

        private void CopyPureInstallerToInstalationDirectory()
        {
            try
            {
                var pureInstallerFileDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                var pureInstallerName = AppDomain.CurrentDomain.FriendlyName;
                var pureInstallerFileDirectoryWithName = Path.Combine(pureInstallerFileDirectory, pureInstallerName);
                var installationDirectory = Path.Combine(Common.AppDirectory, pureInstallerName);

                File.Copy(pureInstallerFileDirectoryWithName, installationDirectory, true);
            }
            catch { /*No Need*/}
        }

    }
}