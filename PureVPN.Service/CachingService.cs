using Newtonsoft.Json;
using PureVPN.Descifrar;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Security.Cryptography;
using PureVPN.Descifrar.Infrastructure;

namespace PureVPN.Service
{
    /// <summary>
    /// Provide methods to maintain cache for views
    /// </summary>
    public class CachingService : ICachingService, IDisposable
    {
        #region ctor & fields/properties

        /// <summary>
        /// Secret key for encryption. Base64 encoded
        /// </summary>
        private const string _FACTOR = "OWYxMzAzNDcyOGE2NDM1ZTllMmVkMjliNmUyMjIzNDc=";

        /// <summary>
        /// Directory for keeping the cached files
        /// </summary>
        private const string _DIRECTORY = "CachedFiles";
        /// <summary>
        /// Filename to provide rules to cache the files
        /// </summary>
        private const string _RULES_FILES = "CachingRules";
        /// <summary>
        /// Interval to check and update if there are any cached files due to update
        /// </summary>
        private const double _INTERVAL_IN_MINUTES = 5;

        /// <summary>
        /// In memory history of cached files
        /// </summary>
        private Dictionary<string, DateTime> _History = new Dictionary<string, DateTime>();


        private List<CachingRuleModel> _CachingRules;
        /// <summary>
        /// Configured rules applied to cached files
        /// </summary>
        protected List<CachingRuleModel> CachingRules
        {
            get
            {
                if (_CachingRules == null || _CachingRules.Count == default(int))
                {
                    _CachingRules = GetCachingRules();
                }
                return _CachingRules ?? new List<CachingRuleModel>();
            }
        }

        /// <summary>
        /// Error Logger
        /// </summary>
        public ISentryService SentryService { get; }
        public ISymmetricManager CryptoHelper { get; }

        /// <summary>
        /// Timer to check and update if there are any cached files due for update
        /// </summary>
        private Timer _Timer;

        public CachingService(ISentryService sentryService, ISymmetricManager cryptoManager)
        {
            SentryService = sentryService;
            CryptoHelper = cryptoManager;
        }
        #endregion

        #region ICachingService implementations
        /// <summary>
        /// Get content by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id for content</param>
        /// <returns>Content from cached file for <paramref name="id"/></returns>
        public void Initialize()
        {
            foreach (var item in CachingRules)
            {
                _History[item.Id] = new DateTime();
            }
            UpdateCache();
            InitializeTimer();
        }

        /// <summary>
        /// Initializes the caching mehanism
        /// </summary>
        public string GetContent(string id)
        {
            return GetFileContent(name: id);
        }
        #endregion


        #region private methods

        /// <summary>
        /// Updates all those files in cache which are due
        /// </summary>
        private void UpdateCache()
        {
            if (CachingRules == null)
            {
                return;
            }
            foreach (var item in CachingRules)
            {
                DateTime lastUpdatedOn;
                _History.TryGetValue(item.Id, out lastUpdatedOn);
                if (DateTime.UtcNow >= lastUpdatedOn.AddMinutes(item.CachingDurationInMinutes))
                {
                    UpdateCachedFile(item);
                }
            }
        }

        /// <summary>
        /// Get file content from cached file
        /// </summary> 
        /// <param name="name"></param>
        private string GetFileContent(string name)
        {
            string content = null;
            var filepath = GetAbsolutePath(name);
            if (File.Exists(filepath))
            {
                var plainContent = File.ReadAllText(filepath);
                if (!String.IsNullOrWhiteSpace(plainContent))
                {
                    content = Decrypt(plainContent);
                }
            }
            return content;
        }

        /// <summary>
        /// Get absulute path for <paramref name="name"/>
        /// </summary>
        /// <param name="name">Filename</param>
        private string GetAbsolutePath(string name)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(directory, _DIRECTORY, name);
        }

        /// <summary>
        /// Update cached file from <see cref="CachingRuleModel.Url"/>
        /// </summary>
        /// <param name="cachingRule"><see cref="CachingRuleModel"/> which describes the file to update</param>
        private async void UpdateCachedFile(CachingRuleModel cachingRule)
        {
            try
            {
                var response = await WebRequestHelper.GetRequest(cachingRule.Url, content: null);
                var fileContent = response.Item1;
                if (!String.IsNullOrWhiteSpace(fileContent))
                {
                    if (cachingRule.IsAlreadyEncrypted.GetValueOrDefault() == false)
                    {
                        fileContent = Encrypt(fileContent);
                    }
                    File.WriteAllText(GetAbsolutePath(cachingRule.Id), fileContent);
                    _History[cachingRule.Id] = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
            }
        }

        /// <summary>
        /// Get all <see cref="CachingRuleModel"/>s
        /// </summary>
        /// <returns></returns>
        private List<CachingRuleModel> GetCachingRules()
        {
            string json;
            List<CachingRuleModel> cachingRules = null;
            try
            {
                var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _RULES_FILES);

                if (File.Exists(filepath) && !String.IsNullOrWhiteSpace(json = File.ReadAllText(filepath)))
                {
                    json = CryptoHelper.Decrypt(json, Encoding.ASCII.GetString(Convert.FromBase64String(_FACTOR)));
                    cachingRules = JsonConvert.DeserializeObject<List<CachingRuleModel>>(json);
                }
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
            }

            return cachingRules;
        }

        /// <summary>
        /// Encrypts <paramref name="plain"/>text and returns cipher text
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        private string Encrypt(string plain)
        {
            return CryptoHelper.Encrypt(plain, Encoding.ASCII.GetString(Convert.FromBase64String(_FACTOR)));
        }

        /// <summary>
        /// Decrypts <paramref name="cipher"/>text and get plain text
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns></returns>
        private string Decrypt(string cipher)
        {
            return CryptoHelper.Decrypt(cipher, Encoding.ASCII.GetString(Convert.FromBase64String(_FACTOR)));
        }
        #endregion

        #region Timer related methods

        /// <summary>
        /// Disposes <see cref="CachingService._Timer"/>
        /// </summary>
        private void DisposeTimer()
        {
            if (this._Timer != null)
            {
                if (this._Timer.Enabled)
                {
                    this._Timer.Stop();
                }
                this._Timer.Dispose();
            }
        }

        /// <summary>
        /// Initializes the <see cref="CachingService._Timer"/> to check and update if there are any cached files due to update
        /// </summary>
        private void InitializeTimer()
        {
            DisposeTimer();
            if (_Timer == null)
            {
                _Timer = new Timer();
                _Timer.Interval = TimeSpan.FromMinutes(_INTERVAL_IN_MINUTES).TotalMilliseconds;
                _Timer.Elapsed += (s, e) => UpdateCache();
                _Timer.Start();
            }
        }
        #endregion

        #region IDisposable implementations
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                DisposeTimer();
            }
            catch (Exception ex)
            {
                //SentryService.LoggingException(ex);
            }
        }
        #endregion
    }
}
