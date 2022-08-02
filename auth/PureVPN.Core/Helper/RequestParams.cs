using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Helper
{
    internal class RequestParams
    {
        internal const string Platform = "platform";
        internal const string ResponseType = "response_type";
        internal const string ClientId = "client_id";
        internal const string RedirectURI = "redirect_uri";
        internal const string UsePKCE = "use_pkce";
        internal const string Scope = "scope";
        internal const string State = "state";
        internal const string CodeChallenge = "code_challenge";
        internal const string CodeChallengeMethod = "code_challenge_method";
        internal const string GrantType = "grant_type";
        internal const string Code = "code";
        internal const string CodeVerifier = "code_verifier";
        internal const string RefreshToken = "refresh_token";
        internal const string PostLogoutRedirectURI = "post_logout_redirect_uri";
        /// <summary>
        /// Request parameter for token
        /// </summary>
        internal const string Token = "token";

        /// <summary>
        /// Request parameter for display
        /// </summary>
        internal const string Display = "display";
    }
}
