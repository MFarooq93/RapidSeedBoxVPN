using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Core.Models
{
    public class BaseErrorModel
    {
        [JsonProperty("error")]
        internal string Error { get; set; }

        [JsonProperty("error_description")]
        internal string ErrorDescription { get; set; }

        [JsonProperty("error_reason")]
        internal string ErrorReason { get; set; }
    }
}
