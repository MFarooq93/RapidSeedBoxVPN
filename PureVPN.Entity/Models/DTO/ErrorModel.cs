using Newtonsoft.Json;
using PureVPN.Entity.Enums;
using PureVPN.Realtime.Repository.Infrastructure.Models;
using System.Collections.Generic;
using PureVPN.Entity.Models;

namespace PureVPN.Entity.Models.DTO
{
    public class ErrorModel : BaseNetwork
    {
        [JsonProperty("atom")]
        public Dictionary<string, Atom> Atom { get; set; }
    }

    public class Atom : IEntity
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("primary_cta")]
        public string Cta { get; set; }

        [JsonProperty("secondary_cta")]
        public string SecondaryCta { get; set; }

        [JsonProperty("retryAble")]
        public bool RetryAble { get; set; }

        /// <summary>
        /// Atom Error Code
        /// </summary>
        [JsonProperty("code")]
        public long? Code { get; set; }

        #region IEntity implementations

        /// <summary>
        /// Unique Id
        /// </summary>
        [JsonIgnore]
        public string Id { get; set; }

        /// <summary>
        /// Describes whether entity is deleted
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Clone values from <paramref name="keyValuePairs"/>
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public void Clone(Dictionary<string, object> keyValuePairs)
        {
            // Value are set statically to avoid more use of reflection
            Url = keyValuePairs.GetValue<string>("url");
            Code = keyValuePairs.GetValue<long>("code");
            Title = keyValuePairs.GetValue<string>("title");
            Action = keyValuePairs.GetValue<string>("action");
            Cta = keyValuePairs.GetValue<string>("primary_cta");
            Message = keyValuePairs.GetValue<string>("message");
            RetryAble = keyValuePairs.GetValue<bool>("retryAble");
            SecondaryCta = keyValuePairs.GetValue<string>("secondary_cta");
        }

        #endregion
    }

    public class GeneralErrors
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("primary_cta")]
        public string Cta { get; set; }

        [JsonProperty("secondary_cta")]
        public string SecondaryCta { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

}
