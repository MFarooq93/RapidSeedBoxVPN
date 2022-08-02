using Newtonsoft.Json;
using System.Collections.Generic;

namespace PureVPN.Core.Models
{
    public class UserInfo__
    {
        [JsonProperty("applicationId")]
        internal string ApplicationID { get; set; }

        [JsonProperty("birthdate")]
        public string Birthday { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("sub")]
        internal string Sub { get; set; }
    }
}
