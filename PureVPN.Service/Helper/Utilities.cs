using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PureVPN.Core.Models;
using PureVPN.Service.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PureVPN.Service.Helper
{
    public class Utilities
    {
        private IDialerToServerService _dialerToServerService;
        public Utilities(IDialerToServerService dialerToServerService)
        {
            _dialerToServerService = dialerToServerService;
        }
        public static string GetMD5(string textToEncrypt)
        {
            try
            {
                byte[] asciiBytes = Encoding.ASCII.GetBytes(textToEncrypt);
                byte[] hashedBytes = System.Security.Cryptography.MD5.Create().ComputeHash(asciiBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
                return "";
            }
        }

        public static string GetHardwareId()
        {
            try
            {
                var id = GetHashedMacAddress();

                if (String.IsNullOrEmpty(id))
                    id = GetHashedCpuId();

                if (String.IsNullOrEmpty(id))
                    return "-2";

                return id;
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
                return "-2";
            }
        }

        private static string GetHashedMacAddress()
        {
            try
            {
                var mac = GetMacAddress();

                if (String.IsNullOrEmpty(mac))
                    return "";

                return GetMD5(mac);
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
                return "";
            }
        }


        public static string GetMacAddress()
        {
            var mac = "";
            try
            {
                mac =
                NetworkInterface.GetAllNetworkInterfaces().Where(x =>
                x.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                !x.Description.ToLower().Contains("wan miniport") &&
                !x.Description.ToLower().Contains("purevpn") &&
                !x.Description.ToLower().Contains("tap-windows adapter v9") &&
                !x.Description.ToLower().Contains("vmware")
                )
                .Select(n => n.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")))
                .Select(macBytes => string.Join("", macBytes))
                .FirstOrDefault()
                ;
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }


            return mac;
        }
        private static string GetHashedCpuId()
        {
            try
            {
                var mc = new ManagementClass("win32_processor");

                foreach (var mo in mc.GetInstances())
                {
                    if (mo != null && mo.Properties["processorID"] != null)
                    {
                        var id = mo.Properties["processorID"].Value.ToString();
                        return GetMD5(id);
                    }
                }
            }
            catch (Exception ex)
            {
                // Service.Helper.Sentry.LoggingException(ex);
            }

            return "";
        }


        public async static Task<bool> IsInternetAvailable()
        {
            if (await CheckHttpTraffic("https://www.google.com"))
                return true;

            if (await CheckHttpTraffic("http://captive.apple.com"))
                return true;

            if (await CheckHttpTraffic("https://www.Baidu.com"))
                return true;

            return false;
        }

        public async static Task<bool> CheckHttpTraffic(string url)
        {
            try
            {
                var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(4);
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                // Service.Helper.Sentry.LoggingException(ex);
                return false;
            }
        }

        public static void RegisterProgram(string appName, string version)
        {
            try
            {
                string Install_Reg_Loc = @"SOFTWARE";
                if (!Environment.Is64BitOperatingSystem)
                    Install_Reg_Loc = @"Software";

                RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(Install_Reg_Loc, true);
                RegistryKey appKey = hKey.CreateSubKey(appName);
                appKey.SetValue("Version", (object)version, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
        }

        public static void RemoveProgram(string appName)
        {
            string InstallerRegLoc1 = @"Software";
            string InstallerRegLoc2 = @"SOFTWARE";

            try
            {
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc1, true);
                RegistryKey appSubKey = homeKey.OpenSubKey(appName);
                if (null != appSubKey)
                    homeKey.DeleteSubKey(appName);
            }
            catch (Exception ex)
            {
                Service.Helper.Sentry.LoggingException(ex);
            }

            try
            {
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc2, true);
                RegistryKey appSubKey = homeKey.OpenSubKey(appName);
                if (null != appSubKey)
                    homeKey.DeleteSubKey(appName);
            }
            catch (Exception ex)
            {
                Service.Helper.Sentry.LoggingException(ex);
            }
        }

        public static string GetLastAppVersion(string appName)
        {
            string installLoc = "";
            try
            {
                installLoc = GetLastAppVersion(appName, @"SOFTWARE");

                if (String.IsNullOrEmpty(installLoc))
                    installLoc = GetLastAppVersion(appName, @"Software");
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
            return installLoc;
        }

        private static string GetLastAppVersion(string appName, string reg_path)
        {
            try
            {
                RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(reg_path, true);
                RegistryKey appKey = hKey.CreateSubKey(appName);
                return appKey.GetValue("Version").ToString();
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
            return "";
        }


        /// <summary>
        /// Gets <see cref="UserInfo"/> from JWT
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="encodedInfo"></param>
        /// <returns></returns>
        internal static UserInfo ParseUserInfo(string accessToken, out string userInfoJson)
        {
            UserInfo userInfo = null;
            userInfoJson = null;
            try
            {
                var encodedInfo = accessToken?.Split('.')?.ElementAtOrDefault(1);
                if (encodedInfo.IsStringNotNullorEmptyorWhitespace())
                {
                    encodedInfo = FixBase64StringLength(encodedInfo);

                    userInfoJson = Encoding.ASCII.GetString(Convert.FromBase64String(encodedInfo));
                    var jwt = JsonConvert.DeserializeObject<JWTModel>(userInfoJson);
                    userInfo = jwt?.user;
                }
            }
            catch { }
            return userInfo;
        }

        /// <summary>
        /// Appends <see cref="="/> to <paramref name="encodedInfo"/> to a valid length
        /// </summary>
        /// <param name="encodedInfo"></param>
        /// <returns></returns>
        private static string FixBase64StringLength(string encodedInfo)
        {
            /*
             * The length of a base64 encoded string is always a multiple of 4.
             * If it is not a multiple of 4, then = characters are appended until it is.
             */

            var multiplier = 4;
            var missingLength = encodedInfo.Length % multiplier;
            if (missingLength > default(int))
            {
                encodedInfo += new string('=', multiplier - missingLength);
            }

            return encodedInfo;
        }

        internal class JWTModel
        {
            public UserInfo user { get; set; }
        }
    }
}
