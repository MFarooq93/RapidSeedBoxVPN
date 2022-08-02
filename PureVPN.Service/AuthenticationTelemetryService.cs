using PureVPN.Core.Interfaces;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PureVPN.Service.Helper;

namespace PureVPN.Service
{
    public class AuthenticationTelemetryService : ITelemetryService
    {
        public AuthenticationTelemetryService(IMixpanelService mixpanelService)
        {
            MixpanelService = mixpanelService;
        }

        public IMixpanelService MixpanelService { get; }

        public void SendAPIFailed(HttpResponseMessage httpResponse)
        {
            Extensions.SendAppApiFailureEvent(httpResponse);
        }
    }
}