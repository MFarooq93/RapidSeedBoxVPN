using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Interfaces
{
    public interface IOAuth2BaseURLProvider
    {
        /// <summary>
        /// Get list of OAuth2 API Urls
        /// </summary>
        /// <returns></returns>
        List<string> GetOAuth2APIUrls();

        /// <summary>
        /// Get list of Redirection callback webpage Urls
        /// </summary>
        /// <returns></returns>
        List<string> GetRedirectionWebPageUrls();
        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for OAuth2 APIs
        /// </summary>
        /// <param name="baseUrl"></param>
        void SetSuccessfulOAuth2APIUrl(string baseUrl);

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Login callback redirection webpage
        /// </summary>
        /// <param name="baseUrl"></param>
        void SetSuccessfulLoginRedirectionWebpageUrl(string baseUrl);
    }
}