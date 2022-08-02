using Microsoft.Win32;
using PureVPN.Installer.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using Tally;

namespace PureVPN.Installer.Assistance
{
    public class Common
    {
        public static string AppDirectory { get; set; }
        public static string AgreementFilePath { get { return System.IO.Path.Combine(ProgramDataFolderPath, "Terms of conditions.rtf"); } }

        public static string AppVersion
        {
            get
            {
                string version = "6.0.3";
                try
                {
                    Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    version = appVersion.ToString();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return version;
            }
        }

        public static string ProgramDataFolderPath
        {
            get
            {
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "purevpn");
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex) { Logger.Log(ex); }
                return path;
            }
        }

        public static string ProgramFilesFolderPath
        {
            get
            {
                string pathProgramFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.Create), "\\GZ Systems\\PureVPN");

                if (!Directory.Exists(pathProgramFiles))
                    Directory.CreateDirectory(pathProgramFiles);

                return pathProgramFiles;
            }
        }

        public static bool InstallationStarted { get; set; }

        public static bool IsAlreadyInstalled { get { return !String.IsNullOrEmpty(ExistingAppLocation); } }

        public static string ExistingAppLocation
        {
            get
            {
                var existingAppLocation = RegistryHelper.GetAppInstallLocation("PureVPN");
                return existingAppLocation;
            }
        }

        public static string OSName
        {
            get
            {
                try
                {
                    var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    string productName = (string)reg.GetValue("ProductName");
                    return productName;
                }
                catch
                {
                    try
                    {
                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion"))
                        {
                            if (key != null)
                            {
                                var osVersion = key.GetValue("CurrentVersion");
                                return osVersion.ToString();
                            }
                        }

                        var name = (from x in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();
                        return name != null ? name.ToString() : "";
                    }
                    catch (Exception ex) { Logger.Log(ex); }
                }

                return "";
            }
        }

        private static bool? _IsWindows8orAbove;
        public static bool IsWindows8orAbove
        {
            get
            {
                _IsWindows8orAbove = _IsWindows8orAbove ?? (Common.OSName.ToLower().Contains("windows 8") || Common.OSName.ToLower().Contains("windows 10"));
                return _IsWindows8orAbove != null && _IsWindows8orAbove == true;
            }
        }

        public static bool InstallExtensionForChrome { get; set; } = true;

        public static string AppDataRootFolderPath
        {
            get
            {
                string AppDataFullPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string Drive = AppDataFullPath.Substring(0, AppDataFullPath.LastIndexOf("Users"));
                var path = Path.Combine(Drive, "Users");
                return path;
            }
        }

        public static string AppDataSubFolderPath
        {
            get
            {
                var path = System.IO.Path.Combine("AppData", "Local", "GZ_Systems");
                return path;
            }
        }

        #region Third party Dependencies Installer exe names

        internal static string WebView2InstallerName => "MicrosoftEdgeWebview2Setup.exe";

        #endregion
    }
}
