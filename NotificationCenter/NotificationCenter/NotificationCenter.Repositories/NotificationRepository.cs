using NotificationCenter.Infrastructure.Models;
using NotificationCenter.Infrastructure.Repositories;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Repositories
{
    /// <summary>
    /// Provides methods to access and manipulate locally saved notifications
    /// </summary>
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(IDatabaseProvider databaseProvider) : base(databaseProvider)
        {
        }

        /// <summary>
        /// Gets count of unread <see cref="Notification"/>s
        /// </summary>
        /// <returns></returns>
        public int GetUnreadCount()
        {
            return this.Count(x => x.IsRead == false);
        }

        new void OnChange(Action<List<DataChangeDTO<Notification>>> callback)
        {
            this.DataChangeCallback = callback;
            this.Subscriber = this.Database.All<Notification>().SubscribeForNotifications(OnLocalDataChanged);
        }
    }
}
