using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    /// <summary>
    /// Provides methods to get notifications for dashboard
    /// </summary>
    public class DashboardNotificationsService : IDashboardNotificationsService
    {
        private List<DashboardNotificationModel> DashboardNotifications = new List<DashboardNotificationModel>();

        public Random Random { get; }

        public DashboardNotificationsService()
        {
            DashboardNotifications = new List<DashboardNotificationModel>
            {
                new DashboardNotificationModel
                {
                    Title = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationInviteFriend),
                    CTATitle = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationInviteFriendCTA),
                    Action = Helper.Common.NotificationActions.OpenScreen,
                    Destination = Helper.Common.NotificationDestinationScreens.ReferAFriend
                },
                new DashboardNotificationModel
                {
                    Title = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationTryWireGuard),
                    CTATitle = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationTryWireGuardCTA),
                    Action = Helper.Common.NotificationActions.OpenScreen,
                    Destination = Helper.Common.NotificationDestinationScreens.Protocols
                },
                new DashboardNotificationModel
                {
                    Title = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationLastConnected),
                    CTATitle = null,
                    Action = null,
                    Destination = Helper.Common.NotificationDestinationScreens.Settings
                },
                new DashboardNotificationModel
                {
                    Title = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationAutoConnect),
                    CTATitle = Helper.Common.Resources.GetString(Helper.Common.Resources.DashboardNotificationAutoConnectCTA),
                    Action = Helper.Common.NotificationActions.OpenScreen,
                    Destination = Helper.Common.NotificationDestinationScreens.Settings
                }
            };

            this.Random = new Random();
        }

        /// <summary>
        /// Gets <see cref="DashboardNotificationModel"/> to show at the dashboard
        /// </summary>
        /// <returns></returns>
        public DashboardNotificationModel GetNotification()
        {
            var index = GetRandomIndex();
            return DashboardNotifications.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Gets a random index for <see cref="DashboardNotifications"/>
        /// </summary>
        /// <returns></returns>
        private int GetRandomIndex()
        {
            var total = DashboardNotifications.Count;
            var ratio = 100 / total;
            var rand = Random.Next(0, 99);
            int index = rand / ratio;
            return index < total ? index : total - 1;
        }
    }
}