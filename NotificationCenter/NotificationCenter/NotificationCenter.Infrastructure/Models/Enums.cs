using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Models
{
    /// <summary>
    /// Types for Notification Actions
    /// </summary>
    public enum NotificationActionType
    {
        /// <summary>
        /// Specific action described by <see cref="ClickToActionDTO.Action"/>
        /// </summary>
        NotificationAction,

        /// <summary>
        /// Notitification has been read
        /// </summary>
        NotificationRead,

        /// <summary>
        /// Notification has been dismissed
        /// </summary>
        NotificationDismiss,
    }

    /// <summary>
    /// Types for change to notification data
    /// </summary>
    public enum NotificationChangeType
    {
        /// <summary>
        /// A new notification record is added
        /// </summary>
        Added,
        /// <summary>
        /// An existing notification record has been updated
        /// </summary>
        Updated,
        /// <summary>
        /// A notification record has been deleted
        /// </summary>
        Deleted
    }
}
