using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Installer.Helpers
{
    class BrowserHelper
    {
        public static void OpenBrowser(string browserName = "", string link = "")
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var browser = GetBrowserPath(browserName);
                    if (browser.Contains("microsoft-edge"))
                        Process.Start("microsoft-edge:" + link);
                    else
                        Process.Start(new ProcessStartInfo(browser, link == null ? "" : link));
                });
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        private static string GetBrowserPath(string browserName = "")
        {
            try
            {
                if (!String.IsNullOrEmpty(browserName))
                {

                    var names = browserName.Split(' ');

                    foreach (var item in names)
                    {
                        object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + item + ".exe", "", null);

                        string installed_path = ExtractPathFromRegistry(path);
                        if (!String.IsNullOrEmpty(installed_path))
                            return installed_path;

                        path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + item + ".exe", "", null);
                        installed_path = ExtractPathFromRegistry(path);
                        if (!String.IsNullOrEmpty(installed_path))
                            return installed_path;

                        installed_path = GetComplexPath(item);
                        if (!String.IsNullOrEmpty(installed_path))
                            return installed_path;
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }

            return DefaultBrowserPath;
        }

        private static string GetComplexPath(string item)
        {
            try
            {
                object path = null;
                string installed_path = "";
                var subkeynames = Registry.CurrentUser.OpenSubKey("SOFTWARE").GetSubKeyNames();
                if (subkeynames != null)
                {
                    var subkey = subkeynames.Where(x => x.ToLower().Contains(item.ToLower())).FirstOrDefault();
                    if (subkey != null)
                    {
                        var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + subkey);

                        if (key.ValueCount != 0)
                        {
                            var val = key.GetValueNames().FirstOrDefault();

                            if (!String.IsNullOrEmpty(val))
                            {
                                path = Registry.GetValue(key.Name, val, null);
                                installed_path = ExtractPathFromRegistry(path);
                                if (!String.IsNullOrEmpty(installed_path))
                                    return installed_path;
                            }
                        }

                        subkeynames = key.GetSubKeyNames();
                        subkey = subkeynames.Where(x => !String.IsNullOrEmpty(x)).ToList().FirstOrDefault();
                        if (subkey != null)
                        {
                            string m = key.Name.Split('\\').Last();
                            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + m + @"\" + subkey);
                            var val = key.GetValueNames().FirstOrDefault();

                            if (!String.IsNullOrEmpty(val))
                            {
                                path = Registry.GetValue(key.Name, val, null);
                                installed_path = ExtractPathFromRegistry(path);
                                if (!String.IsNullOrEmpty(installed_path))
                                    return installed_path;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }

            return null;
        }

        private static string ExtractPathFromRegistry(object path)
        {
            try
            {
                if (path != null)
                {
                    path = path.ToString().Replace("\"", "");
                    if (File.Exists(path.ToString()))
                        return path.ToString();
                }
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
            return null;
        }

        private static string DefaultBrowserPath
        {
            get
            {
                string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
                string browserPathKey = @"$BROWSER$\shell\open\command";

                RegistryKey userChoiceKey = null;
                string browserPath = "";

                try
                {
                    userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                    if (userChoiceKey == null)
                    {
                        var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                        if (browserKey == null)
                        {
                            browserKey =
                            Registry.CurrentUser.OpenSubKey(
                            urlAssociation, false);
                        }
                        var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                        browserKey.Close();
                        return path;
                    }
                    else
                    {
                        // user defined browser choice was found
                        string progId = (userChoiceKey.GetValue("ProgId").ToString());
                        userChoiceKey.Close();
                        if (progId.Contains("AppX"))
                        {
                            return "microsoft-edge://";
                        }
                        // now look up the path of the executable
                        string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                        var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                        browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                        kp.Close();
                        return browserPath;
                    }
                }
                catch (Exception ex)
                {
                    //Logger.Log(ex);
                }
                return "";
            }
        }

        private static string CleanifyBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }
    }
}
