using Microsoft.Win32;
using PureVPN.Installer.Assistance;
using PureVPN.Installer.Helpers;
using PureVPN.Installer.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tally;

namespace PureVPN.Installer.ThirdPartyDependencies
{
    internal class WebView2DependenciesInstaller
    {
        private const string WebView2Version = "98.0.1108.56";

        internal int InstallWebView2Dependencies()
        {
            var dependencyPath = Path.Combine(Common.AppDirectory, Common.WebView2InstallerName);

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(dependencyPath);
                FileInfo fileInfo = new FileInfo(psi.FileName);
                psi.WorkingDirectory = fileInfo.Directory.FullName;
                psi.Arguments = "/silent /install";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process dependencyInstaller = Process.Start(psi);

                while (!dependencyInstaller.HasExited)
                    System.Threading.Thread.Sleep(1000);

                int exitCode = dependencyInstaller.ExitCode;
 
                if (exitCode != 0)
                    return exitCode;

                return 0;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return -1;
            }
        }

        internal bool CheckIfWebView2InstallationIsRequired()
        {
            try
            {
                RegistryKey key;

                if (Environment.Is64BitOperatingSystem)
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", false);
                else
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", false);

                if (key != null)
                {
                    var val = key.GetValue("pv").ToString();

                    var result = val.CompareTo(WebView2Version);

                    if (result < 0)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception e)
            {
                Logger.Log("Error occured while reading the EdgeUpdate\\Client. Reason: " + e.Message);
            }

            return true;
        }
    }
}
