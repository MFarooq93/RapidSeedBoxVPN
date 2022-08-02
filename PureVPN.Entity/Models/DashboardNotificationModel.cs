using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Represents notification to show at dashboard
    /// </summary>
    public class DashboardNotificationModel
    {
        /// <summary>
        /// Message to show at the dashbaord
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Title for CTA
        /// </summary>
        public string CTATitle { get; set; }

        /// <summary>
        /// <see cref="PureVPN.Service.NotificationActions"/> to perform when CTA is clicked
        /// </summary>
        public string Action { get; set; }

        public string Destination { get; set; }
    }
}