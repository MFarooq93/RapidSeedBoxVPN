using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationCenter.Infrastructure;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;

namespace PureVPN.Service
{
    /// <summary>
    /// Provides methods to communicate with the Notification Center
    /// </summary>
    public class NotificationsService : INotificationsService
    {
        public NotificationsService(INotificationInstanceDirector director)
        {
            Director = director;
        }

        /// <summary>
        /// Director for Notification Center Manager
        /// </summary>
        public INotificationInstanceDirector Director { get; }

        private INCManager _NCManager;

        public event EventHandlers.NotificationsUnreadCountChangedEvent NotificationsUnreadCountChanged;

        public INCManager NCManager
        {
            get
            {
                if (_NCManager == null)
                {
                    _NCManager = Director.GetNCManager();
                }
                return _NCManager;
            }
        }

        /// <summary>
        /// Validates whehter if Notification Center is accessible
        /// </summary>
        /// <returns></returns>
        public bool ValidateConnection()
        {
            return NCManager.AreYouThere() == "I am here";
        }

        public void Initialize()
        {
            NCManager.UnreadCountChanged += NCManager_UnreadCountChanged;
            NCManager.Initialize();
        }

        private void NCManager_UnreadCountChanged(int unreadCount)
        {
            NotificationsUnreadCountChanged?.Invoke(unreadCount);
        }

        public void Deinitialize()
        {
            NCManager.Dispose();
            _NCManager = null;
        }
    }
}
