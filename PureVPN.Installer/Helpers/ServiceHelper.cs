using Enumerations;
using PureVPN.Installer.Assistance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Installer.Helpers
{
    public static class ServiceHelper
    {
        public static void RemoveService(string serviceName)
        {
            try
            {
                string SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "purevpn");
                ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                InstallContext Context = new InstallContext();
                ServiceInstallerObj.Context = Context;
                ServiceInstallerObj.ServiceName = serviceName;
                ServiceInstallerObj.Uninstall(null);
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        public static void StopService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status != ServiceControllerStatus.Stopped)
                    service.Stop();
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        public static bool InstallService(string serviceName)
        {
            try
            {
                //Logger.Log("Installing " + serviceName);
                string[] commandLineOptions = new string[1] { string.Format("/LogFile={0}", Path.Combine(Common.ProgramDataFolderPath, "debuglog.txt")) };
                serviceName = serviceName.Contains(".exe") ? serviceName : serviceName + ".exe";

                System.Configuration.Install.AssemblyInstaller installer = new System.Configuration.Install.AssemblyInstaller(serviceName, null);
                installer.UseNewContext = true;
                installer.Install(null);
                installer.Commit(null);
                //Logger.Log(serviceName.Replace(".exe", "") + " Installed");
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
            return false;
        }

        public static void AssignRightsToService(string serviceName, bool toUserOnly = false)
        {
            try
            {
                //Logger.Log("Assigning Access Rights for " + serviceName + (toUserOnly ? " for User" : ""));

                string user = !toUserOnly ? "everyone" : Environment.UserName;
                //string AppDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                string subinaclFile = System.IO.Path.Combine(Common.AppDirectory, "bin", "subinacl.exe");
                string subInaclWorkingDir = System.IO.Path.Combine(Common.AppDirectory, "bin");

                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = subinaclFile;
                string Argument = "/SERVICE \"" + serviceName + "\" /GRANT=" + user + "=F";
                process.StartInfo.Arguments = Argument;
                process.StartInfo.Verb = "runas";
                process.StartInfo.WorkingDirectory = subInaclWorkingDir;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardInput = false;
                process.StartInfo.RedirectStandardError = false;
                process.EnableRaisingEvents = false;

                process.Start();
                process.WaitForExit();

                //if (process.ExitCode != 0)
                //{
                //    Logger.Log("Exit Code : " + process.ExitCode.ToString());
                //}

                process.Close();
                //Logger.Log("Rights elevation for " + serviceName + " via subinacl... completed");
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        public static void StartService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status != ServiceControllerStatus.Running)
                    service.Start();
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        public static void ChangeServiceStatus(string serviceName, ServiceStartMode serviceStartMode)
        {
            try
            {
                StopService(serviceName);
                var svc = new ServiceController(serviceName);
                if (svc != null)
                    ChangeStartMode(svc, serviceStartMode);
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        public static void Execute(this ServiceCommand command, string serviceName = "PureVPNService")
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                if (service.Status == ServiceControllerStatus.Stopped)
                    service.Start();

                while (service.Status != ServiceControllerStatus.Running)
                    service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Running)
                    service.ExecuteCommand((int)command);
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }

        }


        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean ChangeServiceConfig(
            IntPtr hService,
            UInt32 nServiceType,
            UInt32 nStartType,
            UInt32 nErrorControl,
            String lpBinaryPathName,
            String lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            String lpServiceStartName,
            String lpPassword,
            String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(
            IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);

        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

        public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Manager Error");
            }

            var serviceHandle = OpenService(
                scManagerHandle,
                svc.ServiceName,
                SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Error");
            }

            var result = ChangeServiceConfig(
                serviceHandle,
                SERVICE_NO_CHANGE,
                (uint)mode,
                SERVICE_NO_CHANGE,
                null,
                null,
                IntPtr.Zero,
                null,
                null,
                null,
                null);

            if (result == false)
            {
                int nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException("Could not change service start type: " + win32Exception.Message);
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }
    }
}
