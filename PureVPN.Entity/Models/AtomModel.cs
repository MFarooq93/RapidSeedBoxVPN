using PureVPN.Entity.Enums;
using PureVPN.Entity.Models.DTO;
using PureVPN.SpeedTest.Models;
using System;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public sealed class AtomModel
    {

        private static readonly Lazy<AtomModel> lazy = new Lazy<AtomModel>(() => new AtomModel());
        public static AtomModel Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public static string SecretKey { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string UserToken { get; set; }
        public static string HostingID { get; set; }
        public static string Token { get; set; }
        public static string UUID { get; set; }
        public static string ClientId { get; set; }
        public static bool IsDisconnected { get { return !IsConnected; } }
        public static bool IsConnected { get; set; }
        public static bool IsConnecting { get; set; }
        public static bool IsSDKInitializing { get; set; }
        public static bool ISSDKInitialized { get; set; }
        public static bool IsLoggedIn { get; set; }
        public static bool IsEventsRegister { get; set; }
        public static bool IsInternetDown { get; set; }
        public static bool IsCancelled { get; set; }

        public static List<CountryModel> Countries { get; set; }
        public static List<CityModel> Cities { get; set; }
        public static List<ProtocolModel> Protocols { get; set; }
        public static List<CountryModel> CountriesRecent { get; set; }
        public static List<LocationModel> LocationsRecent { get; set; }
        public static List<LocationModel> LocationsRecentCache { get; set; }
        public static ConnectedTo ConnectedTo { get; set; } = ConnectedTo.NotConnected;
        public static ConnectedTo LastConnectedTo { get; set; } = ConnectedTo.NotConnected;
        public static bool BackgroundVpnStatus { get; set; }

        public static bool ShowCancelButton { get; set; }

        public static string SelectedProtocol { get; set; }
        public static string ServerAddress { get; set; }
        public static string ServerIP { get; set; }
        public static string DialedProtocol { get; set; }
        public static bool ShowAppSurvey { get; set; }

        public static UpdateType UpdateType { get; set; }

        public static TabScreen ActiveTab { get; set; }

        public static bool IsUpdateForced { get; set; }
        public static string IsAutoUpdateEnabled { get; set; }

        public static string LeftOverHostingID { get; set; }

        public static Models.DTO.DedicatedIP dedicatedIP { get; set; }

        public static ConnectingFrom connectingFrom { get; set; }
        public static SelectedInterfaceScreen selectedInterfaceScreen { get; set; }
        public static ConnectionInitiatedBy connectionInitiatedBy { get; set; }
        public static bool IsBannerManuallyClosed { get; set; }

        public static bool IsIksEnable { get; set; }
        public static bool IsEnableNetworkAnalysis { get; set; }
        public static bool IsSplitEnable { get; set; }
        public static bool IsIksEnableAtAtom { get; set; }

        public static bool IsQuantumResistantServer { get; set; }

        public static DTO.ProfileReply Profile { get; set; }

        public static string BeforeConnectionLocation { get; set; }
        public static string AfterConnectionLocation { get; set; }
        public static List<string> PendingToMarkFavorite { get; set; }

        public static bool IsSameExperienceCheckEnable { get; set; }

        public static int RandomBit { get; set; }

        public static long LastByteRecieved { get; set; }
        public static long LastByteSent { get; set; }
        public static string MeasureUnit { get; set; }

        public static CountryModel FastestCountry { get; set; }

        public static string DialedProtocolForConnectionDetails { get; set; }

        /// <summary>
        /// Cached <see cref="AuthToken"/>s with respect to <see cref="AccessTokenScopes"/>.
        /// </summary>
        public static Dictionary<string, AuthToken> AuthTokens { get; set; } = new Dictionary<string, AuthToken>();

        public static string AuthorizeClientId { get; set; }
        public static string AuthorizeClientSecret { get; set; }

        public static string protoselection { get; set; }

        public static List<string> IncludedNasIdentifiers { get; set; }
        public static List<string> ExcludedNasIdentifiers { get; set; }

        public static bool WireguardConsentFlag { get; set; }

        /// <summary>
        /// Set to true if the connection is establish with city
        /// </summary>
        public static bool IsConnectedToCity { get; set; }

        /// <summary>
        /// Get the name of last connected country
        /// </summary>
        public static string LastConnectedCountryName { get; set; }

        /// <summary>
        /// Get the name of last connected city
        /// </summary>
        public static string LastConnectedCityName { get; set; }

        /// <summary>
        /// Get the last connection type to get tha last connecting type (city, country or dedicated ip) for connection over connection
        /// </summary>
        public static ConnectingFrom LastConnectionType { get; set; }
        public static List<ApplicationListModel> SelectedAppsForSplit { get; set; }
        public static bool StartConnecting { get; set; }
        public static DTO.SpeedtestExperiment Experiments { get; set; }
        public static string SpeedtestGroup { get; set; }

        public static string CountrySlugAboutToConnect { get; set; }

        public static List<ServerPreference> UserPreferenceServers { get; set; }

        public static bool IsExperimentServerRequested { get; set; }

        /// <summary>
        /// Reason selected for disliking session
        /// </summary>
        public static DislikeReasonModel DislikeReason { get; set; }

        public static List<LocationFilter> locationFilters { get; set; } = new List<LocationFilter>();

        public static bool? isObfSupported { get; set; } = null;
        public static bool? isTorSupported { get; set; } = null;
        
        public static bool IsExperimentedServer { get; set; }

        public static SessionRatingReason SessionRatingReason { get; set; }

        public static bool? isP2pSupported { get; set; } = null;
        public static bool? isQrSupported { get; set; } = null;
        public static bool? isVirtualSupported { get; set; } = null;
        public static LocationFilters LastConnectionLocationFilters { get; set; } = new LocationFilters();
        public static string SelectedInterface { get; set; }
    }
}
