using PureVPN.Entity.Enums;
using PureVPN.Entity.Models.DTO;
using System;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public class SettingsModel
    {
        public bool AutoConnect { get; set; }
        public bool ShowAlerts { get; set; } = true;
        public bool IsAutomaticProtocol { get; set; } = true;
        public ProtocolModel SelectedProtocol { get; set; } = new ProtocolModel();
        public AutoConnectOptionModel SelectedAutoConnectOption { get; set; } = new AutoConnectOptionModel();
        public LocationModel AutoConnectSelectedLocation { get; set; } = new LocationModel();

        public bool IsConnectToFallback { get; set; } = true;

        public bool IsIksEnable { get; set; } = true;

        public LoginReply loginReply { get; set; }

        public bool IsLoggedIn { get; set; }

        private bool _IsSameExperienceCheckEnable;
        public bool IsSameExperienceCheckEnable
        {
            get
            {
                if (AtomModel.RandomBit == 0)
                    return false;
                else
                    return true;
            }
            set
            {
                _IsSameExperienceCheckEnable = value;
            }
        }

        public bool IsRandomBitGenerated { get; set; }
        public int RandomBit { get; set; }

        public bool IsDontShowMessageAgain { get; set; }
        public bool QuitAndDisconnect { get; set; }

        /// <summary>
        /// Stores the preference to show or not show ping values with countries and cities
        /// </summary>
        public bool IsShowPingValues { get; set; } = true;

        /// <summary>
        /// Store the selection of last sorting option for countries list
        /// </summary>
        public string LastSortedOption { get; set; }

        public List<ApplicationListModel> SelectedApps { get; set; }
        public bool IsSplitEnable { get; set; } = false;

        /// <summary>
        /// Store the addition apps that are added by file browser
        /// </summary>
        public List<ApplicationListModel> AppsAddedByFileDialogue { get; set; }
        public int IsDoAchieved { get; set; } = 0;
        public bool IsFeedbackGiven { get; set; } = false;
        public bool Userchoosentonotaskagainfeedback { get; set; } = false;
        public bool IsFriendReffered { get; set; } = false;
        public List<ServerPreference> UserPreferenceServers { get; set; }
        public int NumberOfSessions { get; set; }
        public bool AllServers { get; set; } = true;
        public bool ObfuscatedServer { get; set; }
        public bool P2PServer { get; set; }
        public bool TorServer { get; set; }
        public bool VirtualServer { get; set; }
        public bool QRServer { get; set; }

        public bool IsEnableNetworkAnalysis { get; set; } = true;
        public double PreConnectionBaseSpeedOfUser { get; set; }
        public DateTime PreConnectionSpeedMeasurementTime { get; set; }
    }
}
