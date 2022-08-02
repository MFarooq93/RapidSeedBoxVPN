using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Represents Authentication Token
    /// </summary>
    public class AuthToken
    {
        #region Constructors

        public AuthToken(string token)
        {
            Token = token;
        }

        public AuthToken(string token, string tokenType, DateTime tokenGeneratedOnInUtc, int expiresInSeconds)
        {
            Token = $"{tokenType?.Trim()} {token.Trim()}".Trim();
            ExpiresOn = tokenGeneratedOnInUtc.AddSeconds(expiresInSeconds);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Authentication token
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// <see cref="AuthToken"/> expiry DateTime in UTC
        /// </summary>
        public DateTime ExpiresOn { get; }

        /// <summary>
        /// Describes if <see cref="AuthToken"/> has expired
        /// </summary>
        public bool HasExpired => DateTime.UtcNow >= ExpiresOn;
        #endregion
    }
}
