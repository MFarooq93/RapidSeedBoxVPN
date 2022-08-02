using Newtonsoft.Json;
using PureVPN.Core.Interfaces;
using PureVPN.Entity.Models;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PureVPN.Service.Helper
{
    public class BaseURLProvider : IBaseURLProvider, IOAuth2BaseURLProvider
    {
        private DateTime LastFetchedTime;
        private RemoteConfigModel remoteConfig;
        private const int InMemoryCacheDurationInMinutes = 10;

        public BaseURLProvider(ICachingService cachingService, ISentryService sentryService)
        {
            CachingService = cachingService;
            SettingsProvider = Common.SettingsProvider;
            SentryService = sentryService;
        }

        public ICachingService CachingService { get; }
        public ISettingsProvider SettingsProvider { get; }
        public ISentryService SentryService { get; }

        /// <summary>
        /// Gets list of Base Urls for Auth Token API
        /// </summary>
        /// <returns></returns>
        public List<string> GetAuthTokenUrls()
        {
            var lastSuccessfulBaseUrl = SettingsProvider.LastAuthTokenUrl;
            var baseUrls = GetBaseURLs().Select(x => x.AuthTokenUrl);
            return PrepareListOfBaseURLs(lastSuccessfulBaseUrl, baseUrls);
        }

        /// <summary>
        /// Gets list of Base Urls for Service APIs
        /// </summary>
        /// <returns></returns>
        public List<string> GetServicesUrls()
        {
            var lastSuccessfulBaseUrl = SettingsProvider.LastServerServicesUrl;
            var baseUrls = GetBaseURLs().Select(x => x.ServicesUrl);
            return PrepareListOfBaseURLs(lastSuccessfulBaseUrl, baseUrls);
        }

        /// <summary>
        /// Get list of OAuth2 API Urls
        /// </summary>
        /// <returns></returns>
        public List<string> GetOAuth2APIUrls()
        {
            var lastSuccessfulBaseUrl = SettingsProvider.LastOAuth2APIUrl;
            var baseUrls = GetBaseURLs().Select(x => x.FaAPIUrl);
            return PrepareListOfBaseURLs(lastSuccessfulBaseUrl, baseUrls);
        }

        /// <summary>
        /// Get list of Login Webpage Urls
        /// </summary>
        /// <returns></returns>
        public List<string> GetRedirectionWebPageUrls()
        {
            var lastSuccessfulBaseUrl = SettingsProvider.LastSuccessfulLoginRedirectWebpageUrl;
            var baseUrls = GetBaseURLs().Select(x => x.FAWebPageURL);
            return PrepareListOfBaseURLs(lastSuccessfulBaseUrl, baseUrls);
        }

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Auth Token API
        /// </summary>
        /// <param name="baseUrl"></param>
        public void SetSuccessfulAuthTokenUrl(string baseUrl)
        {
            SettingsProvider.LastAuthTokenUrl = baseUrl;
        }

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Service APIs
        /// </summary>
        /// <param name="baseUrl"></param>
        public void SetSuccessfulServerServicesUrl(string baseUrl)
        {
            if (String.IsNullOrWhiteSpace(baseUrl) == false)
            {
                SettingsProvider.LastServerServicesUrl = baseUrl;
            }
        }

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for OAuth2 APIs
        /// </summary>
        /// <param name="baseUrl"></param>
        public void SetSuccessfulOAuth2APIUrl(string baseUrl)
        {
            if (String.IsNullOrWhiteSpace(baseUrl) == false)
            {
                SettingsProvider.LastOAuth2APIUrl = baseUrl;
            }
        }

        /// <summary>
        /// Set successful <paramref name="baseUrl"/> for Login webpage
        /// </summary>
        /// <param name="baseUrl"></param>
        public void SetSuccessfulLoginRedirectionWebpageUrl(string baseUrl)
        {
            if (String.IsNullOrWhiteSpace(baseUrl) == false)
            {
                SettingsProvider.LastSuccessfulLoginRedirectWebpageUrl = baseUrl;
            }
        }

        /// <summary>
        /// Get list of <see cref="URLModel"/> from <see cref="RemoteConfigModel"/>
        /// </summary>
        /// <returns></returns>
        private List<URLModel> GetBaseURLs()
        {
            try
            {
                if (remoteConfig == null || DateTime.UtcNow > LastFetchedTime.AddMinutes(10))
                {
                    var json = CachingService.GetContent(Common.CachedFiles.RemoteConfig);
                    LastFetchedTime = DateTime.UtcNow;
                    remoteConfig = JsonConvert.DeserializeObject<RemoteConfigModel>(json);
                    if (remoteConfig?.BaseUrls?.Count <= default(int))
                    {
                        throw new Exception($"No baseUrls found in remote config");
                    }
                }
                return remoteConfig.BaseUrls;
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
            }

            // Return a default list of Base URLs
            return new List<URLModel>
            {
                new URLModel
                {
                    AuthTokenUrl = Service.Helper.ServiceHelperConstants.DefaultAuthTokenBaseUrl,
                    ServicesUrl = Service.Helper.ServiceHelperConstants.DefaultServicesBaseUrl,
                    FaAPIUrl = Service.Helper.ServiceHelperConstants.DefaultFAAPIBaseUrl,
                    FAWebPageURL = Service.Helper.ServiceHelperConstants.DefaultFAWebpageBaseUrl
                }
            };
        }

        private List<string> PrepareListOfBaseURLs(string lastSuccessfulBaseUrl, IEnumerable<string> baseUrls)
        {
            var list = new List<string>();
            if (String.IsNullOrWhiteSpace(lastSuccessfulBaseUrl) == false)
            {
                list.Add(lastSuccessfulBaseUrl);
            }
            foreach (var item in baseUrls)
            {
                if (list.Contains(item) == false)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
