using Microsoft.Win32;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Handlers;
using PureVPN.Core.Interfaces;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PureVPN.Service.Helper
{
    public class Common
    {
        public static string ExeName { get { return "PureVPN"; } }

        public static string ExePath
        {
            get
            {
                return Path.GetFullPath(new Uri($@"{ExeDirectory}\{ExeName}.exe").LocalPath);
            }
        }

        public static string ExeDirectory
        {
            get
            {
                return $@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)}";
            }
        }

        public static string ProgramDataFolderPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "purevpn");
            }
        }

        public static string AppUpdaterFullName
        {
            get
            {
                return Path.GetFullPath((new Uri(System.IO.Path.Combine(ExeDirectory, AppUpdaterExe)).LocalPath));
            }
        }

        public static string AppUpdaterExe
        {
            get
            {
                return "Updater.exe";
            }
        }

        public static string ProductVersion { get; set; }
        public static string ProductFileVersion { get; set; }
        public static string ProductBuildVersion { get; set; }
        public static string ReleaseType { get; set; }
        public static string ReleaseTypeCode { get; set; }
        public static bool MuteTracker { get; set; } = false;
        /// <summary>
        /// Culture Info code for selected app language
        /// </summary>
        public static string AppLanguage { get; set; }
        /// <summary>
        /// App Language code for Web APIs for selected app language
        /// </summary>
        public static string AppLanguageLocale { get { return AppLanguage?.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault(); } }

        public static string Environment { get; set; }
        private static string _osName;
        public static string OSName
        {
            get { _osName = string.IsNullOrEmpty(_osName) ? GetOSName() : _osName; return _osName; }

        }

        private static string GetOSName()
        {
            _osName = GetOSNamebyMOS();
            if (!string.IsNullOrEmpty(GetOSNamebyMOS()))
                return _osName;

            _osName = GetOSNameByProductNameKey();
            if (!string.IsNullOrEmpty(_osName))
                return _osName;

            _osName = GetOSNameByCurrentVersionKey();
            if (!string.IsNullOrEmpty(_osName))
                return _osName;

            return "";
        }

        public static string GetOSNameByProductNameKey()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        return key.GetValue("ProductName").ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
            return "";
        }

        /// <summary>
        /// <see cref="IMixpanelService"/> for Telemetry
        /// </summary>
        public static IMixpanelService MixpanelService { get; set; }

        public static Dictionary<string, Dictionary<string, object>> LeftOverEvents { get; set; }

        public static string LiveVersionFile
        {
            get
            {
                return "live_version.json";
            }
        }

        public static string LiveVersionFullName
        {
            get
            {
                return Path.GetFullPath((new Uri(System.IO.Path.Combine(ExeDirectory, LiveVersionFile)).LocalPath));
            }
        }

        public static string GetOSNameByCurrentVersionKey()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        return key.GetValue("CurrentVersion").ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
            return "";
        }

        public static string GetOSNamebyMOS()
        {
            try
            {
                var name = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<System.Management.ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();
                return name != null ? name.ToString() : "";
            }
            catch (Exception ex)
            {
                // Service.Helper.Sentry.LoggingException(ex);
            }
            return "";
        }

        public static DateTime connectTime { get; set; }
        public static bool IsExecuting { get; set; }

        public static List<Atom.SDK.Core.Models.ServerFilter> ServerChoiceList { get; set; }

        /// <summary>
        /// <see cref="INotificationLifecycleHandler"/> instance to handle notification lifecycle
        /// </summary>
        public static INotificationLifecycleHandler NotificationLifecycleHandler { get; set; }

        /// <summary>
        /// <see cref="INotificationUIHandler"/> to manage UI for <see cref="Notification"/>
        /// </summary>
        public static INotificationUIHandler NotificationUIHandler { get; set; }

        /// <summary>
        /// <see cref="INotificationCenterErrorLogger"/> to log errors from <see cref="NotificationCenter.Infrastructure.INCManager"/>
        /// </summary>
        public static INotificationCenterErrorLogger NotificationCenterErrorLogger { get; set; }

        /// <summary>
        /// Already initialized FirestoreDb instance
        /// </summary>
        public static object FirestoreDb { get; set; }

        /// <summary>
        /// <see cref="ISettingsProvider"/> implementation to provide access to AppSettings
        /// </summary>
        public static ISettingsProvider SettingsProvider { get; set; }

        /// <summary>
        /// <see cref="IBrowserLauncher"/> implementation to provide methods to interact with the browser
        /// </summary>
        public static IBrowserLauncher BrowserLauncher { get; set; }

        /// <summary>
        /// Keys for Request Paramters
        /// </summary>
        public static class RequestParameters
        {
            public const string Username = "username";
            public const string Password = "password";
            public const string DeviceType = "device_type";
            public const string DeviceId = "device_id";
            public const string AppVersion = "app_version";
            public const string ReleaseType = "release_type";
            public const string DeviceModel = "device_model";
            public const string GrantType = "grant_type";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
            public const string Scope = "scope";
            public const string Uuid = "uuid";
            public const string Message = "message";
            public const string ServerIP = "server_ip";
            public const string Locale = "locale";
            public const string NotificationId = "notification_id";
            public const string Token = "token";
        }

        /// <summary>
        /// Keys for Request Headers
        /// </summary>
        public static class RequestsHeaders
        {
            public const string Authorization = "Authorization";
            public const string Token = "Token";
        }

        /// <summary>
        /// Scope values for Access Token
        /// </summary>
        public static class AccessTokenScopes
        {
            public const string Authorization = "dialer-api/authorization";
            public const string User = "dialer-api/user";
            public const string Device = "dialer-api/device";
            public const string Profile = "dialer-api/profile";
        }


        /// <summary>
        /// Keys for only DXN APIs Request Paramters
        /// </summary>
        public static class DXNRequestParameters
        {
            public const string Username = "sUsername";
            public const string Uuid = "sUUID";
            public const string DeviceType = "sDeviceType";
            public const string DeviceId = "sDeviceId";
            public const string AppVersion = "sAppVersion";
            public const string ReleaseType = "sReleaseType";
            public const string APIKey = "api_key";
            public const string Locale = "sLocale";
            public const string DateTime = "sDateTime";
            public const string Signature = "sSignature";
        }

        /// <summary>
        /// Provides access to app resources
        /// </summary>
        public static class Resources
        {
            public const string ConnectionStatusConnected = "ConnectionStatusConnected";
            public const string ConnectionStatusConnecting = "ConnectionStatusConnecting";
            public const string ConnectionStatusNotConnected = "ConnectionStatusNotConnected";
            public const string ConnectionStatusDisconnecting = "ConnectionStatusDisconnecting";
            public const string DashboardNotificationInviteFriend = "ReferAFriendOnConnectedScreenText1";
            public const string DashboardNotificationInviteFriendCTA = "InviteNow";
            public const string DashboardNotificationTryWireGuard = "DashboardNotificationTryWireGuard";
            public const string DashboardNotificationTryWireGuardCTA = "DashboardNotificationTryWireGuardCTA";
            public const string DashboardNotificationLastConnected = "DashboardNotificationLastConnected";
            public const string DashboardNotificationAutoConnect = "DashboardNotificationAutoConnect";
            public const string DashboardNotificationAutoConnectCTA = "DashboardNotificationAutoConnectCTA";

            /// <summary>
            /// Get string resource from App by name
            /// </summary>
            public static Func<string, string> GetString;
        }

        /// <summary>
        /// Constants for Realtime Database
        /// </summary>
        public static class RealtimeDatabase
        {
            /// <summary>
            /// Project Id
            /// </summary>
            public const string ProjectId = "pragmatic-bongo-92909";

            /// <summary>
            /// Collection name for troubleshoot actions
            /// </summary>
            public const string TroubleshootColllectionName = "troubleshoot_windows";

            /// <summary>
            /// Collection name for Dislike Reasons
            /// </summary>
            public const string DislikeReasonsCollection = "reasons_dislike";

            /// <summary>
            /// Query for dislike reasons
            /// </summary>
            public const string DislikeReasonsQuery = "windows";

            /// <summary>
            /// Firebase API Key Base64 encoded
            /// </summary>
            public const string APIKey = "QUl6YVN5REtiSWVPaGlrbVpOMnA0dFFtLVJmZVdtQi1jZFFsQXNF";
        }

        /// <summary>
        /// Describes supported actions for <see cref="Notification"/>
        /// </summary>
        public class NotificationActions
        {
            /// <summary>
            /// Launch url specified in destination property in browser
            /// </summary>
            public const string OpenUrl = "open-url";

            /// <summary>
            /// Launch screen specified in the destination property in browser, possible values for destination 
            /// </summary>
            public const string OpenScreen = "open-screen";

            /// <summary>
            /// Launch members area in web browser
            /// Value for destination replaces the destination_slug placeholder in the following url
            /// </summary>
            public const string OpenMemberArea = "open-member-area";

            /// <summary>
            /// Establish a connection to the recommended location
            /// This action is only performed when VPN is DISCONNECTED, otherwise take no action
            /// </summary>
            public const string QuickConnect = "quick-connect";

            /// <summary>
            /// Delete the notification
            /// </summary>
            public const string Dismiss = "dismiss";
        }

        /// <summary>
        /// Constants for routable screens for Notification
        /// </summary>
        public class NotificationDestinationScreens
        {
            /// <summary>
            /// Referes to Locations screen
            /// </summary>
            public const string Locations = "locations";

            /// <summary>
            /// Refers to Refer a friend screen
            /// </summary>
            public const string ReferAFriend = "refer-a-friend";

            /// <summary>
            /// Refers to settings screen
            /// </summary>
            public const string Settings = "settings";

            /// <summary>
            /// Refers to Profile screen
            /// </summary>
            public const string AccountDetails = "account-details";

            /// <summary>
            /// Refers to Support screen
            /// </summary>
            public const string Support = "support";

            /// <summary>
            /// Refers to Protocols under Settings screen
            /// </summary>
            public const string Protocols = "protocols";
        }

        /// <summary>
        /// Provides slugs for Addons
        /// </summary>
        public static class Addons
        {
            /// <summary>
            /// Slug for Dedicated IP Addon
            /// </summary>
            public const string DedicatedIP = "dedicated_ip";

            /// <summary>
            /// Slug for Port Forwarding
            /// </summary>
            public const string PortForwarding = "port_forwarding";

            public const string PasswordManager = "purekeep";
        }

        /// <summary>
        /// Slugs for Cached files
        /// </summary>
        public static class CachedFiles
        {
            /// <summary>
            /// Slug for remote config file
            /// </summary>
            public static string RemoteConfig { get; set; }
        }

        /// <summary>
        /// Statuses for Addons
        /// </summary>
        public static class AddonStatus
        {
            /// <summary>
            /// Addon subscription is active
            /// </summary>
            public const string Active = "active";
        }

        /// <summary>
        /// Provides status codes for API Header (Not HTTP Status Codes)
        /// </summary>
        public static class APIResponseCodes
        {
            /// <summary>
            /// Describes status code value for successful requests
            /// </summary>
            public const int Success = 200;
        }
    }
}