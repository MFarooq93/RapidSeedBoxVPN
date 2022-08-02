using Newtonsoft.Json;
using PureVPN.Core.Exceptions;
using PureVPN.Core.Extensions;
using PureVPN.Core.Helper;
using PureVPN.Core.Interfaces;
using PureVPN.Core.Models;
using PureVPN.Core.PureVPN.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.NetworkHelper
{
    internal class WebRequestHelper
    {
        private const int _REQ_TIMEOUT_MILLISECONDS = 10000;
        private const string _BEARER = "Bearer";
        internal static IOAuth2BaseURLProvider OAuth2BaseURLProvider;

        internal static async Task<T> PostAsync<T>(string requestUri, Dictionary<string, string> content = null, string token = null, ITelemetryService telemetryService = null)
        {
            string baseUrl = null;
            HttpResponseMessage response = null;
            List<string> alreadyTriedUrls = new List<string>();
            var baseAddresses = OAuth2BaseURLProvider.GetOAuth2APIUrls() ?? new List<string>();

            for (int i = 0; i < baseAddresses.Count; i++)
            {
                baseUrl = baseAddresses.ElementAtOrDefault(i);
                if (alreadyTriedUrls.Contains(baseUrl)) { continue; }

                try
                {
                    var client = new HttpClient();

                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromMilliseconds(_REQ_TIMEOUT_MILLISECONDS);

                    var httpContent = new FormUrlEncodedContent(content);

                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_BEARER, token);
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

                    alreadyTriedUrls.Add(baseUrl);
                    response = await client.PostAsync(requestUri, httpContent);
                    SendAPIFailEventIfRequired(telemetryService, response);
                    break;
                }
                catch (Exception ex)
                {
                    if (i >= baseAddresses.Count - 1)
                    {
                        throw;
                    }
                }
            }
            OAuth2BaseURLProvider.SetSuccessfulOAuth2APIUrl(baseUrl);
            var responseContent = await response?.Content?.ReadAsStringAsync();
            var responseResult = JsonConvert.DeserializeObject<T>(responseContent);

            CheckForErrorInResponse(responseResult);
            return responseResult;
        }

        internal static async Task<T> GetAsync<T>(string requestUri, string token = null, ITelemetryService telemetryService = null)
        {
            string baseUrl = null;
            HttpResponseMessage response = null;
            List<string> alreadyTriedUrls = new List<string>();
            var baseAddresses = OAuth2BaseURLProvider.GetOAuth2APIUrls() ?? new List<string>();

            for (int i = 0; i < baseAddresses.Count; i++)
            {
                baseUrl = baseAddresses.ElementAtOrDefault(i);
                if (alreadyTriedUrls.Contains(baseUrl))
                {
                    continue;
                }

                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromMilliseconds(_REQ_TIMEOUT_MILLISECONDS);

                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_BEARER, token);
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

                    response = await client.GetAsync(requestUri);
                    SendAPIFailEventIfRequired(telemetryService, response);
                    break;
                }
                catch (Exception ex)
                {
                    if (i >= baseAddresses.Count - 1)
                    {
                        throw;
                    }
                }
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseResult = JsonConvert.DeserializeObject<T>(responseContent);

            CheckForErrorInResponse(responseResult);
            return responseResult;
        }

        /// <summary>
        /// Makes a web request with DELETE http method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="token"></param>
        /// <param name="telemetryService"></param>
        /// <returns></returns>
        internal static async Task<T> DeleteAsync<T>(string requestUri, Dictionary<string, object> parameters, string token = null, ITelemetryService telemetryService = null)
        {
            string baseUrl = null;
            HttpResponseMessage response = null;
            List<string> alreadyTriedUrls = new List<string>();
            var baseAddresses = OAuth2BaseURLProvider.GetOAuth2APIUrls() ?? new List<string>();

            for (int i = 0; i < baseAddresses.Count; i++)
            {
                baseUrl = baseAddresses.ElementAtOrDefault(i);
                if (alreadyTriedUrls.Contains(baseUrl))
                {
                    continue;
                }

                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromMilliseconds(_REQ_TIMEOUT_MILLISECONDS);

                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_BEARER, token);
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

                    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    if (parameters != null && parameters.Count > 0)
                        foreach (var x in parameters)
                            query.Add(x.Key, x.Value?.ToString());

                    var uri = $"{requestUri}?{query}";
                    response = await client.DeleteAsync(uri);
                    SendAPIFailEventIfRequired(telemetryService, response);
                    break;
                }
                catch (Exception ex)
                {
                    if (i >= baseAddresses.Count - 1)
                    {
                        throw;
                    }
                }
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseResult = JsonConvert.DeserializeObject<T>(responseContent);
            return responseResult;
        }

        /// <summary>
        /// Trigger <see cref="ITelemetryService"/> to send API fail event
        /// </summary>
        /// <param name="telemetryService"></param>
        /// <param name="httpResponse"></param>
        /// <returns>Returns <see cref="true"/></returns>
        private static bool SendAPIFailEventIfRequired(ITelemetryService telemetryService, HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode == false)
            {
                telemetryService.SendAPIFailed(httpResponse);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets <see cref="FormUrlEncodedContent"/> from <see cref="Dictionary{String, String}"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static FormUrlEncodedContent ToFormUrlConcodedContent(Dictionary<string, string> content)
        {
            return new FormUrlEncodedContent(content);
        }

        private static void CheckForErrorInResponse(object responseResult)
        {
            if (responseResult.GetType() == new Authentication().GetType())
            {
                var autherizationToken = (Authentication)Convert.ChangeType(responseResult, typeof(Authentication));

                if (autherizationToken.Error.IsStingNotNullorEmpty())
                    throw new Exception($"ErrorType: {autherizationToken.Error}, " +
                                        $"ErrorDescription: {autherizationToken.ErrorDescription}");
            }
            else if (responseResult.GetType() == new UserInfoBody().GetType())
            {
                var userInfo = (UserInfoBody)Convert.ChangeType(responseResult, typeof(UserInfoBody));

                if (userInfo.Error.IsStingNotNullorEmpty())
                    throw new Exception($"ErrorType: {userInfo.Error}, " +
                                        $"ErrorDescription: {userInfo.ErrorDescription}");
            }

            return;
        }
    }
}
