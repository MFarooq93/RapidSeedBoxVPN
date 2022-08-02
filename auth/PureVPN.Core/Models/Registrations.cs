using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class Registrations
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("insertInstant")]
        public string InsertInstant { get; set; }

        [JsonProperty("lastLoginInstant")]
        public string LastLoginInstant { get; set; }

        [JsonProperty("lastUpdateInstant")]
        public string LastUpdateInstant { get; set; }

        [JsonProperty("usernameStatus")]
        public string UsernameStatus { get; set; }

        [JsonProperty("verified")]
        public string Verified { get; set; }

        [JsonProperty("preferredLanguages")]
        public List<string> PreferredLanguages { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }
}
