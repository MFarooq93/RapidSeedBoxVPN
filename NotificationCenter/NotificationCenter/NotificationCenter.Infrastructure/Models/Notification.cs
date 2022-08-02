using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Models
{
    /// <summary>
    /// Represents a Notification
    /// </summary>
    public class Notification : RealmObject
    {
        [JsonIgnore]
        [PrimaryKey]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("expiry")]
        [Ignored]
        public DateTime Expiry { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("is_read")]
        public bool IsRead { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created_on")]
        public long CreatedOn { get; set; }

        [JsonProperty("title")]
        public TextContent Title { get; set; }

        [JsonProperty("description")]
        public TextContent Description { get; set; }

        [JsonProperty("cta_primary")]
        public ClickToAction CtaPrimary { get; set; }

        [JsonProperty("cta_secondary")]
        public ClickToAction CtaSecondary { get; set; }

        [JsonProperty("cta_tertiary")]
        public ClickToAction CtaTertiary { get; set; }
    }

    public class ClickToAction : RealmObject
    {
        [JsonProperty("title")]
        public TextContent Title { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public class TextContent : RealmObject
    {
        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("fr")]
        public string Fr { get; set; }

        [JsonProperty("de")]
        public string De { get; set; }
    }
}
