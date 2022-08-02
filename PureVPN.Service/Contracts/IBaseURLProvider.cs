using PureVPN.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    public interface IBaseURLProvider : IOAuth2BaseURLProvider
    {
        /// <summary>
        /// Gets list of Base Urls for Auth Token API
        /// </summary>
        /// <returns></returns>
        List<string> GetAuthTokenUrls();
        /// <summary>
        /// Gets list of Base Urls for Service APIs
        /// </summary>
        /// <returns></returns>
        List<string> GetServicesUrls();

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Auth Token API
        /// </summary>
        /// <param name="baseUrl"></param>
        void SetSuccessfulAuthTokenUrl(string baseUrl);

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Service APIs
        /// </summary>
        /// <param name="baseUrl"></param>
        void SetSuccessfulServerServicesUrl(string baseUrl);
    }
}
