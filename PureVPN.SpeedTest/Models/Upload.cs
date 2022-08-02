using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.SpeedTest.Models
{
    public class Upload
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "bandwidth")]
        public long Bandwidth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "bytes")]
        public long Bytes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "elapsed")]
        public int Elapsed { get; set; }
    }
}
