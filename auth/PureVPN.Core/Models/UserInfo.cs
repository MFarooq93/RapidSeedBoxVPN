using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class UserInfoBody : BaseErrorModel
    {
        [JsonProperty("user")]
        public UserInfo UserInfo { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
