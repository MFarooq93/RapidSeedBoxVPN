using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Enums
{
    /// <summary>
    /// Describes port forwarding details status
    /// </summary>
    public enum PortForwardingDetailsStatus
    {
        /// <summary>
        /// User is not subscribed to port forwarding addon
        /// </summary>
        PurchaseAddon,
        /// <summary>
        /// No ports are configured for port forwarding
        /// </summary>
        None,
        /// <summary>
        /// All ports are configured for port forwarding
        /// </summary>
        All,
        /// <summary>
        /// Some ports are configured for port forwarding
        /// </summary>
        EnabledPortsOnly,
        /// <summary>
        /// API request to get port forwarding details has failed
        /// </summary>
        FailedToLoad
    }
}
