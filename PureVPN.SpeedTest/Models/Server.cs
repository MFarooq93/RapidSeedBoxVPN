using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class Server
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "port")]
        public int Port { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ip")]
        public string IP { get; set; }
    }
}
