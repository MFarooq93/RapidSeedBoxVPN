using System;
using System.Linq;
using System.Windows.Resources;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using Ionic.Zip;
using Tally;
using Microsoft.Win32;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using PureVPN.Installer.Helpers;
using System.Collections.Generic;

namespace PureVPN.Installer.Assistance
{
    public class Utilities
    {
        public static bool IsProgramInstalled(string program = "Microsoft Visual C++ 2013 Redistributable (x86)")
        {
            var isInstalled = false;
            try
            {
                isInstalled = IsProgramInstalled(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", program);
                if (!isInstalled)
                    isInstalled = IsProgramInstalled(@"SOFTWARE\Classes\Installer\Dependencies", program);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return isInstalled;
        }

        private static bool IsProgramInstalled(string registry_key, string program)
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
                {
                    foreach (string subkey_name in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            if (subkey.GetValue("DisplayName") != null && subkey.GetValue("DisplayName").ToString().Contains(program))
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool DownloadFile(string url, string saveTo = "")
        {
            bool fileDownloaded = false;
            try
            {
                var client = new WebClient();
                var name = url.Split('/').Last();
                var fileName = saveTo.EndsWith(name) ? saveTo : (saveTo + "/" + name);
                var tempFileName = saveTo.EndsWith("temp" + name) ? saveTo : (saveTo + "/" + "temp" + name);
                client.DownloadFile(url, tempFileName);
                File.Copy(tempFileName, fileName, true);
                fileDownloaded = true;
                File.Delete(tempFileName);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return fileDownloaded;
        }

        public static void CreateAppShortcutToDesktop(string shortcutName, string description, string appLaunchPath)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string desktopPath2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                DeleteShortcuts(shortcutName, desktopPath);
                DeleteShortcuts(shortcutName, desktopPath2);
                DeleteShortcuts(shortcutName, @"C:\Users\Public\Desktop");

                IShellLink link = (IShellLink)new ShellLink();

                // setup shortcut information
                link.SetDescription(description);
                link.SetPath(appLaunchPath);

                // save it
                var file = (System.Runtime.InteropServices.ComTypes.IPersistFile)link;

                file.Save(Path.Combine(desktopPath, string.Format("{0}.lnk", shortcutName)), false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void DeleteUninstallShortcutStartMenu(string appLaunchPath)
        {
            try
            {
                string startmenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                var path = Path.Combine(startmenu, "Uninstall PureVPN.lnk");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void DeleteShortcuts(string shortcutName, string directory)
        {
            try
            {
                foreach (var item in Directory.GetFiles(directory))
                {
                    if (item.ToLower().Replace(".lnk", "").EndsWith(shortcutName.ToLower()))
                    {
                        try { File.Delete(item); }
                        catch (Exception ex) { Logger.Log(ex); }
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public static void GrantFullAccessForFile(string path)
        {
            try
            {
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                var fileSec = File.GetAccessControl(path);

                fileSec.AddAccessRule(new FileSystemAccessRule(account.ToString(), FileSystemRights.FullControl, AccessControlType.Allow));
                File.SetAccessControl(path, fileSec);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void ExecuteCommand(string fileName, string workingDirectory = "", string arguments = "")
        {
            try
            {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = fileName;
                if (!String.IsNullOrEmpty(arguments))
                    process.StartInfo.Arguments = arguments;
                process.StartInfo.Verb = "runas";
                if (!String.IsNullOrEmpty(workingDirectory))
                    process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.UseShellExecute = true;

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Logger.Log("Exit Code : " + process.ExitCode.ToString());
                }

                process.Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void EmptyFolder(DirectoryInfo directoryInfo)
        {
            try
            {
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
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

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }

        public static void ExtractZippedFile(string filePath, string folderToExtract = "")
        {
            try
            {
                if (String.IsNullOrEmpty(folderToExtract))
                    folderToExtract = Environment.CurrentDirectory;

                ZipFile zip = ZipFile.Read(filePath);
                zip.ExtractAll(folderToExtract, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public static void ExtractZippedResource(string resourcePath, string folderToExtract = "")
        {
            try
            {
                if (String.IsNullOrEmpty(folderToExtract))
                    folderToExtract = Environment.CurrentDirectory;

                StreamResourceInfo sri = Application.GetResourceStream(new Uri(resourcePath));
                if (sri != null)
                {
                    using (Stream s = sri.Stream)
                    {
                        if (!Directory.Exists(folderToExtract))
                            Directory.CreateDirectory(folderToExtract);

                        ZipFile zip = ZipFile.Read(s);
                        zip.ExtractAll(folderToExtract, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public static bool EraseDirectory(string folderPath, bool recursive)
        {
            //Safety check for directory existence.
            if (!Directory.Exists(folderPath))
                return false;

            foreach (string file in Directory.GetFiles(folderPath))
            {
                File.Delete(file);
            }

            //Iterate to sub directory only if required.
            if (recursive)
            {
                foreach (string dir in Directory.GetDirectories(folderPath))
                {
                    EraseDirectory(dir, recursive);
                }
            }
            //Delete the parent directory before leaving
            Thread.Sleep(1000);
            Directory.Delete(folderPath);
            return true;
        }

        public static bool SetFolderPermission(string pathname, string userRights, SecurityIdentifier userSid, bool inheritSubDirectories)
        {
            if (String.IsNullOrEmpty(pathname))
            {
                return false;
            }

            pathname = pathname.TrimEnd('\\');
            var rights = (FileSystemRights)0;
            if (userRights == "R")
            {
                rights = FileSystemRights.ReadAndExecute;
            }
            else if (userRights == "C")
            {
                rights = FileSystemRights.ChangePermissions;
            }
            else if (userRights == "F")
            {
                rights = FileSystemRights.FullControl;
            }

            NTAccount acct = (NTAccount)userSid.Translate(typeof(NTAccount));

            var accessRule = new FileSystemAccessRule(acct.Value, rights,
                                        InheritanceFlags.None,
                                        PropagationFlags.NoPropagateInherit,
                                        AccessControlType.Allow);
            var info = new DirectoryInfo(pathname);
            var security = info.GetAccessControl(AccessControlSections.Access);
            bool result;
            security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);
            if (!result)
            {
                return false;
            }

            InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
            if (inheritSubDirectories)
            {
                iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            }
            accessRule = new FileSystemAccessRule(acct.Value, rights,
                                        iFlags,
                                        PropagationFlags.InheritOnly,
                                        AccessControlType.Allow);

            security.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);
            if (!result)
            {
                return false;
            }

            info.SetAccessControl(security);
            return true;
        }

        public static void AddChromeExtension()
        {
            try
            {
                if (!Environment.Is64BitOperatingSystem)
                {
                    RegistryKey nkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Google\Chrome", true);
                    if (nkey != null)
                    {
                        nkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Google\Chrome\Extensions", true);
                        if (nkey != null)
                        {
                            RegistryKey newkey = nkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                            newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                            newkey.Close();
                        }
                        else
                        {
                            RegistryKey basekey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Google\Chrome", true);
                            RegistryKey extkey = basekey.CreateSubKey("Extensions");
                            RegistryKey newkey = extkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                            newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                            newkey.Close();
                        }
                    }
                }
                else
                {
                    RegistryKey nkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Google\Chrome", true);

                    if (nkey != null)
                    {
                        nkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Google\Chrome\Extensions", true);

                        if (nkey != null)
                        {
                            RegistryKey newkey = nkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                            newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                            newkey.Close();
                        }
                        else
                        {
                            RegistryKey basekey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Google\Chrome", true);
                            RegistryKey extkey = basekey.CreateSubKey("Extensions");
                            RegistryKey newkey = extkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                            newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                            newkey.Close();

                        }
                    }

                    if (!Common.IsWindows8orAbove)
                    {
                        RegistryKey win7nkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Google\Chrome", true);

                        if (win7nkey != null)
                        {
                            win7nkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Google\Chrome\Extensions", true);

                            if (win7nkey != null)
                            {
                                RegistryKey win7newkey = win7nkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                                win7newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                                win7newkey.Close();
                            }
                            else
                            {
                                RegistryKey win7basekey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Google\Chrome", true);
                                RegistryKey win7extkey = win7basekey.CreateSubKey("Extensions");
                                RegistryKey win7newkey = win7extkey.CreateSubKey("bfidboloedlamgdmenmlbipfnccokknp");
                                win7newkey.SetValue("update_url", "https://clients2.google.com/service/update2/crx");
                                win7newkey.Close();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void CreateScriptToDeleteAppDir(string appPath)
        {
            try
            {
                var scriptPath = Path.Combine(Path.GetTempPath(), "delete_uninstaller.bat");
                try { File.Delete(scriptPath); }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                List<string> commands = new List<string>();
                commands.Add("timeout /t 5");
                commands.Add("rd /s /q " + "\"" + appPath + "\"");
                File.WriteAllLines(scriptPath, commands);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
