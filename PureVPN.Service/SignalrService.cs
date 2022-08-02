using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    [HubName("PureHub")]
    public class SignalrService : Hub
    {
        public SignalrService()
        {

        }
        public void VpnStatus(string message)
        {
            try
            {
                Clients.All.VpnStatus(message);
            }
            catch (Exception e)
            {

            }
        }

        public void ConnectStatus(string message)
        {
            try
            {
                Clients.All.ConnectStatus(message);
            }
            catch (Exception e)
            {

            }
        }

        public void DisconnectStatus(string message)
        {
            try
            {
                Clients.All.DisconnectStatus(message);
            }
            catch (Exception e)
            {

            }
        }

    }
}
