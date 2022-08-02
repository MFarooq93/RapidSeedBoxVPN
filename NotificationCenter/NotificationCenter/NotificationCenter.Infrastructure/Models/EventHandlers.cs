using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Models
{
    public class EventHandlers
    {
        public delegate void DismissButtonClicked(NotificationDTO notification);
        public delegate void PrimaryButtonClicked(NotificationDTO notification);
        public delegate void SecondaryButtonClicked(NotificationDTO notification);
        public delegate void UnreadCountChanged(int unreadCount);
    }
}
