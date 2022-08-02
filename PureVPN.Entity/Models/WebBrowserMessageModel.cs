using Newtonsoft.Json;
using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class WebBrowserResponse
    {
        [JsonProperty("response")]
        public WebBrowserMessageModel Response { get; set; }
    }


    /// <summary>
    /// Represents a web browser message
    /// </summary>
    public class WebBrowserMessageModel
    {
        /// <summary>
        /// Describes message type from browser
        /// </summary>
        [JsonProperty("type")]
        public WebBrowserMessageType Type { get; set; }

        /// <summary>
        /// Describes message to show to user
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Describes whether operation in browser succeeded
        /// </summary>
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// <see cref="WebBrowserMessageModel.Type"/> respective data from browser
        /// </summary>
        [JsonProperty("data")]
        public WebBrowserMessageData Data { get; set; }
    }

    /// <summary>
    /// Provides all properties for <see cref="WebBrowserMessageModel.Data"/>
    /// </summary>
    public class WebBrowserMessageData
    {
        /// <summary>
        /// Grant code or access token generation
        /// </summary>
        [JsonProperty("grant_code")]
        public string GrantCode { get; set; }

        /// <summary>
        /// URL to open in external browser
        /// </summary>
        [JsonProperty("link")]
        public string RedirectURL { get; set; }
    }
}
