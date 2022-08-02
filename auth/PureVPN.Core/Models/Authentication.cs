using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class Authentication : BaseErrorModel
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        internal string TokenType { get; set; }

        [JsonProperty("userId")]
        internal string UserId { get; set; }
        
        [JsonProperty("scope")]
        internal string Scope { get; set; }

        public bool IsTokenExpired
        {
            get
            {
                return (DateTime.UtcNow - TokenGenerationTime).TotalSeconds >= Convert.ToInt32(ExpiresIn);
            }
        }

        public DateTime TokenGenerationTime { get; set; }

        public Authentication()
        {
            TokenGenerationTime = DateTime.UtcNow;
        }
    }
}
