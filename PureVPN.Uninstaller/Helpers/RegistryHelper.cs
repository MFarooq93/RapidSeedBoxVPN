using Microsoft.Win32;
using System;
using System.Linq;
using Tally;

namespace PureVPN.Uninstaller.Helpers
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

        public static string GetAppInstallLocation(string appName)
        {
            string installLoc = "";
            try
            {
                installLoc = GetAppInstallLocation(appName, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

                if (String.IsNullOrEmpty(installLoc))
                    installLoc = GetAppInstallLocation(appName, @"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return installLoc;
        }

        private static string GetAppInstallLocation(string appName, string reg_path)
        {
            try
            {
                var keys = (Registry.LocalMachine).OpenSubKey(reg_path, true).GetSubKeyNames();
                var key = keys.FirstOrDefault(x => x.ToLower().Contains(appName.ToLower()));
                if (!string.IsNullOrEmpty(key))
                {

                    RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(reg_path, true);
                    RegistryKey appKey = hKey.CreateSubKey(key);


                    return appKey.GetValue("InstallLocation").ToString();

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return "";
        }

        private static string GetAppInstallLocationBackup(string appName, string reg_path)
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

        public static void RegisterStartupItem(string appExePath, string appName = "PureVPN")
        {
            try
            {
                RegistryKey homeKey = GetShortcutRegistryKey();

                if (homeKey.GetValue(appName) == null)
                    homeKey.SetValue(appName, appExePath);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void UnRegisterStartupItem(string appName = "PureVPN")
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

        internal static void DeleteUriSchemeInRegistry()
        {
            try
            {
                // Remove keys about URI Scheme for this program
                Registry.ClassesRoot.DeleteSubKeyTree("PureVPN");
            }
            catch (Exception)
            {
            }
        }
    }
}
