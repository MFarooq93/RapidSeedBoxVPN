using PureVPN.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides methods to get notifications for dashboard
    /// </summary>
    public interface IDashboardNotificationsService
    {
        /// <summary>
        /// Gets <see cref="DashboardNotificationModel"/> to show at the dashboard
        /// </summary>
        /// <returns></returns>
        DashboardNotificationModel GetNotification();
    }
}