using PureVPN.Entity.Enums;
using PureVPN.Entity.Models;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    /// <summary>
    /// Implements <see cref="IPortForwardingDetailsService"/> to provide port forwarding details
    /// </summary>
    public class PortForwardingDetailsService : IPortForwardingDetailsService
    {
        public PortForwardingDetailsService(IUserProfileProvider userProfileProvider, IDialerToServerService dialerToServerService, ISentryService sentryService)
        {
            UserProfileProvider = userProfileProvider;
            DialerToServerService = dialerToServerService;
            SentryService = sentryService;
        }

        public IUserProfileProvider UserProfileProvider { get; }
        public IDialerToServerService DialerToServerService { get; }
        public ISentryService SentryService { get; }

        /// <summary>
        /// Gets <see cref="PortForwardingDetail"/> for user
        /// </summary>
        /// <returns></returns>
        public async Task<PortForwardingDetail> GetPortForwardingDetails()
        {
            var response = new PortForwardingDetail();

            var isPFSubscribed = this.UserProfileProvider.HasAddon(Common.Addons.PortForwarding);

            if (isPFSubscribed == false)
            {
                response.Status = PortForwardingDetailsStatus.PurchaseAddon;
                return response;
            }

            PortForwardingReply portForwarding = null;
            try
            {
                portForwarding = await this.DialerToServerService.GetForwardedPorts();
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
                response.Status = PortForwardingDetailsStatus.FailedToLoad;
                return response;
            }

            return ProcessPortForwardingReply(portForwarding);
        }

        /// <summary>
        /// Gets <see cref="PortForwardingDetail"/> from <see cref="PortForwardingReply"/> <paramref name="reply"/>
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        private PortForwardingDetail ProcessPortForwardingReply(PortForwardingReply reply)
        {
            var response = new PortForwardingDetail();

            if (reply?.header?.response_code != Common.APIResponseCodes.Success ||
                String.IsNullOrWhiteSpace(reply?.Body?.Status) ||
                reply.Body.Status.Equals(Common.AddonStatus.Active, StringComparison.OrdinalIgnoreCase) == false)
            {
                response.Status = PortForwardingDetailsStatus.FailedToLoad;
                return response;
            }

            if (reply.Body.IsOpenAllPorts)
            {
                response.Status = PortForwardingDetailsStatus.All;
            }

            if (reply.Body.IsBlockAllPorts)
            {
                response.Status = PortForwardingDetailsStatus.None;
            }

            if (reply.Body.IsBlockAllPortsExcept)
            {
                response.Status = PortForwardingDetailsStatus.EnabledPortsOnly;
                response.PortNumbers = reply.Body.Ports;
            }

            return response;
        }
    }
}
