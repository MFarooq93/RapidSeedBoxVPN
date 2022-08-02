using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Helper
{
    public class ServiceHelperConstants
    {
        public const string UrlGetAppSurveyContent = "/device/appcontent/v3";

        public const string UrlGetAccountInformation = "/user/accountinfo/v3";
        public const string UrlGetForceUpdate = "/rollout/checkupdate";
        public const string UrlKillDedicatedIpSession = "/user/killsession/v3";
        public const string UrlLoginWithEmail = "/user/login";
        public const string UrlGetDedicatedIpInfo = "/user/dedicatedipdetail/v3";
        public const string UrlPortForwarding = "/user/portforwarding/v3";
        public const string UrlCreateTicketInZendesk = "/user/zendeskticket/v3";
        public const string UrlGetUserProfile = "/user/profile/v3";
        public const string UrlAuthToken = "/oauth2/token";
        public const string UrlImportToken = "/user/importtoken";
        public const string UrlReferAFriend = "/user/referafriend/v3";
        public const string UrlGetFirestoreToken = "/user/firestoretoken/v3";
        public const string UrlRecordDesiredOutcome = "/user/setdesiredoutcome/v3";
        public const string UrlMigrateUserToEmail = "/user/migrateemail/v3";
        public const string UrlGetSetupOtherDevices = "/device/setupotherdevices/v3";

        /// <summary>
        /// Url to mark notification as read
        /// </summary>
        public const string UrlNotificationMarkRead = "/notification/markread/v3";
        /// <summary>
        /// Url to delete notification
        /// </summary>
        public const string DeleteNotification = "/notification/delete/v3";
        public const string UrlSpeedtestExperiment = "/user/experiments/v3";


        public const string DefaultAuthTokenBaseUrl = "https://auth2.dialertoserver.com/";
        public const string DefaultServicesBaseUrl = "https://gateway.dialertoserver.com/";
        public const string DefaultFAAPIBaseUrl = "https://auth.purevpn.com/";
        public const string DefaultFAWebpageBaseUrl = "https://login.purevpn.com/";
    }
}