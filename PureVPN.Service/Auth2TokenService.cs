using PureVPN.Core.Interfaces;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    /// <summary>
    /// Provides Auth 2.0 Token
    /// </summary>
    public class Auth2TokenService : IAuthTokenService
    {
        private IFactory _authFactory;
        IAuthenticationManager authTokenManager;

        public Auth2TokenService(IFactory authFactory, ITelemetryService authenticationTelemetryService, IOAuth2BaseURLProvider baseURLProvider)
        {
            _authFactory = authFactory;
            authTokenManager = _authFactory.GetAuthenticationManager(baseURLProvider, authenticationTelemetryService, Helper.Common.BrowserLauncher);
        }
        public async Task<AuthToken> GetAuthToken(string scope)
        {
            var authToken = await authTokenManager.GetAccessToken();
            return new AuthToken(authToken.Token);
        }
    }
}