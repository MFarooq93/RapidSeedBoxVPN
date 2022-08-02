using PureVPN.Core.Exceptions;
using PureVPN.Core.Extensions;
using PureVPN.Core.Helper;
using PureVPN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Validator
{
    internal class ValidationHelper
    {
        internal static void ValidateRefreshTokenRequest(Authentication authentication, bool isAccessTokenGenerating)
        {
            if (null == authentication)
                Common.ThrowPureVPNCoreException(ErrorCodes.AccessTokenNotFound);
            else if (isAccessTokenGenerating)
                Common.ThrowPureVPNCoreException(ErrorCodes.GeneratingAccessToken);
            else if (authentication.RefreshToken.IsStingNullorEmpty())
                Common.ThrowPureVPNCoreException(ErrorCodes.RefreshTokenNotFound);
        }
    }
}
