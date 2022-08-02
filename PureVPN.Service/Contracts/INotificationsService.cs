using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PureVPN.Service.Helper.EventHandlers;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides methods to communicate with the Notification Center
    /// </summary>
    public interface INotificationsService
    {
        event NotificationsUnreadCountChangedEvent NotificationsUnreadCountChanged;

        /// <summary>
        /// Validates whehter if Notification Center is accessible
        /// </summary>
        /// <returns></returns>
        bool ValidateConnection();

        /// <summary>
        /// Intializes the Notification Center
        /// </summary>
        void Initialize();

        /// <summary>
        /// Deinitialized the Notification Center
        /// </summary>
        void Deinitialize();
    }
}
