using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Describes port forwarding detail
    /// </summary>
    public class PortForwardingDetail
    {
        /// <summary>
        /// Describes port forwarding details status
        /// </summary>
        public PortForwardingDetailsStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the port numbers for port forwarding
        /// </summary>
        public string PortNumbers { get; set; }
    }
}
