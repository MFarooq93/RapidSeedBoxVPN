using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    public interface IMixpanelService
    {

        void FireEvent(string eventName, string hostingId, Dictionary<string, object> properties = null);
    }
}
