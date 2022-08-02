using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class Result
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url")]
        public string URL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "persisted")]
        public bool Persisted { get; set; }
    }
}
