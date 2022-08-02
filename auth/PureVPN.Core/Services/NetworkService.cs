using Newtonsoft.Json;
using PureVPN.Core.Exceptions;
using PureVPN.Core.Helper;
using PureVPN.Core.Interfaces;
using PureVPN.Core.Manager;
using PureVPN.Core.Models;
using PureVPN.Core.NetworkHelper;
using PureVPN.Core.PureVPN.Core.Exceptions;
using PureVPN.Core.RequestParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Services
{
    internal class NetworkService
    {
        public ITelemetryService TelemetryService { get; }

        public NetworkService(ITelemetryService telemetryService, IOAuth2BaseURLProvider baseURLProvider)
        {
            TelemetryService = telemetryService;
            WebRequestHelper.OAuth2BaseURLProvider = baseURLProvider;
        }

        internal async Task<Authentication> GenerateAccessToken(string url, TokenRequestParameter tokenRequestParameter)
        {
            try
            {
                var content = new Dictionary<string, string>()
                {
                    { RequestParams.GrantType, tokenRequestParameter.GrantType},
                    { RequestParams.ClientId, tokenRequestParameter.ClientID},
                    { RequestParams.Code, tokenRequestParameter.Code},
                    { RequestParams.RedirectURI, tokenRequestParameter.RedirectUri},
                    { RequestParams.CodeVerifier, tokenRequestParameter.CodeVerifier}
                };

                var authToken = await WebRequestHelper.PostAsync<Authentication>(url, content, telemetryService: this.TelemetryService);

                if (null != authToken)
                {
                    return new Authentication()
                    {
                        Token = authToken.Token,
                        RefreshToken = authToken.RefreshToken,
                        ExpiresIn = authToken.ExpiresIn,
                        Scope = authToken.Scope,
                        UserId = authToken.UserId,
                        TokenType = authToken.TokenType
                    };
                }
            }
            catch (JsonSerializationException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.AccessTokenJsonSerializationException, ex);
            }
            catch (WebException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.AccessTokenWebException, ex);
            }

            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.AccessTokenUnableToGetResponseFromAPI, ex);
            }

            return default;
        }

        internal async Task<Authentication> RefreshAccessToken(string url, RefreshTokenRequestParameter refreshTokenRequestParameter)
        {
            try
            {
                var content = new Dictionary<string, string>()
                {
                    {RequestParams.GrantType, refreshTokenRequestParameter.GrantType },
                    {RequestParams.ClientId, refreshTokenRequestParameter.ClientID },
                    {RequestParams.RefreshToken, refreshTokenRequestParameter.RefreshToken }
                };

                var authToken = await WebRequestHelper.PostAsync<Authentication>(url, content, telemetryService: this.TelemetryService);

                if (null != authToken)
                {
                    return new Authentication()
                    {
                        Token = authToken.Token,
                        RefreshToken = authToken.RefreshToken,
                        ExpiresIn = authToken.ExpiresIn,
                        Scope = authToken.Scope,
                        UserId = authToken.UserId,
                        TokenType = authToken.TokenType
                    };
                }
            }
            catch (JsonSerializationException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.RefreshAccessTokenJsonSerializationException, ex);
            }
            catch (WebException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.RefreshAccessTokenWebException, ex);
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGetResponseFromRefreshToken, ex);
            }

            return default;
        }

        /// <summary>
        /// Revokes refresh token
        /// </summary>
        /// <param name="url">API Endpoint to Revoke refresh token API</param>
        /// <param name="refreshToken">Refresh token to revoke</param>
        /// <returns></returns>
        internal async Task RevokeRefreshToken(string url, string refreshToken, string token)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add(RequestParams.Token, refreshToken);
                var response = await WebRequestHelper.DeleteAsync<BaseErrorModel>(url, parameters, token: token, telemetryService: this.TelemetryService);
            }
            catch { }
        }

        internal async Task<UserInfo> GetUserInfo(string url, string token)
        {
            try
            {
                var userInfo = await WebRequestHelper.GetAsync<UserInfoBody>(url, token, telemetryService: this.TelemetryService);

                if (null != userInfo)
                {
                    return userInfo.UserInfo;
                }
                else
                {
                    Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGetUserInfo);
                }
            }
            catch (JsonSerializationException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UserInfoJsonSerializationException, ex);
            }
            catch (WebException ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UserInfoWebException, ex);
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGetRepsonseFromUserInfoApi, ex);
            }
            return default;
        }
    }
}
