using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Exceptions
{
    internal class ErrorCodes
    {
        internal const int UnableToGetAccessToken = 8001;
        internal const int UnableToGenerateAccessToken = 8002;
        internal const int UnableToGenerateAutherizeURL = 8003;
        internal const int UnableToRedirectToBrowser = 8004;
        internal const int AccessTokenUnableToGetResponseFromAPI = 8005;
        internal const int AccessTokenJsonSerializationException = 8006;
        internal const int AccessTokenWebException = 8007;
        internal const int RefreshTokenNotFound = 8009;
        internal const int AccessTokenNotFound = 8010;
        internal const int GeneratingAccessToken = 8011;
        internal const int UnableToGetUserInfo = 8012;
        internal const int GivenTokenIsNull = 8013;
        internal const int AuthGrantCodeIsNull = 8014;
        internal const int UnableToGenerateLogoutURL = 8015;
        internal const int UnableToRedirectToBrowserWithLogoutUrl = 8016;
        internal const int GetNullFromAcessToken = 8017;
        internal const int RefreshAccessTokenJsonSerializationException = 8018;
        internal const int RefreshAccessTokenWebException = 8019;
        internal const int UnableToGetResponseFromRefreshToken = 8020;
        internal const int GetNullFromRefreshToken = 8021;
        internal const int UnableToGenerateRefreshToken = 8022;
        internal const int InvalidRefreshTokenRequest = 8023;
        internal const int UserInfoJsonSerializationException = 8024;
        internal const int UserInfoWebException = 8025;
        internal const int UnableToGetRepsonseFromUserInfoApi = 8026;
        internal const int UnableToGetUserInfoFromJson = 8027;
    }
}
