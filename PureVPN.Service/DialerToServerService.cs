using Newtonsoft.Json;
using PureVPN.Core.Interfaces;
using PureVPN.Entity.Models;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class DialerToServerService : IDialerToServerService
    {
        private IMixpanelService mixpanelService;

        private IAuthTokenService authTokenService;
        private readonly IBaseURLProvider baseURLProvider;
        private IFactory _authFactory;
        private IAuthenticationManager _authManager;
        private IClientAppAccessTokenService clientAppAccessTokenService;

        public static string Salt()
        {
            return System.Text.Encoding.ASCII.GetString(Convert.FromBase64String("KlBWUE4xMjMjLGo=")).Split(',')[0];
        }

        public static string LoginSalt()
        {
            return System.Text.Encoding.ASCII.GetString(Convert.FromBase64String("a0tVUjlLWiF7Ki9ZdEI/Xixq")).Split(',')[0];
        }
        public Dictionary<string, object> LoginData(string username, string password)
        {
            var loginData = WebRequestHelper.DefaultValues();
            loginData.Add(Common.RequestParameters.Username, username);
            loginData.Add(Common.RequestParameters.Password, password);
            return loginData;
        }

        public async Task<LoginReply> Login(Dictionary<string, object> loginData)
        {
            var authToken = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Authorization);
            var localPath = ServiceHelperConstants.UrlLoginWithEmail;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var headers = this.GenerateHeaders(LoginSalt(), loginData[Common.RequestParameters.Username].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, loginData, AuthorizeToken: authToken.Token, headers: headers);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            return JsonConvert.DeserializeObject<LoginReply>(response?.ResponseBody);
        }

        private void SendResponseToMixpanel(string host, string localPath, double? response_time)
        {
            mixpanelService.SendResponseToMixpanel(host, localPath, response_time);
        }

        public DialerToServerService(IMixpanelService _mixpanelService, IAuthTokenService authTokenService, IBaseURLProvider baseURLProvider, IFactory authFactory, IClientAppAccessTokenService _clientAppAccessTokenService, ITelemetryService authenticationTelemetryService)
        {
            this.mixpanelService = _mixpanelService;
            this.authTokenService = authTokenService;
            this.baseURLProvider = baseURLProvider;
            this._authFactory = authFactory;
            this._authManager = _authFactory.GetAuthenticationManager(baseURLProvider, authenticationTelemetryService, Common.BrowserLauncher);
            this.clientAppAccessTokenService = _clientAppAccessTokenService;
        }

        public async Task<LoginReply> GetAuthenticationKeyByUserName(string username, string uuid)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Authorization);
            var localPath = ServiceHelperConstants.UrlGetAccountInformation;

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Uuid, uuid);
            postData.Add(Common.RequestParameters.Username, username);

            var headers = GenerateHeaders(LoginSalt(), username);

            var response = await WebRequestHelper.GetRequest(baseURLProvider.GetServicesUrls(), localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);

            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<LoginReply>(response?.ResponseBody);
        }

        public async Task<DevicesReply> GetSetupOtherDevices()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Device);
            var localPath = ServiceHelperConstants.UrlGetSetupOtherDevices;
            var postData = WebRequestHelper.DefaultValues();
            var baseUrls = baseURLProvider.GetServicesUrls();
            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());
            var response = await WebRequestHelper.GetRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);

            return JsonConvert.DeserializeObject<DevicesReply>(response?.ResponseBody);
        }
        public async Task<MigrateUserToEmail> GetMigrateUserToEmail(string username, string uuid)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Authorization);
            var localPath = ServiceHelperConstants.UrlMigrateUserToEmail;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Uuid, uuid);

            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);

            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<MigrateUserToEmail>(response?.ResponseBody);
        }

        /// <summary>
        /// Gets token for Firestore SDK authentication
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="username">Username</param>
        /// <returns></returns>
        public async Task<FirestoreTokenReply> GetFirestoreTokenAsync(string uuid, string username)
        {

            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Authorization);

            var localPath = ServiceHelperConstants.UrlGetFirestoreToken;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Uuid, uuid);
            postData.Add(Common.RequestParameters.Username, username);

            var headers = GenerateHeaders(LoginSalt(), username);

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<FirestoreTokenReply>(response?.ResponseBody);
        }

        public async Task<AppSurvey> GetAppContent()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.Device);
            var localPath = ServiceHelperConstants.UrlGetAppSurveyContent;

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            //  postData.Add(Common.RequestParameters.APIKey, "pvpnUserPrd");

            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());

            // string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // postData.Add(Common.DXNRequestParameters.DateTime, dt);
            // postData.Add(Common.DXNRequestParameters.Signature, Utilities.GetMD5(dt + Salt()));

            var response = await WebRequestHelper.GetRequest(baseURLProvider.GetServicesUrls(), localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);

            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<AppSurvey>(response?.ResponseBody);
        }

        public async Task<ForceUpdateReply> GetForceUpdate()
        {
            var token = await this.clientAppAccessTokenService.GetAuthToken(Common.AccessTokenScopes.Device);
            var localPath = ServiceHelperConstants.UrlGetForceUpdate;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();

            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);

            return JsonConvert.DeserializeObject<ForceUpdateReply>(response?.ResponseBody);
        }

        public async Task<DedicatedIP> GetDedicatedIpInfo()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var baseUrls = baseURLProvider.GetServicesUrls();
            var localPath = ServiceHelperConstants.UrlGetDedicatedIpInfo;

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);

            var response = await WebRequestHelper.GetRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<DedicatedIP>(response?.ResponseBody);
        }

        public async Task<DedicatedIP> KillDedicatedIpSession(string DedicatedIP)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlKillDedicatedIpSession;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);
            postData.Add(Common.RequestParameters.ServerIP, DedicatedIP);

            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, true, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);

            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<DedicatedIP>(response?.ResponseBody);
        }

        public async Task<TicketReply> CreateTicketInZendesk(string sMessage)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlCreateTicketInZendesk;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Message, sMessage);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);

            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<TicketReply>(response?.ResponseBody);
        }

        public async Task<ProfileReply> GetUserProfile()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);
            var localPath = ServiceHelperConstants.UrlGetUserProfile;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);
            var response = await WebRequestHelper.GetRequest(baseUrls, localPath, postData, token.Token, headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<ProfileReply>(response?.ResponseBody);
        }

        public async Task<ReferAFriend> GetReferAFrinedCountAndLink()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);
            var localPath = ServiceHelperConstants.UrlReferAFriend;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);

            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<ReferAFriend>(response?.ResponseBody);
        }

        public async Task<TicketReply> RecordDesireOutcome()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlRecordDesiredOutcome;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);


            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.Username].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);

            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<TicketReply>(response?.ResponseBody);
        }

        /// <summary>
        /// Mark <see cref="NotificationCenter.Infrastructure.Models.NotificationDTO"/> as Read
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task MarkNotificationAsRead(string notificationId)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlNotificationMarkRead;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);
            postData.Add(Common.RequestParameters.NotificationId, notificationId);

            //var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());
            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
        }

        /// <summary>
        /// Delete <see cref="NotificationCenter.Infrastructure.Models.NotificationDTO"/> by <paramref name="notificationId"/>
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task DeleteNotification(string notificationId)
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.DeleteNotification;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);
            postData.Add(Common.RequestParameters.NotificationId, notificationId);

            //var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.DeviceId].ToString());
            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
        }

        public async Task<SpeedtestExperiment> GetExperimentGroupOfUser()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlSpeedtestExperiment;
            var baseUrls = baseURLProvider.GetServicesUrls();
            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), postData[Common.RequestParameters.Username].ToString());

            var response = await WebRequestHelper.GetRequest(baseUrls, localPath, postData, token.Token, headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<SpeedtestExperiment>(response?.ResponseBody);
        }

        /// <summary>
        /// Gets forwarded ports <see cref="PortForwardingReply"/>
        /// </summary>
        /// <returns></returns>
        public async Task<PortForwardingReply> GetForwardedPorts()
        {
            var token = await this.authTokenService.GetAuthToken(Common.AccessTokenScopes.User);

            var localPath = ServiceHelperConstants.UrlPortForwarding;
            var baseUrls = baseURLProvider.GetServicesUrls();

            var postData = WebRequestHelper.DefaultValues();
            postData.Add(Common.RequestParameters.Username, AtomModel.Username);
            postData.Add(Common.RequestParameters.Uuid, AtomModel.UUID);

            var headers = GenerateHeaders(LoginSalt(), AtomModel.Username);
            var response = await WebRequestHelper.GetRequest(baseUrls, localPath, postData, AuthorizeToken: token.Token, headers: headers, authenticationManager: _authManager);
            baseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            SendResponseToMixpanel(response?.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<PortForwardingReply>(response?.ResponseBody);
        }

        /// <summary>
        /// Generates Web request headers collection
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Dictionary<string, string> GenerateHeaders(string salt, string value)
        {
            var headers = new Dictionary<string, string>();
            headers.Add(Common.RequestsHeaders.Token, Utilities.GetMD5(salt + value));
            return headers;
        }

    }
}
