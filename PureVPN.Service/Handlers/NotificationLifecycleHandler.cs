using NotificationCenter.Infrastructure.Handlers;
using NotificationCenter.Infrastructure.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Handlers
{
    /// <summary>
    /// Lifecycle handler for Notification
    /// </summary>
    public class NotificationLifecycleHandler : INotificationLifecycleHandler
    {
        public NotificationLifecycleHandler(IDialerToServerService dialerToServerService)
        {
            DialerToServerService = dialerToServerService;
        }

        public IDialerToServerService DialerToServerService { get; }

        /// <summary>
        /// When any of the CTAs of notification are clicked OR notification is dismissed
        /// </summary>
        /// <param name="type"><see cref="NotificationActionType"/></param>
        /// <param name="notification">The <see cref="Notification"/> on which action is performed</param>
        /// <param name="cta">Custom <see cref="ClickToAction"/> to perform</param>
        public void OnActionPerformed(NotificationActionType type, Notification notification, ClickToAction cta)
        {
            MarkRead(notification.Id);
        }

        /// <summary>
        /// When new notification is received
        /// </summary>
        /// <param name="notification"></param>
        public void OnNotificationReceived(Notification notification)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When new notification is received OR an existing notification is marked read or dismissed
        /// </summary>
        /// <param name="numberOfUnreadNotifications"></param>
        public void OnReadCountChanged(int numberOfUnreadNotifications)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks <see cref="Notification"/> as Read
        /// </summary>
        /// <param name="notificationId"></param>
        private async void MarkRead(string notificationId)
        {
            await DialerToServerService.MarkNotificationAsRead(notificationId);
        }
    }
}