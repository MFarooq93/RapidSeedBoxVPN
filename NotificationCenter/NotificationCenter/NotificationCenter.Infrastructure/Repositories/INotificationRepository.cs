using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Repositories
{
    /// <summary>
    /// Provides methods to access and manipulate locally saved notifications
    /// </summary>
    public interface INotificationRepository : IRepository<Notification>
    {
        /// <summary>
        /// Gets count of unread <see cref="Notification"/>s
        /// </summary>
        /// <returns></returns>
        int GetUnreadCount();
    }
}
