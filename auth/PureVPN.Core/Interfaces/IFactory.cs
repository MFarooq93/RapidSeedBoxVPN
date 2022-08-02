using PureVPN.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Interfaces
{
    public interface IFactory
    {
        AuthenticationManager GetAuthenticationManager(IOAuth2BaseURLProvider baseURLProvider, ITelemetryService telemetryService, IBrowserLauncher browserLauncher);
    }
}
