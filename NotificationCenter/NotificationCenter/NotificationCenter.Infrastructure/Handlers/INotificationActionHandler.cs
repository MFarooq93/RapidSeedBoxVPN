using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Handlers
{
    public interface INotificationActionHandler
    {
        void OnDismissClick(NotificationDTO notification);
        void OnPrimaryActionClick(NotificationDTO notification);
        void OnSecondaryActionClick(NotificationDTO notification);
    }
}