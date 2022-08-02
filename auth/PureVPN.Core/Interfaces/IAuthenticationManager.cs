using PureVPN.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Interfaces
{
    public interface IAuthenticationManager
    {
        void InitiateAccessTokenRequest();
        Task GenerateAccessToken(string authGrantCode);
        Task<Authentication> GetAccessToken();
        Task<UserInfo> GetUserInto(string token);
        void SetToken(Authentication authentication);
        Task GenerateTokenFromRefreshToken(string refreshToken);

        /// <summary>
        /// Initiate user logout flow
        /// </summary>
        void Logout();
    }
}
