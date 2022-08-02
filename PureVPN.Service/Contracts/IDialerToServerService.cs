using System.Collections.Generic;
using System.Threading.Tasks;
using PureVPN.Entity.Models.DTO;

namespace PureVPN.Service.Contracts
{
    public interface IDialerToServerService
    {
        Dictionary<string, object> LoginData(string username, string password);
        Task<LoginReply> Login(Dictionary<string, object> loginData);
        Task<DevicesReply> GetSetupOtherDevices();
        Task<LoginReply> GetAuthenticationKeyByUserName(string username, string uuid);
        Task<ForceUpdateReply> GetForceUpdate();
        Task<AppSurvey> GetAppContent();
        Task<TicketReply> CreateTicketInZendesk(string sMessage);
        Task<DedicatedIP> GetDedicatedIpInfo();
        Task<DedicatedIP> KillDedicatedIpSession(string DedicatedIP);
        Task<ProfileReply> GetUserProfile();
        Task<ReferAFriend> GetReferAFrinedCountAndLink();

        Task<MigrateUserToEmail> GetMigrateUserToEmail(string username,string uuid);

        /// <summary>
        /// Gets token for Firestore SDK authentication
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<FirestoreTokenReply> GetFirestoreTokenAsync(string uuid, string username);
        Task<TicketReply> RecordDesireOutcome();

        /// <summary>
        /// Mark <see cref="NotificationCenter.Infrastructure.Models.NotificationDTO"/> as Read
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task MarkNotificationAsRead(string notificationId);

        /// <summary>
        /// Delete <see cref="NotificationCenter.Infrastructure.Models.NotificationDTO"/> by <paramref name="notificationId"/>
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task DeleteNotification(string notificationId);
        Task<SpeedtestExperiment> GetExperimentGroupOfUser();

        /// <summary>
        /// Gets forwarded ports <see cref="PortForwardingReply"/>
        /// </summary>
        /// <returns></returns>
        Task<PortForwardingReply> GetForwardedPorts();
    }
}