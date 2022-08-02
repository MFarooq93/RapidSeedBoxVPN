using Newtonsoft.Json;
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
    /// Provides Authentication Token
    /// </summary>
    public class AuthTokenService : IClientAppAccessTokenService
    {
        #region Constructors
        public AuthTokenService(IMixpanelService mixpanelService, IBaseURLProvider baseURLProvider)
        {
            MixpanelService = mixpanelService;
            BaseURLProvider = baseURLProvider;
        }
        #endregion

        #region Properties
        public IMixpanelService MixpanelService { get; }
        public IBaseURLProvider BaseURLProvider { get; }
        #endregion

        #region IAuthTokenService implementations
        /// <summary>
        /// Get Authentication Token <see cref="AuthToken"/>
        /// </summary>
        /// <returns><see cref="AuthToken"/></returns>
        public async Task<AuthToken> GetAuthToken(string scope)
        {
            AuthToken cachedToken;
            AtomModel.AuthTokens.TryGetValue(scope, out cachedToken);

            if (cachedToken == null ||
                String.IsNullOrWhiteSpace(cachedToken.Token) ||
                cachedToken.HasExpired)
            {
                int maxAttempts = 4;
                AccessTokenReply reply = null;
                DateTime tokenGeneratedOnInUtc = default(DateTime);
                for (int attemptNumber = 1; attemptNumber <= maxAttempts; attemptNumber++)
                {
                    try
                    {
                        tokenGeneratedOnInUtc = DateTime.UtcNow;
                        reply = await GetNewToken(scope);
                        if (ValidateTokenReply(reply))
                        {
                            break;
                        }
                    }
                    catch
                    {
                        if (attemptNumber >= maxAttempts)
                        {
                            throw;
                        }
                    }
                }

                if (ValidateTokenReply(reply))
                {
                    AtomModel.AuthTokens[scope] = new AuthToken(reply.access_token, reply.token_type, tokenGeneratedOnInUtc, reply.expires_in);
                    return AtomModel.AuthTokens[scope];
                }
            }

            return cachedToken;
        }
        #endregion

        #region Private methods


        /// <summary>
        /// Gets new authentication token
        /// </summary>
        /// <returns></returns>
        private async Task<AccessTokenReply> GetNewToken(string scope)
        {
            var localPath = ServiceHelperConstants.UrlAuthToken;
            var baseUrls = BaseURLProvider.GetAuthTokenUrls();
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.GrantType, "client_credentials"));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.ClientId, AtomModel.AuthorizeClientId));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.ClientSecret, AtomModel.AuthorizeClientSecret));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.Scope, scope));

            var response = await WebRequestHelper.PostRequestAsFormUrlEncoded(baseUrls, localPath, postData);
            BaseURLProvider.SetSuccessfulAuthTokenUrl(response.SuccessfulBaseUrl);
            this.MixpanelService.SendResponseToMixpanel(response.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<AccessTokenReply>(response.ResponseBody);
        }

        /// <summary>
        /// Validates Authentication Token Reply
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        private bool ValidateTokenReply(AccessTokenReply reply)
        {
            return reply != null && !String.IsNullOrWhiteSpace(reply.access_token) && reply.expires_in > default(int);
        }

        #endregion
    }
}
