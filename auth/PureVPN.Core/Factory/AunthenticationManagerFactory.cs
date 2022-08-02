using PureVPN.Core.Helper;
using PureVPN.Core.Interfaces;
using PureVPN.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Factory
{
    public class AunthenticationManagerFactory : IFactory
    {
        internal static AuthenticationManager _authenticationManager;

        public AuthenticationManager GetAuthenticationManager(IOAuth2BaseURLProvider baseURLProvider, ITelemetryService telemetryService = null, IBrowserLauncher browserLauncher = null)
        {
            if (null == _authenticationManager)
            {
                browserLauncher = browserLauncher ?? new DefaultBrowserLauncher();
                _authenticationManager = new AuthenticationManager(telemetryService, baseURLProvider, browserLauncher);
            }

            return _authenticationManager;
        }
    }
}
