using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NotificationCenter.Infrastructure.Models.EventHandlers;

namespace NotificationCenter.Infrastructure.Handlers
{
    /// <summary>
    /// Donates an instance which manages the UI for <see cref="NotificationDTO"/>
    /// </summary>
    public interface INotificationUIHandler
    {
        /// <summary>
        /// Instance of <see cref="NotificationLifecycleHandler"/> to handle notification actions
        /// </summary>
        //INotificationLifecycleHandler NotificationLifecycleHandler { get; set; }

        event DismissButtonClicked DismissButtonClickedEvent;
        event PrimaryButtonClicked PrimaryButtonClickedEvent;
        event SecondaryButtonClicked SecondaryButtonClickedEvent;

        void Add(NotificationDTO notification);
        void Remove(NotificationDTO notification);
        void Add(IEnumerable<NotificationDTO> notifications);
        void Clear();
    }
}
