using PureVPN.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provides Authentication Token
    /// </summary>
    public interface IAuthTokenService
    {
        /// <summary>
        /// Get Authentication Token <see cref="AuthToken"/>
        /// </summary>
        /// <returns><see cref="AuthToken"/></returns>
        Task<AuthToken> GetAuthToken(string scope);
    }
}
