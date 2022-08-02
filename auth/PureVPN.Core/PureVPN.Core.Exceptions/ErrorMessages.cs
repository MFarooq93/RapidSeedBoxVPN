using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Exceptions
{
    internal class ErrorMessages
    {
        internal static string GetErrorMessage(int errorCode)
        {
            try
            {
                if (ErrorCodesDictionary.ContainsKey(errorCode)) 
                {
                    return ErrorCodesDictionary[errorCode];
                }
            }
            catch
            {
                
            }
            return "General exception please check inner exception - " + errorCode;
        }

        private static Dictionary<int, string> ErrorCodesDictionary = new Dictionary<int, string>()
        {
            { ErrorCodes.UnableToGetAccessToken, "Unable to get access token"},
            { ErrorCodes.UnableToGenerateAccessToken, "Unable to generate access token"},
            { ErrorCodes.UnableToGenerateAutherizeURL, "Unable to generate autherize URL"},
            { ErrorCodes.UnableToRedirectToBrowser, "Unable to redirect to browser"},
            //{ ErrorCodes.UnableToGetResponseFromAPI, "unable to get api response"},
            //{ ErrorCodes.JsonSerializationException, "Json serialization error"},
            //{ ErrorCodes.WebException, "Getting error while processing the request"},
            { ErrorCodes.RefreshTokenNotFound, "Refresh token not found, Please initialize the token request from start"},
            { ErrorCodes.AccessTokenNotFound, "Access token not found, please intialialize token request again"},
            { ErrorCodes.GeneratingAccessToken, "Access token is generating please try to get after sometime"},
            { ErrorCodes.UnableToGetUserInfo, "Unable to get user info"},
            { ErrorCodes.GivenTokenIsNull, "The provided token is null"},
            { ErrorCodes.AuthGrantCodeIsNull, "Auth grant code is null or empty" }       
        };
    }
}
