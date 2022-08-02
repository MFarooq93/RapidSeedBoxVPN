using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class Interface
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "internalIp")]
        public string InternalIP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "macAddr")]
        public string MacAddress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isVpn")]
        public bool IsVPN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "externalIp")]
        public string ExternalIP { get; set; }
    }
}
