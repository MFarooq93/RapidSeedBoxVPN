using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class SpeedTestResult
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ping")]
        public Ping Ping { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "download")]
        public Download Download { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "upload")]
        public Upload Upload { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "packetLoss")]
        public long PacketLoss { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isp")]
        public string ISP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "interface")]
        public Interface Interface { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "server")]
        public Server Server { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public Result Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message")]
        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
