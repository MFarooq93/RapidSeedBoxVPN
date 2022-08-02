using Microsoft.Win32;
using System;
using System.Linq;
using Tally;

namespace PureVPN.Installer.Helpers
{
    public class RegistryHelper
    {
        public static void RegisterControlPanelProgram(string appName, string publisher, string installLocation, string displayicon, string displayVersion, string uninstallString)
        {
            try
            {
                string Install_Reg_Loc = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                if (!Environment.Is64BitOperatingSystem)
                    Install_Reg_Loc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

                RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(Install_Reg_Loc, true);
                RegistryKey appKey = hKey.CreateSubKey(appName);

                appKey.SetValue("DisplayName", (object)appName, RegistryValueKind.String);
                appKey.SetValue("Publisher", (object)publisher, RegistryValueKind.String);
                appKey.SetValue("InstallLocation", (object)installLocation, RegistryValueKind.ExpandString);
                appKey.SetValue("DisplayIcon", (object)displayicon, RegistryValueKind.String);
                appKey.SetValue("UninstallString", (object)uninstallString, RegistryValueKind.ExpandString);
                appKey.SetValue("DisplayVersion", (object)displayVersion, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static string GetAppInstallLocation(string appName, string keyName = "InstallLocation")
        {
            try
            {
                string path = GetAppInstallLocation(appName, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", keyName);

                if (String.IsNullOrEmpty(path))
                    path = GetAppInstallLocation(appName, @"Software\Microsoft\Windows\CurrentVersion\Uninstall", keyName);

                return path.TrimEnd('\\');
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        public static string GetAdvancedInstaller()
        {
            try
            {
                string appName = "PureVPN Beta (RV) ";
                string keyName = "UninstallString";

                string path = GetAdvancedInstaller(appName, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", keyName);

                if (String.IsNullOrEmpty(path))
                    path = GetAdvancedInstaller(appName, @"Software\Microsoft\Windows\CurrentVersion\Uninstall", keyName);

                return path;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        private static string GetAdvancedInstaller(string appName, string reg_path, string keyName)
        {
            try
            {
                var keys = (Registry.LocalMachine).OpenSubKey(reg_path, true).GetSubKeyNames();
                var key = keys.FirstOrDefault(x => x.ToLower().Contains(appName.ToLower()));
                if (!string.IsNullOrEmpty(key))
                {

                    RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(reg_path, true);
                    RegistryKey appKey = hKey.CreateSubKey(key);

                    return appKey.GetValue(keyName).ToString();

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        private static string GetAppInstallLocation(string appName, string reg_path, string keyName)
        {
            try
            {
                RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(reg_path, true);
                RegistryKey appKey = hKey.CreateSubKey(appName);

                return appKey.GetValue("InstallLocation").ToString();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        public static void RemoveControlPanelProgram(string applicationName)
        {
            string InstallerRegLoc1 = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
            string InstallerRegLoc2 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc1, true);
                RegistryKey appSubKey = homeKey.OpenSubKey(applicationName);
                if (null != appSubKey)
                    homeKey.DeleteSubKey(applicationName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            try
            {
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc2, true);
                RegistryKey appSubKey = homeKey.OpenSubKey(applicationName);
                if (null != appSubKey)
                    homeKey.DeleteSubKey(applicationName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static RegistryKey ShortcutRegistryEntry32Bit
        {
            get
            {
                return (Registry.CurrentUser).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            }
        }

        private static RegistryKey ShortcutRegistryEntry64Bit
        {
            get
            {
                return (Registry.CurrentUser).OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", true);
            }
        }

        public static string GetWindowsReleaseVersion()
        {
            try
            {
                RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true);
                return hKey.GetValue("ReleaseId").ToString();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        public static void UnRegisterStartupItem(string appName = "PureVPN Beta (RV)")
        {
            try
            {
                RegistryKey homeKey = GetShortcutRegistryKey();

                if (homeKey.GetValue(appName) != null)
                    homeKey.DeleteValue(appName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static RegistryKey GetShortcutRegistryKey()
        {
            string Install_Reg_Loc = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run";
            if (!Environment.Is64BitOperatingSystem)
                Install_Reg_Loc = @"Software\Microsoft\Windows\CurrentVersion\Run";

            RegistryKey homeKey = (Registry.CurrentUser).OpenSubKey(Install_Reg_Loc, true);
            return homeKey;
        }

        internal static void AddUriSchemeInRegistry(string path)
        {
            try
            {
                path = path + "\\PureVPN.exe";

                // Create URI Scheme
                RegistryKey scheme = Registry.ClassesRoot.CreateSubKey("PureVPN");
                scheme.SetValue("", "URL:PureVPN Protocol");
                scheme.SetValue("URL Protocol", "");

                //Create DefaultIcon section
                var defaultIconCmd = scheme.CreateSubKey("DefaultIcon");
                defaultIconCmd.SetValue("", $"{path}, 1");

                // Create URI Scheme's action
                var command = scheme.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                command.SetValue("", $"{path} \"%1\"");
            }
            catch (Exception ex)
            {

            }
            
        }

        internal static void DeleteUriSchemeInRegistry()
        {
            try
            {
                // Remove keys about URI Scheme for this program
                Registry.ClassesRoot.DeleteSubKeyTree("PureVPN");
            }
            catch (Exception ex)
            {
            }
        }
    }

}
