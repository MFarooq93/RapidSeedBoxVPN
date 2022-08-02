using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PureVPN.Entity.Models.DTO;
using System.Net;
using PureVPN.Core.Interfaces;

namespace PureVPN.Service.Helper
{
    public class WebRequestHelper
    {
        private const string DEVICE_TYPE = "windows";

        private static string _DeviceId;
        private static string DeviceId
        {
            get
            {
                if (String.IsNullOrEmpty(_DeviceId) || _DeviceId == "-2")
                    _DeviceId = Utilities.GetHardwareId();

                return _DeviceId;
            }
        }
        private const string UnauthorizedAccessErrorMessage = "401 (Unauthorized)";


        public static async Task<ApiResponseMetrics> PostRequest(List<string> BaseUrls, string uri, Dictionary<string, object> content, bool Timeout = false,
                                                                 string AuthorizeToken = "", Dictionary<string, string> headers = null,
                                                                 IAuthenticationManager authenticationManager = null, int isTryWithNewToken = 1)
        {
            var apiMetrics = new ApiResponseMetrics();
            BaseUrls = BaseUrls ?? new List<string>();
            List<string> alreadyTriedBaseUrls = new List<string>();

            for (int i = 0; i < BaseUrls.Count; i++)
            {
                try
                {
                    apiMetrics.ResponseBody = null;
                    apiMetrics.ResponseTime = default(double);

                    string baseUrl = BaseUrls.ElementAtOrDefault(i);
                    if (alreadyTriedBaseUrls.Contains(baseUrl))
                    {
                        continue;
                    }
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        if (!string.IsNullOrEmpty(AuthorizeToken))
                            client.DefaultRequestHeaders.Add(Common.RequestsHeaders.Authorization, AuthorizeToken);

                        if (headers != null)
                        {
                            foreach (var item in headers)
                            {
                                client.DefaultRequestHeaders.Add(item.Key, item.Value);
                            }
                        }

                        if (Timeout)
                            client.Timeout = TimeSpan.FromSeconds(15);

                        var serialized = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                        var stopWatch = Stopwatch.StartNew();
                        using (HttpResponseMessage response = await client.PostAsync(uri, serialized))
                        {
                            var contentstring = JsonConvert.SerializeObject(content);
                            var headerstring = JsonConvert.SerializeObject(headers);
                            var url = uri;

                            response.EnsureSupportedStatusCode();
                            apiMetrics.ResponseBody = await response.Content.ReadAsStringAsync();
                            apiMetrics.ResponseTime = stopWatch.Elapsed.TotalMilliseconds;
                            stopWatch.Stop();
                            apiMetrics.SuccessfulBaseUrl = baseUrl;

                            break;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    //If token is invalid new token will create and recall the method
                    if (isTryWithNewToken < 4 && ex.Message.Contains(UnauthorizedAccessErrorMessage))
                    {
                        await authenticationManager.GenerateTokenFromRefreshToken(authenticationManager.GetAccessToken().Result?.RefreshToken);
                        var authToken = await authenticationManager.GetAccessToken();

                        //We will retry 3 times
                        isTryWithNewToken = isTryWithNewToken + 1;
                        return await PostRequest(BaseUrls: BaseUrls, uri: uri, content: content, AuthorizeToken: authToken.Token, headers: headers,
                                                    authenticationManager: authenticationManager, isTryWithNewToken: isTryWithNewToken);
                    }

                    if (i >= (BaseUrls.Count - 1))
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    if (i >= (BaseUrls.Count - 1))
                    {
                        throw;
                    }
                }
            }
            return apiMetrics;
        }

        public static async Task<ApiResponseMetrics> PostRequestAsFormUrlEncoded(List<string> BaseUrls, string uri, List<KeyValuePair<string, string>> content, bool Timeout = false)
        {
            var apiMetrics = new ApiResponseMetrics();
            BaseUrls = BaseUrls ?? new List<string>();
            List<string> alreadyTriedBaseUrls = new List<string>();

            for (int i = 0; i < BaseUrls.Count; i++)
            {
                try
                {
                    apiMetrics.ResponseBody = null;
                    apiMetrics.ResponseTime = default(double);

                    string baseUrl = BaseUrls.ElementAtOrDefault(i);
                    if (alreadyTriedBaseUrls.Contains(baseUrl))
                    {
                        continue;
                    }
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);
                        client.DefaultRequestHeaders.ExpectContinue = false;

                        if (Timeout)
                            client.Timeout = TimeSpan.FromSeconds(15);

                        HttpContent httpcontent = new FormUrlEncodedContent(content);

                        httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        httpcontent.Headers.ContentType.CharSet = "UTF-8";

                        var stopWatch = Stopwatch.StartNew();
                        using (HttpResponseMessage response = await client.PostAsync(uri, httpcontent))
                        {
                            Extensions.EnsureSuccessStatusCode(response);
                            apiMetrics.ResponseBody = await response.Content.ReadAsStringAsync();
                            apiMetrics.ResponseTime = stopWatch.Elapsed.TotalMilliseconds;
                            stopWatch.Stop();
                            apiMetrics.SuccessfulBaseUrl = baseUrl;
                            break;
                        }
                    }
                }
                catch
                {
                    if (i >= (BaseUrls.Count - 1))
                    {
                        throw;
                    }
                }
            }
            return apiMetrics;

        }

        /// <summary>
        /// Get request without authentication
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <param name="authenticationManager"></param>
        /// <returns></returns>
        public static async Task<Tuple<string, double>> GetRequest(string uri, Dictionary<string, object> content, Dictionary<string, string> headers = null, IAuthenticationManager authenticationManager = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var param = string.Empty;
                if (content != null && content.Count > 0)
                    param = $"?{(string.Join("&", content.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))))}";

                var stopWatch = Stopwatch.StartNew();
                using (HttpResponseMessage response = await client.GetAsync($"{uri}{param}"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    double apiResponseTime = stopWatch.Elapsed.TotalMilliseconds;
                    stopWatch.Stop();

                    return new Tuple<string, double>(responseBody, apiResponseTime);
                }
            }
        }

        /// <summary>
        /// Makes HTTP GET request on list of Base URLs
        /// </summary>
        /// <param name="BaseUrls">List of Base URLs</param>
        /// <param name="uri">Relative URL</param>
        /// <param name="content">Request Params</param>
        /// <param name="AuthorizeToken">Authorize Token</param>
        /// <param name="headers">Request Headers</param>
        /// <returns></returns>
        public static async Task<ApiResponseMetrics> GetRequest(List<string> BaseUrls, string uri, Dictionary<string, object> content, string AuthorizeToken = "",
                                                                Dictionary<string, string> headers = null, IAuthenticationManager authenticationManager = null,
                                                                int isTryWithNewToken = 1)
        {
            var apiMetrics = new ApiResponseMetrics();
            BaseUrls = BaseUrls ?? new List<string>();
            List<string> alreadyTriedBaseUrls = new List<string>();

            for (int i = 0; i < BaseUrls.Count; i++)
            {
                try
                {
                    apiMetrics.ResponseBody = null;
                    apiMetrics.ResponseTime = default(double);

                    string baseUrl = BaseUrls.ElementAtOrDefault(i);
                    if (alreadyTriedBaseUrls.Contains(baseUrl))
                    {
                        continue;
                    }
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);
                        alreadyTriedBaseUrls.Add(baseUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        if (!string.IsNullOrEmpty(AuthorizeToken))
                            client.DefaultRequestHeaders.Add(Common.RequestsHeaders.Authorization, AuthorizeToken);

                        if (headers != null)
                        {
                            foreach (var item in headers)
                            {
                                client.DefaultRequestHeaders.Add(item.Key, item.Value);
                            }
                        }

                        var param = string.Empty;
                        if (content != null && content.Count > 0)
                            param = $"?{(string.Join("&", content.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))))}";

                        var stopWatch = Stopwatch.StartNew();
                        using (HttpResponseMessage response = await client.GetAsync($"{uri}{param}"))
                        {
                            response.EnsureSupportedStatusCode();
                            apiMetrics.ResponseBody = await response.Content.ReadAsStringAsync();
                            apiMetrics.ResponseTime = stopWatch.Elapsed.TotalMilliseconds;
                            stopWatch.Stop();
                            apiMetrics.SuccessfulBaseUrl = baseUrl;

                            break;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    //If token is invalid new token will create and recall the method
                    if (isTryWithNewToken < 4 && ex.Message.Contains(UnauthorizedAccessErrorMessage))
                    {
                        await authenticationManager.GenerateTokenFromRefreshToken(authenticationManager.GetAccessToken().Result?.RefreshToken);
                        var authToken = await authenticationManager.GetAccessToken();

                        //We will retry 3 times
                        isTryWithNewToken = isTryWithNewToken + 1;
                        return await GetRequest(BaseUrls: BaseUrls, uri: uri, content: content, AuthorizeToken: authToken.Token, headers: headers,
                                                authenticationManager: authenticationManager, isTryWithNewToken: isTryWithNewToken);
                    }

                    if (i >= (BaseUrls.Count - 1))
                    {
                        throw;
                    }
                }
                catch
                {
                    if (i >= (BaseUrls.Count - 1))
                    {
                        throw;
                    }
                }
            }
            return apiMetrics;
        }

        /// <summary>
        /// Describes whehter <paramref name="uri"/> is accessible
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<bool> IsAccessible(string uri)
        {
            try
            {
                var timeoutInSeconds = 4;
                var request = WebRequest.Create(uri);
                request.Method = "HEAD";
                request.Timeout = timeoutInSeconds * 1000;
                var response = await Task.Run(() => (HttpWebResponse)request.GetResponse());
                return response.StatusCode.IsSuccessStatusCode();
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Default values for Gateway APIs
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> DefaultValues()
        {
            var defaultData = new Dictionary<string, object>
            {
                { Common.RequestParameters.DeviceType, DEVICE_TYPE },
                { Common.RequestParameters.DeviceId, DeviceId },
                { Common.RequestParameters.AppVersion, Common.ProductVersion },
                { Common.RequestParameters.ReleaseType, Common.ReleaseType },
                { Common.RequestParameters.DeviceModel, DEVICE_TYPE },
                {Common.RequestParameters.Locale, Service.Helper.Common.AppLanguageLocale }
            };
            return defaultData;
        }

        /// <summary>
        /// Default values for DXN APIs
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> DefaultValuesForDXNAPIs()
        {
            var defaultData = new Dictionary<string, object>();
            defaultData.Add(Common.DXNRequestParameters.DeviceType, DEVICE_TYPE);
            defaultData.Add(Common.DXNRequestParameters.DeviceId, DeviceId);
            defaultData.Add(Common.DXNRequestParameters.AppVersion, Common.ProductVersion);
            defaultData.Add(Common.DXNRequestParameters.ReleaseType, Common.ReleaseType);
            defaultData.Add(Common.DXNRequestParameters.Locale, Service.Helper.Common.AppLanguageLocale);

            return defaultData;
        }

        public static async Task<bool> DownloadFileAsync(Uri requestUri, string filename)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                    {
                        using (
                            Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                            stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        {
                            await contentStream.CopyToAsync(stream);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Service.Helper.Sentry.LoggingException(ex);
            }
            return false;
        }
    }
}
