using Newtonsoft.Json;
using PureVPN.Core.Enums;
using PureVPN.Core.Exceptions;
using PureVPN.Core.Extensions;
using PureVPN.Core.Helper;
using PureVPN.Core.Interfaces;
using PureVPN.Core.Models;
using PureVPN.Core.PureVPN.Core.Exceptions;
using PureVPN.Core.RequestParameter;
using PureVPN.Core.Services;
using PureVPN.Core.Utilities;
using PureVPN.Core.Validator;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PureVPN.Core.Manager
{
    public class AuthenticationManager : IAuthenticationManager
    {
        #region Private properties

        private AuthUtilities _authUtilitiesInstance;
        private Authentication _authentication;
        private bool _isAccessTokenGenerating = false;

        public ITelemetryService TelemetryService { get; }
        public IOAuth2BaseURLProvider BaseURLProvider { get; }
        public IBrowserLauncher BrowserLauncher { get; }

        #endregion

        #region Public properties

        #endregion

        #region Const

        internal AuthenticationManager(ITelemetryService telemetryService, IOAuth2BaseURLProvider baseURLProvider, IBrowserLauncher browserLauncher)
        {
            _authUtilitiesInstance = AuthUtilities.GetAuthUtilitiesInstance();
            TelemetryService = telemetryService;
            BaseURLProvider = baseURLProvider;
            BrowserLauncher = browserLauncher;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method initiate access token request
        /// </summary>
        /// <exception cref=""></exception>
        public void InitiateAccessTokenRequest()
        {
            try
            {
                string authUrl = GetAuthorizationURL();
                RedirectToBrowser(authUrl);
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToRedirectToBrowser, ex);
            }
        }

        /// <summary>
        /// Initiate user logout flow
        /// </summary>
        public async void Logout()
        {

            try
            {
                string logoutUrl = GetLogoutURL();
                RedirectToBrowser(logoutUrl, isCloseable: true);

                var auth = await GetAccessToken();

                await new NetworkService(this.TelemetryService, this.BaseURLProvider)
                    .RevokeRefreshToken(URLs.RevokeRefreshTokenEndpoint, auth.RefreshToken, auth.Token);
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToRedirectToBrowserWithLogoutUrl, ex);
            }
        }

        /// <summary>
        /// This method generate access token from API
        /// </summary>
        /// <param name="code">code get from API</param>
        /// <returns></returns>
        /// <exception cref="PureVPNCoreException">thorw if access token is not generated</exception>
        public async Task GenerateAccessToken(string authGrantCode)
        {
            try
            {
                if (authGrantCode.IsStingNullorEmpty())
                    throw new PureVPNCoreException(ErrorCodes.AuthGrantCodeIsNull, ErrorMessages.GetErrorMessage(ErrorCodes.AuthGrantCodeIsNull));

                _isAccessTokenGenerating = true;
                var tokenParameters = new TokenRequestParameter()
                {
                    Code = authGrantCode,
                    GrantType = GrantType.authorization_code.ToString(),
                    CodeVerifier = _authUtilitiesInstance.CodeVerifier,
                    RedirectUri = URLs.GetRedirectorUrl(this.BaseURLProvider, URLs.CallbackAuthorisation)
                };

                _authentication = await new NetworkService(this.TelemetryService, this.BaseURLProvider).GenerateAccessToken(URLs.TokenUrl, tokenParameters);
                if (null == _authentication?.Token)
                    Common.ThrowPureVPNCoreException(ErrorCodes.GetNullFromAcessToken);

                _isAccessTokenGenerating = false;
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGenerateAccessToken, ex);
            }
        }

        public async Task<Authentication> GetAccessToken()
        {
            try
            {
                if (!_authentication.IsTokenExpired)
                    return _authentication;
                else
                {
                    ValidationHelper.ValidateRefreshTokenRequest(_authentication, _isAccessTokenGenerating);
                    var authToken = await GetTokenFromRefreshToken(_authentication.RefreshToken);
                    return authToken;
                }
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.InvalidRefreshTokenRequest, ex);
                return default;
            }
        }

        public async Task GenerateTokenFromRefreshToken(string refreshToken)
        {
            await GetTokenFromRefreshToken(refreshToken);
        }

        public async Task<UserInfo> GetUserInto(string token)
        {
            try
            {
                return await new NetworkService(this.TelemetryService, this.BaseURLProvider).GetUserInfo(URLs.UserInfoUrl, token);
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGetUserInfo, ex);
                return default;
            }
        }

        public void SetToken(Authentication authentication)
        {
            if (null == authentication)
                throw new PureVPNCoreException(ErrorCodes.GivenTokenIsNull, ErrorMessages.GetErrorMessage(ErrorCodes.GivenTokenIsNull));

            _authentication = authentication;
        }

        #endregion

        #region Private Mehtods

        private async Task<Authentication> GetTokenFromRefreshToken(string refreshToken)
        {
            try
            {
                var refreshTokenParameters = new RefreshTokenRequestParameter()
                {
                    GrantType = GrantType.refresh_token.ToString(),
                    RefreshToken = refreshToken
                };

                _authentication = await new NetworkService(this.TelemetryService, this.BaseURLProvider).RefreshAccessToken(URLs.TokenUrl, refreshTokenParameters);
                if (null == _authentication)
                    Common.ThrowPureVPNCoreException(ErrorCodes.GetNullFromRefreshToken);

                return _authentication;
            }
            catch (PureVPNCoreException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGenerateRefreshToken, ex);
                return default;
            }
        }

        /// <summary>
        /// Gets redirection url to log user out
        /// </summary>
        /// <returns></returns>
        private string GetLogoutURL()
        {
            try
            {
                NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(RequestParams.ClientId, CommonKeys.ClientID);
                queryString.Add(RequestParams.Platform, CommonKeys.Platform);
                queryString.Add(RequestParams.PostLogoutRedirectURI, URLs.GetRedirectorUrl(this.BaseURLProvider, URLs.CallbackLogout));
                var query = queryString.ToString();

                UriBuilder uriBuilder = new UriBuilder(URLs.GetUrl(this.BaseURLProvider, URLs.LogoutUrl));
                uriBuilder.Query = query;

                string autherizationURL = uriBuilder.Uri.ToString();
                return autherizationURL;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGenerateLogoutURL, ex);
                return null;
            }
        }

        private string GetAuthorizationURL()
        {
            try
            {
                NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(RequestParams.Platform, CommonKeys.Platform);
                queryString.Add(RequestParams.ResponseType, ResponseType.code.ToString());
                queryString.Add(RequestParams.ClientId, CommonKeys.ClientID);
                queryString.Add(RequestParams.RedirectURI, URLs.GetRedirectorUrl(this.BaseURLProvider, URLs.CallbackAuthorisation));
                queryString.Add(RequestParams.Scope, GetUserScope());
                queryString.Add(RequestParams.State, CommonKeys.State);
                queryString.Add(RequestParams.CodeChallengeMethod, CodeChallengeMethodType.S256.ToString());
                queryString.Add(RequestParams.UsePKCE, "true");
                queryString.Add(RequestParams.Display, "in_app");

                var codeVerifier = _authUtilitiesInstance.GetCodeVerifier();
                var codeChallange = _authUtilitiesInstance.GetCodeChallenge(codeVerifier);

                queryString.Add(RequestParams.CodeChallenge, codeChallange);

                var query = queryString.ToString();

                UriBuilder uriBuilder = new UriBuilder(URLs.GetUrl(this.BaseURLProvider, URLs.AuthorisationUrl));
                uriBuilder.Query = query;

                string autherizationURL = uriBuilder.Uri.ToString();
                return autherizationURL;
            }
            catch (Exception ex)
            {
                Common.ThrowPureVPNCoreException(ErrorCodes.UnableToGenerateAutherizeURL, ex);
                return default;
            }
        }

        private void RedirectToBrowser(string authUrl, bool isCloseable = true)
        {
            this.BrowserLauncher?.OpenURL(authUrl, isCloseable);
        }

        private string GetUserScope()
        {
            StringBuilder scopeString = new StringBuilder();
            scopeString.Append(ScopeType.offline_access);

            return scopeString.ToString();
        }
        #endregion
    }
}