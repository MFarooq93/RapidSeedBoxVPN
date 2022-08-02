using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class Data
    {
        [JsonProperty("isMAAutoLoginAllowed")]
        public bool? IsMembersAreaLoginAllowed { get; set; }

        [JsonProperty("accountCode")]
        public string AccountCode { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subscription")]
        public Dictionary<string, IEnumerable<Subscription>> Subscription { get; set; }
    }
}
