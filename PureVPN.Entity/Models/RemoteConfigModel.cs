using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Represents Remote Config
    /// </summary>
    public class RemoteConfigModel
    {
        [JsonProperty("baseUrls")]
        public List<URLModel> BaseUrls { get; set; }
    }
}
