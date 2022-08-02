using Newtonsoft.Json;
using PureVPN.Realtime.Repository.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureVPN.Entity.Models;

namespace PureVPN.Entity.Models.DTO
{
    public class DislikeReasonsModel : IEntity
    {
        [JsonProperty("reasons")]
        public IEnumerable<DislikeReasonModel> Reasons { get; set; }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public void Clone(Dictionary<string, object> keyValuePairs)
        {
            var reasons = keyValuePairs.GetValue<List<object>>("reasons");
            if (reasons != null)
            {
                var reasonModels = new List<DislikeReasonModel>();
                this.Reasons = reasonModels;
                foreach (var item in reasons)
                {
                    var reason = item as Dictionary<string, object>;
                    var reasonModel = new DislikeReasonModel();
                    reasonModel.Clone(reason);
                    reasonModels.Add(reasonModel);
                }
            }
        }
    }

    public class DislikeReasonModel : IEntity
    {
        /// <summary>
        /// Actionf for dislike reason
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// Describes whether comment is required for this reason
        /// </summary>
        [JsonProperty("isReasonRequired")]
        public bool IsReasonRequired { get; set; }

        /// <summary>
        /// Describes watermark for comment box
        /// </summary>
        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        /// <summary>
        /// Describes subtitle
        /// </summary>

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        /// <summary>
        /// Describes title for reason
        /// </summary>

        [JsonProperty("title")]
        public string Title { get; set; }

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
            this.Action = keyValuePairs.GetValue<string>("action");
            this.Title = keyValuePairs.GetValue<string>("title");
            this.Placeholder = keyValuePairs.GetValue<string>("placeholder");
            this.IsReasonRequired = keyValuePairs.GetValue<bool>("isReasonRequired");
            this.Placeholder = keyValuePairs.GetValue<string>("placeholder");
            this.Subtitle = keyValuePairs.GetValue<string>("subtitle");
        }
    }
}