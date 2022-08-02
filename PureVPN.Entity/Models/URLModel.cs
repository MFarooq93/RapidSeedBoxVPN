using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Represents a URL
    /// </summary>
    public class URLModel
    {
        /// <summary>
        /// Url to Auth Token API
        /// </summary>
        [JsonProperty("authTokenUrl")]
        public string AuthTokenUrl { get; set; }

        /// <summary>
        /// Url to Services APIs 
        /// </summary>
        [JsonProperty("servicesUrl")]
        public string ServicesUrl { get; set; }

        /// <summary>
        /// Url for Authentication APIs
        /// </summary>
        [JsonProperty("faAPIUrl")]
        public string FaAPIUrl { get; set; }

        /// <summary>
        /// Url for authentication web page
        /// </summary>
        [JsonProperty("faRedirectionWebpageUrl")]
        public string FAWebPageURL { get; set; } = "https://login.purevpn.com/";
    }
}