using Newtonsoft.Json;
using PureVPN.Entity.Models;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class MigrationService : IMigrationService
    {
        private AccessTokenReply reply;
        public MigrationService(IMixpanelService mixpanelService, IBaseURLProvider baseURLProvider)
        {
            MixpanelService = mixpanelService;
            BaseURLProvider = baseURLProvider;
        }
        public IBaseURLProvider BaseURLProvider { get; }
        public IMixpanelService MixpanelService { get; }

        public Dictionary<string, object> LoginData(string username, string password)
        {
            var loginData = WebRequestHelper.DefaultValues();
            loginData.Add(Common.RequestParameters.Username, username);
            loginData.Add(Common.RequestParameters.Password, password);
            return loginData;
        }

        public async Task<LoginReply> Login(string username, string password)
        {
            var loginData = LoginData(username,password);
            reply = await GetTokenForMigration();
            var localPath = ServiceHelperConstants.UrlLoginWithEmail;
            var baseUrls = BaseURLProvider.GetServicesUrls();
            var headers = this.GenerateHeaders(DialerToServerService.LoginSalt(), loginData[Common.RequestParameters.Username].ToString());

            var response = await WebRequestHelper.PostRequest(baseUrls, localPath, loginData, AuthorizeToken: reply.access_token, headers: headers);
            BaseURLProvider.SetSuccessfulServerServicesUrl(response.SuccessfulBaseUrl);
            return JsonConvert.DeserializeObject<LoginReply>(response?.ResponseBody);
        }

        private  async Task<AccessTokenReply> GetTokenForMigration()
        {
            var localPath = ServiceHelperConstants.UrlAuthToken;
            var baseUrls = BaseURLProvider.GetAuthTokenUrls();
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.GrantType, "client_credentials"));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.ClientId, AtomModel.AuthorizeClientId));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.ClientSecret, AtomModel.AuthorizeClientSecret));
            postData.Add(new KeyValuePair<string, string>(Common.RequestParameters.Scope, Common.AccessTokenScopes.Authorization));

            var response = await WebRequestHelper.PostRequestAsFormUrlEncoded(baseUrls, localPath, postData);
            BaseURLProvider.SetSuccessfulAuthTokenUrl(response.SuccessfulBaseUrl);
            this.MixpanelService.SendResponseToMixpanel(response.Host, localPath, response?.ResponseTime);
            return JsonConvert.DeserializeObject<AccessTokenReply>(response.ResponseBody);
        }//GetNewToken

        public async Task<MigrateTokenReply> GetTokenAfterMigration(string username,string password)
        {
            try
            {
                LoginReply LoginResponse = await Login(username, password);

                if (LoginResponse.header.response_code == 200 && LoginResponse.body?.uuid != null)
                {
                    var localPath = ServiceHelperConstants.UrlImportToken;
                    var baseUrls = BaseURLProvider.GetServicesUrls();
                    var postData = WebRequestHelper.DefaultValues();
                    var headers = GenerateHeaders(DialerToServerService.LoginSalt(), LoginResponse.body.vpn_usernames.Select(x => x.username).FirstOrDefault());
                    postData.Add(Common.RequestParameters.Uuid, LoginResponse.body.uuid);
                    postData.Add(Common.RequestParameters.Token, reply.access_token);
                    postData.Add(Common.RequestParameters.Username, LoginResponse.body.vpn_usernames.Select(x => x.username).FirstOrDefault());

                    var response = await WebRequestHelper.PostRequest(baseUrls, localPath, postData, AuthorizeToken: reply.access_token, headers: headers);
                    BaseURLProvider.SetSuccessfulAuthTokenUrl(response.SuccessfulBaseUrl);
                    this.MixpanelService.SendResponseToMixpanel(response.Host, localPath, response?.ResponseTime);
                    return JsonConvert.DeserializeObject<MigrateTokenReply>(response.ResponseBody);
                }
            }
            catch(Exception e)
            {
                
            }                
            return null;            
        }

        private Dictionary<string, string> GenerateHeaders(string salt, string value)
        {
            var headers = new Dictionary<string, string>();
            headers.Add(Common.RequestsHeaders.Token, Utilities.GetMD5(salt + value));
            return headers;
        }


    }
}
