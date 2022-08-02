using Microsoft.AspNet.SignalR.Hubs;

namespace PureVPN.Service.Contracts
{
    public interface ISignalrService : IHub
    {
        void VpnStatus(string message);
        
    }
}
