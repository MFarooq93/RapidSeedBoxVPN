using PureVPN.LaunchAtomSDKInstaller.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.ThirdPartyInstallerLauncher
{
    public class DependenciesInstaller
    {
        public async Task<int> InstallDependencies()
        {
            return await InatallPureVPN();
        }

        private async Task<int> InatallPureVPN()
        {
            try
            {
                string installerName = Constants.PureVPNInstallerExeName;
                var directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                var path = System.IO.Path.Combine(directory, installerName);

                ProcessStartInfo processStartInfo = new ProcessStartInfo(path);
                Process process = Process.Start(processStartInfo);

                while (!process.HasExited)
                    await Task.Delay(1000);

                return process.ExitCode;
            }
            catch (Exception)
            {
            }

            return -1;
        }
    }
}
