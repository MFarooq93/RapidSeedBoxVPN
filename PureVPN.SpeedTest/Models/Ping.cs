using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class Ping
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jitter")]
        public double Jitter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "latency")]
        public double Latency { get; set; }
    }
}
