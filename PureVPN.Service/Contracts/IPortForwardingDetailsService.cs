using PureVPN.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides port forwarding details
    /// </summary>
    public interface IPortForwardingDetailsService
    {
        /// <summary>
        /// Gets <see cref="PortForwardingDetail"/> for user
        /// </summary>
        /// <returns></returns>
        Task<PortForwardingDetail> GetPortForwardingDetails();
    }
}
