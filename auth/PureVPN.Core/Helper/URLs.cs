using PureVPN.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Helper
{
    internal class URLs
    {
        private const string AuthorisationUrlEndpoint = "oauth2/authorize";
        private const string PostAuthorisationCallbackEndpoint = "callbacks/windows-in-app";

        private const string TokenUrlEndPoint = "oauth2/token";
        private const string UserInfoUrlEndPoint = "oauth2/userinfo";

        /// <summary>
        /// Endpoint to logout user
        /// </summary>
        private const string LogoutEndpoint = "oauth2/logout";
        /// <summary>
        /// Redirector
        /// </summary>
        private const string PostLogoutCallbackEndpoint = "callbacks/windows-app/logout-in-app";

        /// <summary>
        /// Endpoint to Revoke refresh token API
        /// </summary>
        internal const string RevokeRefreshTokenEndpoint = "api/jwt/refresh";

        internal static string AuthorisationUrl => AuthorisationUrlEndpoint;
        internal static string CallbackAuthorisation => PostAuthorisationCallbackEndpoint;

        internal static string LogoutUrl => LogoutEndpoint;
        internal static string CallbackLogout => PostLogoutCallbackEndpoint;

        internal static string TokenUrl => TokenUrlEndPoint;
        internal static string UserInfoUrl => UserInfoUrlEndPoint;

        /// <summary>
        /// Gets complete URL for OAuth2 API
        /// </summary>
        /// <param name="baseURLProvider"></param>
        /// <param name="fragment"></param>
        /// <returns></returns>
        internal static string GetUrl(IOAuth2BaseURLProvider baseURLProvider, string fragment)
        {
            return baseURLProvider.GetOAuth2APIUrls().FirstOrDefault() + fragment;
        }

        /// <summary>
        /// Gets complete URL for redirection webpages
        /// </summary>
        /// <param name="baseURLProvider"></param>
        /// <param name="fragment"></param>
        /// <returns></returns>
        internal static string GetRedirectorUrl(IOAuth2BaseURLProvider baseURLProvider, string fragment)
        {
            return baseURLProvider.GetRedirectionWebPageUrls().FirstOrDefault() + fragment;
        }
    }
}
