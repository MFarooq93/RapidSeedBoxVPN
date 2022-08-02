using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Handlers
{
    /// <summary>
    /// Lifecycle handler for Notification
    /// </summary>
    public interface INotificationLifecycleHandler
    {
        /// <summary>
        /// When new notification is received
        /// </summary>
        /// <param name="notification"></param>
        void OnNotificationReceived(NotificationDTO notification);

        /// <summary>
        /// When new notification is received OR an existing notification is marked read or dismissed
        /// </summary>
        /// <param name="numberOfUnreadNotifications"></param>
        void OnReadCountChanged(int numberOfUnreadNotifications);

        /// <summary>
        /// When any of the CTAs of notification are clicked OR notification is dismissed
        /// </summary>
        /// <param name="type"><see cref="NotificationActionType"/></param>
        /// <param name="notification">The <see cref="NotificationDTO"/> on which action is performed</param>
        /// <param name="cta">Custom <see cref="ClickToActionDTO"/> to perform</param>
        void OnActionPerformed(NotificationActionType type, NotificationDTO notification, ClickToActionDTO cta);
    }
}
