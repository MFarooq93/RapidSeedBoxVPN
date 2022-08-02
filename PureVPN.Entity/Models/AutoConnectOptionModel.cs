using Newtonsoft.Json;

namespace PureVPN.Entity.Models
{
    public class AutoConnectOptionModel
    {
        public static string RecommendedLocationLocalizedDisplayName;

        public AutoConnectOptionModel()
        {
            Id = 0;
            Name = "Recommended Location";
            OptionSlug = "Recommended Location";
            DisplayName = RecommendedLocationLocalizedDisplayName;

            IsActive = true;
        }

        public bool IsActive { get; set; }
        //
        // Summary:
        //     Gets or sets the integer id of the protocol.
        [JsonIgnore]
        public int Id { get; set; }
        //
        // Summary:
        //     Gets or sets the ProtocolSlug of the protocol. The valid ProtocolSlug is required
        //     for VPN Dialing.

        public string OptionSlug { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the protocol.
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Text to display under settings
        /// </summary>
        [JsonIgnore]
        public string DisplayName { get; set; }
    }
}
